using System;
using System.Linq;
namespace mediumvalue_api.Model
{
    internal class MediumValueYandex : BasicImplementation
    {
        internal string GeneralUrl = "https://market.yandex.ru";
        public MediumValueYandex(DataRequest data)
        {
            GenerateUrl(data, GeneralUrl);
        }

        internal override void GenerateUrl(DataRequest data, string generalUrl)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (string.IsNullOrEmpty(data.GetQuery))
            {
                throw new ArgumentNullException("parameter GetQuery is null");
            }

            GetQuery = data.GetQuery;
            string resultReqUrl = $"{generalUrl}/search?text={data.GetQuery}&suggest=2&cvredirect=2";
            GeneratedUrl = resultReqUrl;
        }
    }

    internal class MediumValueYoula : BasicImplementation
    {

        internal MediumValueYoula(DataRequest data)
        {
            GenerateUrl(data, GeneralUrl);
        }
        internal string GeneralUrl = "https://api.youla.io/api/v1/";
        internal override void GenerateUrl(DataRequest data, string generalUrl)
        {
            if (data.Service is null)
            {
                throw new ArgumentNullException("parameter Service is null");
            }

            if (data.SubCategories is null)
            {
                throw new ArgumentNullException("parameter SubCategories is null");
            }

            if (data.GetQuery is null)
            {
                throw new ArgumentNullException("parameter GetQuery is null");
            }

            string[] categories = data.SubCategories.Split("/");

            if (!categories.Any())
            {
                throw new ArgumentException("parameter SubCategories not contains category or subcategory");
            }

            string resultReqUrl = $"{generalUrl}{data.Service}?category={categories[0]}&subcategories={categories[1]}";

            GetQuery = data.GetQuery;
            GeneratedUrl = $"{resultReqUrl}&search='{data.GetQuery}'";
        }
    }

    internal class MediumValueAvito : BasicImplementation
    {
        internal MediumValueAvito(DataRequest data, string productItemNode, string paginationNode)
        {
            GenerateUrl(data, GeneralUrl);
            ProductItemNode = productItemNode;
            PaginationNode = paginationNode;
        }

        internal string GeneralUrl = "https://www.avito.ru/";

        internal override void GenerateUrl(DataRequest data, string generalUrl)
        {
            string resultReqUrl = $"{generalUrl}{data.Location}/{data.SubCategories}";

            if (!string.IsNullOrEmpty(data.Brand))
            {
                resultReqUrl += $"/{data.Brand}";
            }
            GetQuery = data.GetQuery;
            GeneratedUrl = $"{resultReqUrl}?s=2&q={data.GetQuery}";
        }
    }
    public class BasicImplementation
    {
        internal string GetQuery { get; set; }
        internal string ProductItemNode { get; set; }
        internal string PaginationNode { get; set; }
        internal string GeneratedUrl { get; set; }
        internal virtual void GenerateUrl(DataRequest data, string generalUrl)
        {
            string resultReqUrl = $"{generalUrl}{data.Location}/{data.SubCategories}";

            if (!string.IsNullOrEmpty(data.Brand))
            {
                resultReqUrl += $"/{data.Brand}";
            }
            GetQuery = data.GetQuery;
            GeneratedUrl = $"{resultReqUrl}?s=2&q={data.GetQuery}";
        }
    }
}
