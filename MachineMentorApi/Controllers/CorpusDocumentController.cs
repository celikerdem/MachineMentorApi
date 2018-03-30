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
    public class CorpusDocumentController : ApiController
    {
        private CorpusDocumentService _corpusDocumentService;

        public CorpusDocumentController()
        {
            _corpusDocumentService = new CorpusDocumentService();
        }

        public void Options() { }

        public ResponseBaseJson<CorpusDocument> Get(int documentId)
        {
            var response = new ResponseBaseJson<CorpusDocument>();

            var corpusDocumentResponse = _corpusDocumentService.GetCorpusDcoument(documentId);
            if (response.ValidData(corpusDocumentResponse.Data, "CorpusDocument"))
            {
                response.Success(corpusDocumentResponse.Data);
            }

            return response;
        }
    }
}
