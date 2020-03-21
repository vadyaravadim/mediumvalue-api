using HtmlAgilityPack;
using mediumvalue_api.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mediumvalue_api.Interface
{
    public interface IDataControlYandex
    {
        Task<int> RunProcessingAsync(DataRequest data, BasicImplementation medumValue);
        Task<string> GetYandexDataAsync(string searchValue);
        YandexTransformData TransformData(string dataValue);
        Task<int> ControlReturnDataAsync(YandexTransformData dataProduct);
        Task<HtmlDocument> DownloadPageAsync();
        decimal GetPricesItems(HtmlDocument document);
    }

    public interface ISearchYandexLayer
    {
        Task<string> RunProcessingAsync(string searchValue);
    }

    public interface IParserMethodAvito
    {
        Task<int> RunProcessingAsync(DataRequest data, BasicImplementation medumValue);
        int[] PageToFetch(HtmlDocument html);
        Task<List<ParsedHtmlDoc>> DownloadPagesAsync(int[] countPages);
        List<int> GetPricesItems(List<ParsedHtmlDoc> html, DataRequest dataRequest);
    }
    public interface IRequestMethodYoula
    {
        Task<int> RunProcessingAsync(DataRequest data, BasicImplementation medumValue);
        Task<YoulaArray> PagesToProcessingAsync(DataRequest dataRequest);
        IEnumerable<int> GetProductItems(List<YoulaData> youlaArray);
    }
    public interface IParserModel
    {
        internal string PriceItemNode { get; set; }
        internal string PaginationNode { get; set; }
        internal string GeneratedUrl { get; set; }
        internal void GenerateUrl(DataRequest data, string generalUrl);
        HtmlWeb _Web { get; set; }
        HtmlWeb Web { get; set; }
    }
}
