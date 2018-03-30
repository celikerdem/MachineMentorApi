using MachineMentorApi.Models;
using MachineMentorApi.Models.Commons;
using MachineMentorApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MachineMentorApi.Controllers
{
    public class CorpusTaggerController : ApiController
    {
        private CorpusTaggerService _corpusTaggerService;

        public CorpusTaggerController()
        {
            _corpusTaggerService = new CorpusTaggerService();
        }

        public void Options() { }

        public ResponseBaseJson<CorpusTagger> Get(string username)
        {
            var response = new ResponseBaseJson<CorpusTagger>();

            var taggerResponse = _corpusTaggerService.GetTagger(username);
            if (response.ValidData(taggerResponse.Data, "Tagger"))
            {
                if (taggerResponse.Data.IsAuthorized == 1)
                    response.Success(taggerResponse.Data);
                else
                    response.Error("unauthorized-login", "Tagger is not authorized!");
            }

            return response;
        }

        public ResponseBaseJson<CorpusTagger> Post(CorpusTagger tagger)
        {
            var response = new ResponseBaseJson<CorpusTagger>();

            var taggerResponse = _corpusTaggerService.AddTagger(tagger);
            if (taggerResponse.Status == ServiceResponseStatuses.Success)
            {
                response.Success(taggerResponse.Data);
            }
            else
            {
                response.Error(taggerResponse.Messages);
            }

            return response;
        }
    }
}
