using mediumvalue_api.Helper;
using mediumvalue_api.Interface;
using mediumvalue_api.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SentencesFuzzyComparison;

namespace mediumvalue_api
{
    public class MediumValueYoulaRequest : IRequestMethodYoula
    {
        BasicImplementation _basicObject;
        HttpClient _client;
        int _quantityPages = 50;
        RequestDatabase _requestDatabase;
        FuzzyComparer _fuzzyComparer;
        public MediumValueYoulaRequest(RequestDatabase requestDatabase, HttpClient client, FuzzyComparer fuzzyComparer)
        {
            _requestDatabase = requestDatabase;
            _client = client;
            _fuzzyComparer = fuzzyComparer;
        }

        public async Task<int> RunProcessingAsync(DataRequest data, BasicImplementation basicObject)
        {
            PricesStatistic pricesStatistic = await HelperDatabaseRequest.SelectExistDatePriceAsync(basicObject.GetQuery, _requestDatabase);
            if (pricesStatistic != null && pricesStatistic.YoulaValue != 0)
            {
                return pricesStatistic.YoulaValue;
            }

            _basicObject = basicObject;

            YoulaArray youlaArray = await PagesToProcessingAsync(data);
            if (youlaArray.Data == null || !youlaArray.Data.Any())
            {
                return 0;
            }

            IEnumerable<int> prices = GetProductItems(youlaArray.Data);

            (int mediana, bool isEven) = MathStatistic.CalcMediana(prices.ToList());

            if (mediana != 0)
            {
                Task TaskPushPrice = _requestDatabase.PushPricesStatisticAsync(basicObject.GetQuery, youlaPrice: mediana);
            }

            return mediana;
        }

        public async Task<YoulaArray> PagesToProcessingAsync(DataRequest dataRequest)
        {
            YoulaArray arrays = new YoulaArray();
            arrays.Data = new List<YoulaData>();
            for (int counter = 0; counter < _quantityPages; counter++)
            {
                YoulaArray youla = await RequestPaginationYoulaAsync(counter);

                if (youla.Data.Count() == 0 && counter > 1)
                {
                    return arrays;
                }

                Parallel.ForEach(youla.Data, (product) =>
                {
                    if (youla.Data == null || !youla.Data.Any())
                    {
                        return;
                    }

                    if (_fuzzyComparer.IsFuzzyEqual(product.Name, dataRequest.GetQuery))
                    {
                        arrays.Data.Add(product);
                    }
                });
            }

            return arrays;
        }

        private async Task<YoulaArray> RequestPaginationYoulaAsync(int counter)
        {
            HttpResponseMessage httpReponse = await _client.GetAsync($"{_basicObject.GeneratedUrl}&page={counter}");
            HttpContent contentResponse = httpReponse.Content;
            string message = await contentResponse.ReadAsStringAsync();
            YoulaArray youla = JsonConvert.DeserializeObject<YoulaArray>(message);
            return youla;
        }

        public IEnumerable<int> GetProductItems(List<YoulaData> youlaArray)
        {
            foreach (YoulaData product in youlaArray)
            {
                yield return int.Parse(product.Price.Remove(product.Price.Length - 2));
            }
        }
    }
}
