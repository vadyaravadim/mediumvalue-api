using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using mediumvalue_api.Interface;
using mediumvalue_api.Model;
using mediumvalue_api.Helper;
using System;
using SentencesFuzzyComparison;
using System.Net;
using System.Text;
using System.Net.Http;

namespace mediumvalue_api
{
    public class MediumValueAvitoParser : IParserMethodAvito
    {
        BasicImplementation _basicObject;
        HtmlWeb _htmlWeb;
        RequestDatabase _requestDatabase;
        HandlerRequest _handlerRequest;
        FuzzyComparer _fuzzyComparer;
        public MediumValueAvitoParser(RequestDatabase requestDatabase, HtmlWeb htmlWeb, HandlerRequest handlerRequest, FuzzyComparer fuzzyComparer)
        {
            _requestDatabase = requestDatabase;
            _htmlWeb = htmlWeb;
            _handlerRequest = handlerRequest;
            _fuzzyComparer = fuzzyComparer;
        }

        public async Task<int> RunProcessingAsync(DataRequest data, BasicImplementation basicObject)
        {
            PricesStatistic pricesStatistic = await HelperDatabaseRequest.SelectExistDatePriceAsync(basicObject.GetQuery, _requestDatabase);
            if (pricesStatistic != null && pricesStatistic.AvitoValue != 0)
            {
                return pricesStatistic.AvitoValue;
            }

            _basicObject = basicObject;

            HtmlDocument html = await _htmlWeb.LoadFromWebAsync(_basicObject.GeneratedUrl);
            if (html.DocumentNode.InnerText.Contains("Ваш браузер не поддерживается"))
            {
                Console.WriteLine("Load document throw exception: browser is not supported");
                throw new HttpRequestException("Load document throw exception: browser is not supported");
            }

            int[] countPages = PageToFetch(html);
            List<ParsedHtmlDoc> htmlDocs = await DownloadPagesAsync(countPages);
            List<int> prices = GetPricesItems(htmlDocs, data);

            (int mediana, bool isEven) = MathStatistic.CalcMediana(prices);

            if (mediana != 0)
            {
                Task TaskPushPrice = _requestDatabase.PushPricesStatisticAsync(basicObject.GetQuery, avitoPrice: mediana);
            }

            return mediana;
        }

        public List<int> GetPricesItems(List<ParsedHtmlDoc> htmlDocs, DataRequest dataRequest)
        {
            List<int> allPrice = new List<int>();

            for (int counter = 0; counter < htmlDocs.Count(); counter++)
            {
                HtmlNodeCollection pageProducts = htmlDocs[counter].Html.DocumentNode.SelectNodes(_basicObject.ProductItemNode);

                for (int nodeProductItem = 0; nodeProductItem < pageProducts.Count(); nodeProductItem++)
                {
                    string s = pageProducts[nodeProductItem].InnerText;
                    if (_fuzzyComparer.IsFuzzyEqual(pageProducts[nodeProductItem].InnerText, dataRequest.GetQuery))
                    {
                        string nodePrice = pageProducts[nodeProductItem + 1].InnerText;
                        int.TryParse(string.Join("", nodePrice.Where(symbol => char.IsDigit(symbol))), out int price);
                        allPrice.Add(price);
                    }
                    nodeProductItem++;
                }
            }

            return allPrice;
        }

        public int[] PageToFetch(HtmlDocument html)
        {
            HtmlNode nodePages = html.DocumentNode.SelectSingleNode(_basicObject.PaginationNode);
            if (nodePages == null)
            {
                return new int[] { 1 };
            }

            List<int> countPages = new List<int>(), valuePages = new List<int>();

            foreach(HtmlNode node in nodePages.ChildNodes)
            {
                if (int.TryParse(node.InnerText, out int value))
                {
                    countPages.Add(value);
                }
            }

            (int page, bool isEven) = MathStatistic.CalcMediana(countPages, true);

            if (isEven)
            {
                return new int[] { page, page + 1 };
            }
            else
            {
                return new int[] { page };
            }
        }

        public async Task<List<ParsedHtmlDoc>> DownloadPagesAsync(int[] countPages)
        {
            List<ParsedHtmlDoc> htmlDocs = new List<ParsedHtmlDoc>();
            foreach(int page in countPages)
            {
                string categoryPage = $"{_basicObject.GeneratedUrl}&p={page}";
                HtmlDocument htmlDoc = await _htmlWeb.LoadFromWebAsync(categoryPage);
                htmlDocs.Add(new ParsedHtmlDoc(htmlDoc, page));
            }
            return htmlDocs;
        }
    }
}
