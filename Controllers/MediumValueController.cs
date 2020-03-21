using mediumvalue_api.Helper;
using mediumvalue_api.Interface;
using mediumvalue_api.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace mediumvalue_api.MediumValueController
{
    [Route("[controller]")]
    [ApiController]
    public class MediumValueController : ControllerBase
    {
        IParserMethodAvito _basicParser;
        IRequestMethodYoula _basicRequest;
        ISearchYandexLayer _basicSearch;
        IDataControlYandex _yandexRequest;
        BasicImplementation _basicImplementation;
        HandlerRequest _handlerRequest;
        public MediumValueController(IRequestMethodYoula youlaRequest, IParserMethodAvito avitoParser,
                                     ISearchYandexLayer basicSearch, IDataControlYandex yandexRequest,
                                     BasicImplementation basicImplementation, HandlerRequest handlerRequest)
        {
            _basicRequest = youlaRequest;
            _basicParser = avitoParser;
            _basicSearch = basicSearch;
            _yandexRequest = yandexRequest;
            _basicImplementation = basicImplementation;
            _handlerRequest = handlerRequest;
        }

        [HttpPost]
        [Route("avito")]
        public async Task<IActionResult> AvitoPostAsync(DataRequest data)
        {
            _handlerRequest.HandlerStartRequest("avito");

            _basicImplementation = new MediumValueAvito(data, "//span[@data-marker=\"item-price\"] | //h3[@data-marker=\"item-title\"]", "//div[@data-marker=\"pagination-button\"]");
            int mediana = await _basicParser.RunProcessingAsync(data, _basicImplementation);

            _handlerRequest.HandlerEndRequest("avito");

            return new OkObjectResult(mediana);
        }

        [HttpPost]
        [Route("youla")]
        public async Task<IActionResult> YoulaPostAsync(DataRequest data)
        {
            _handlerRequest.HandlerStartRequest("youla");

            _basicImplementation = new MediumValueYoula(data);
            int mediana = await _basicRequest.RunProcessingAsync(data, _basicImplementation);

            _handlerRequest.HandlerEndRequest("youla");

            return new OkObjectResult(mediana);
        }

        [HttpPost]
        [Route("yandex")]
        public async Task<IActionResult> YandexDataControl(DataRequest data)
        {
            _handlerRequest.HandlerStartRequest("yandex");

            _basicImplementation = new MediumValueYandex(data);
            int mediana = await _yandexRequest.RunProcessingAsync(data, _basicImplementation);

            _handlerRequest.HandlerStartRequest("yandex");

            return new OkObjectResult(mediana);

        }

        [HttpGet]
        [Route("searchValue")]
        public async Task<IActionResult> YandexSearchValue(string searchValue)
        {
            string yandexSearchResult = await _basicSearch.RunProcessingAsync(searchValue);
            return new OkObjectResult(yandexSearchResult);
        }
    }
}