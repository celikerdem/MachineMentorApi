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
    public class CorpusTagResponseController : ApiController
    {
        private CorpusTagResponseService _corpusTagResponseService;

        public CorpusTagResponseController()
        {
            _corpusTagResponseService = new CorpusTagResponseService();
        }

        public void Options() { }

        public ResponseBaseJson<CorpusTagResponse> Get(int projectId, int taggerId, Int64 originId, int iteration)
        {
            var response = new ResponseBaseJson<CorpusTagResponse>();

            var tagResResponse = _corpusTagResponseService.GetAssignedTagResponse(projectId, taggerId, originId, iteration);
            if (tagResResponse.Data != null && (tagResResponse.Data.Id != originId || iteration < 0))
            {
                response.Success(tagResResponse.Data);
            }
            else
            {
                var assignResponse = _corpusTagResponseService.AssignNewTagDocument(projectId, taggerId);
                if (assignResponse.Status == ServiceResponseStatuses.Success)
                {
                    var newTagResResponse = _corpusTagResponseService.GetTagResponse(assignResponse.Data.Id);
                    if (response.ValidData(newTagResResponse.Data, "TagResponse"))
                        response.Success(newTagResResponse.Data);
                    else if (tagResResponse.Data != null)
                        response.Success(tagResResponse.Data);
                }
            }

            return response;
        }

        public ResponseBaseJson<CorpusTagResponse> Put(CorpusTagResponse tagResponse)
        {
            var response = new ResponseBaseJson<CorpusTagResponse>();

            var tagResResponse = _corpusTagResponseService.SubmitTagResponse(tagResponse.Id, tagResponse.CorpusTagId);
            if (tagResResponse.Status == ServiceResponseStatuses.Success)
            {
                response.Success(tagResponse);
            }

            return response;
        }
    }
}
