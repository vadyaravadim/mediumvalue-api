using mediumvalue_api.Interface;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace mediumvalue_api
{
    public class SearchYandexRequest : ISearchYandexLayer
    {
        HttpClient _client;
        public SearchYandexRequest(HttpClient client)
        {
            _client = client;
        }
        public async Task<string> RunProcessingAsync(string searchValue)
        {
            HttpResponseMessage yandexResponse = await _client.GetAsync($"https://yandex.ru/suggest-market/suggest-market-rich?v=2&enable-continuation=1&group_sort=max&hl=1&srv=market&wiz=TrWth&pos=6&svg=1&_=1583584388921&part={searchValue}");
            string searchYandexResult = await yandexResponse.Content.ReadAsStringAsync();
            JArray arrayResponse = JArray.Parse(searchYandexResult);
            return arrayResponse[1].ToString();
        }
    }
}
