using HtmlAgilityPack;
using mediumvalue_api.Helper;
using mediumvalue_api.Interface;
using mediumvalue_api.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace mediumvalue_api
{
    public class MediumValueYandexRequest : IDataControlYandex
    {
        HttpClient _client;
        HtmlWeb _htmlWeb;
        BasicImplementation _basicImplementation;
        RequestDatabase _requestDatabase;
        public MediumValueYandexRequest(HttpClient httpClient, HtmlWeb htmlWeb, RequestDatabase requestDatabase)
        {
            _client = httpClient;
            _htmlWeb = htmlWeb;
            _requestDatabase = requestDatabase;
        }

        public async Task<int> RunProcessingAsync(DataRequest data, BasicImplementation basicObject)
        {
            PricesStatistic pricesStatistic = await HelperDatabaseRequest.SelectExistDatePriceAsync(basicObject.GetQuery, _requestDatabase);
            if (pricesStatistic != null && pricesStatistic.YandexValue != 0)
            {
                return pricesStatistic.YandexValue;
            }

            _basicImplementation = basicObject;

            string searchYandexResult = await GetYandexDataAsync(data.GetQuery);
            YandexTransformData dataProduct = TransformData(searchYandexResult);
            int mediana = await ControlReturnDataAsync(dataProduct);

            if (mediana != 0)
            {
                Task TaskPushPrice = _requestDatabase.PushPricesStatisticAsync(basicObject.GetQuery, yandexPrice: mediana);
            }

            return mediana;
        }

        public async Task<string> GetYandexDataAsync(string searchValue)
        {
            HttpResponseMessage yandexResponse = await _client.GetAsync($"https://yandex.ru/suggest-market/suggest-market-rich?part={searchValue}");
            string searchYandexResult = await yandexResponse.Content.ReadAsStringAsync();
            return searchYandexResult;
        }

        public YandexTransformData TransformData(string dataValue)
        {
            if (string.IsNullOrEmpty(dataValue))
            {
                return null;
            }

            JArray arrayResponse = JArray.Parse(dataValue);
            YandexTransformData yandexData = new YandexTransformData();
            yandexData.ProductName = arrayResponse[1][0].ToString();
            yandexData.ItemPrices = JsonConvert.DeserializeObject<YandexSearchItemInfo[]>(arrayResponse[2][0].ToString())[0];
            return yandexData;
        }
        public async Task<int> ControlReturnDataAsync(YandexTransformData dataProduct)
        {
            IEnumerable<decimal?> products = dataProduct?.ItemPrices?.Prices?.Select(region => region?.Avg).Where(value => value != 0 && value != null);
            if (products != null)
            {
                decimal? pricesRegionSum = products.Sum() / products.Count();
                return decimal.ToInt32(pricesRegionSum.Value);
            }

            HtmlDocument document = await DownloadPageAsync();

            if (document.DocumentNode.InnerText.Contains("Ой!"))
            {
                Console.WriteLine("Load document throw exception: many request from this ip");
                return 0;
            }

            decimal price = GetPricesItems(document);
            return int.Parse(price.ToString());
        }

        public async Task<HtmlDocument> DownloadPageAsync()
        {
            HtmlDocument document = await _htmlWeb.LoadFromWebAsync(_basicImplementation.GeneratedUrl, "saerv@yandex.ru", "1vinnikova1m", "https://passport.yandex.ru/auth/welcome");
            return document;
        }

        public decimal GetPricesItems(HtmlDocument document)
        {
            HtmlNode htmlNode = document.DocumentNode.SelectSingleNode("//span[@class=\"price\"]");
            decimal.TryParse(string.Join("", htmlNode.InnerText.Where(symbol => char.IsDigit(symbol))), out decimal price);
            return price;
        }
    }
}
