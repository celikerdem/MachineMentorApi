using MachineMentorApi.Models;
using MachineMentorApi.Models.Commons;
using MachineMentorApi.Models.ViewModels;
using MachineMentorApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MachineMentorApi.Controllers
{
    public class CorpusProjectController : ApiController
    {
        private CorpusTaggerService _corpusTaggerService;
        private CorpusProjectSubscriptionService _corpusProjectSubscriptionService;
        private CorpusProjectService _corpusProjectService;

        public CorpusProjectController()
        {
            _corpusTaggerService = new CorpusTaggerService();
            _corpusProjectSubscriptionService = new CorpusProjectSubscriptionService();
            _corpusProjectService = new CorpusProjectService();
        }

        public void Options() { }

        public ResponseBaseJson<List<CorpusProjectViewModel>> Get(int taggerId)
        {
            var response = new ResponseBaseJson<List<CorpusProjectViewModel>>();

            var projectSubscriptionsResponse = _corpusProjectSubscriptionService.GetProjectSubscriptionsByTagger(taggerId);
            if (response.ValidData(projectSubscriptionsResponse.Data, "ProjectSubscription"))
            {
                var corpusProjectList = new List<CorpusProjectViewModel>();
                foreach (var projectSubscription in projectSubscriptionsResponse.Data)
                {
                    var projectResponse = _corpusProjectService.GetProject(projectSubscription.CorpusProjectId);
                    projectResponse.Data.IsSubscribed = projectSubscription.Id > 0 ? 1 : 0;
                    if (projectResponse != null)
                        corpusProjectList.Add(projectResponse.Data);
                }

                response.Success(corpusProjectList);
            }

            return response;
        }
    }
}
