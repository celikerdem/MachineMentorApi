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
    public class CorpusTagController : ApiController
    {
        private CorpusTagService _corpusTagService;

        public CorpusTagController()
        {
            _corpusTagService = new CorpusTagService();
        }

        public void Options() { }

        public ResponseBaseJson<List<CorpusTag>> Get(int projectId)
        {
            var response = new ResponseBaseJson<List<CorpusTag>>();

            var corpusTagResponse = _corpusTagService.GetCorpusTags(projectId);
            if (response.ValidData(corpusTagResponse.Data, "CorpusTag"))
            {
                response.Success(corpusTagResponse.Data);
            }

            return response;
        }
    }
}
