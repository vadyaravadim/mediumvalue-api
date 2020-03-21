using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace mediumvalue_api.Model
{
    /// <summary>
    /// Namespace for deserialization response service
    /// </summary>
    /// 

    public class YoulaArray
    {
        [JsonProperty("data")]
        public List<YoulaData> Data;
    }
    public class YoulaData
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("price")]
        public string Price { get; set; }
        [JsonProperty("date_created")]
        public string DateCreated { get; set; }
        [JsonProperty("location")]
        public YoulaLocation Location { get; set; }
    }

    public class YoulaLocation
    {
        [JsonProperty("city_name")]
        public string CityName { get; set; }
        [JsonProperty("city")]
        public string CityId { get; set; }
    }

    public class DataRequest
    {
        public string Location { get; set; }
        public string SubCategories { get; set; }
        public string Brand { get; set; }
        public string GetQuery { get; set; }
        public string Page { get; set; }
        public string GeneralUrl { get; set; }
        public string GeneratedUrl { get; set; }
        public string Service { get; set; }
    }

    public class ProductItem
    {
        public int[] Price { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
    }

    public class ParsedHtmlDoc
    {
        public HtmlDocument Html { get; set; }
        public int ValuePage { get; set; }

        public ParsedHtmlDoc(HtmlDocument html, int valuePage)
        {
            Html = html;
            ValuePage = valuePage;
        }
    }

    public class YandexSearchLayerResponse
    {
        public YandexSearchItemInfo YandexSearchItem { get; set; }
    }

    #region Transform data for Yandex medium value
    public class YandexTransformData
    {
        [JsonProperty("productName")]
        public string ProductName { get; set; }
        [JsonProperty("itemPrices")]
        public YandexSearchItemInfo ItemPrices { get; set; }
    }

    public class YandexSearchItemInfo
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }
        [JsonProperty("popularity")]
        public decimal Popularity { get; set; }
        [JsonProperty("prices")]
        public YandexSearchItemPrices[] Prices { get; set; }
    }

    public class YandexSearchItemPrices
    {
        [JsonProperty("max")]
        public decimal Max { get; set; }
        [JsonProperty("region")]
        public int Region { get; set; }
        [JsonProperty("avg")]
        public decimal Avg { get; set; }
        [JsonProperty("min")]
        public decimal Min { get; set; }
    }

    #endregion
}
