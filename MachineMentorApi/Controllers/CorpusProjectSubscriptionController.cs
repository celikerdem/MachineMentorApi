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
    public class CorpusProjectSubscriptionController : ApiController
    {
        private CorpusTaggerService _corpusTaggerService;
        private CorpusProjectSubscriptionService _corpusProjectSubscriptionService;
        private CorpusProjectService _corpusProjectService;

        public CorpusProjectSubscriptionController()
        {
            _corpusTaggerService = new CorpusTaggerService();
            _corpusProjectSubscriptionService = new CorpusProjectSubscriptionService();
            _corpusProjectService = new CorpusProjectService();
        }

        public void Options() { }

        public ResponseBaseJson<List<CorpusProjectSubscriptionViewModel>> Get(int projectId)
        {
            var response = new ResponseBaseJson<List<CorpusProjectSubscriptionViewModel>>();

            var projectSubscriptionsResponse = _corpusProjectSubscriptionService.GetProjectSubscriptionsByProject(projectId);
            if (response.ValidData(projectSubscriptionsResponse.Data, "ProjectSubscription"))
            {
                response.Success(projectSubscriptionsResponse.Data);
            }

            return response;
        }

        public ResponseBaseJson<CorpusProjectSubscription> Post(CorpusProjectSubscription subscription)
        {
            var response = new ResponseBaseJson<CorpusProjectSubscription>();

            var projectSubscriptionResponse = _corpusProjectSubscriptionService.AddSubscription(subscription.CorpusProjectId, subscription.CorpusTaggerId);
            if (response.ValidData(projectSubscriptionResponse.Data, "ProjectSubscription"))
            {
                response.Success(projectSubscriptionResponse.Data);
            }

            return response;
        }
    }
}
