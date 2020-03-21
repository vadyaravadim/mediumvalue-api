using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using mediumvalue_api.Model;
using System.Text;

namespace mediumvalue_api
{
    public class RequestDatabase
    {
        HttpClient _client;
        public RequestDatabase(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> PushPricesStatisticAsync(string name, int youlaPrice = 0, int avitoPrice = 0, int yandexPrice = 0)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("message", nameof(name));
            }

            if (youlaPrice == 0 && avitoPrice == 0 && yandexPrice == 0)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }

            PricesStatistic item = new PricesStatistic()
            {
                AvitoValue = avitoPrice,
                Name = name,
                YoulaValue = youlaPrice,
                YandexValue = yandexPrice,
                CreatedDate = DateTime.Now
            };

            StringContent content = new StringContent(JsonConvert.SerializeObject(item), Encoding.Unicode, "application/json");
            HttpResponseMessage response = await _client.PostAsync("http://mediumvalue.ru:5130/api/pricesstatistic", content);
            var x = await response.Content.ReadAsStringAsync();
            return response;
        }

        public async Task<PricesStatistic[]> PricesStatisticExistDate(string name)
        {
            if (string.IsNullOrEmpty(name))
            { 
                throw new ArgumentException("message", nameof(name));
            }

            HttpResponseMessage response = await _client.GetAsync($"http://mediumvalue.ru:5130/api/pricesstatistic/checkexist?nameproduct={name}");
            string message = await response.Content.ReadAsStringAsync();
             
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<PricesStatistic[]>(message);
            }

            Console.WriteLine($"unsuccessful request to database: {message}");

            return null;
        }
    } 
}
