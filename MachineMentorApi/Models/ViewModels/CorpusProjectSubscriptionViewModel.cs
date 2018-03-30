using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MachineMentorApi.Models.ViewModels
{
    public class CorpusProjectSubscriptionViewModel
    {
        public int Id { get; set; }

        public int CorpusProjectId { get; set; }
        public int CorpusTaggerId { get; set; }

        public string CorpusProjectName { get; set; }
        public string CorpusProjectDescription { get; set; }

        public string CorpusTaggerUsername { get; set; }
        public int AssignedCorpusDocumentCount { get; set; }
        public int RespondedCorpusDocumentCount { get; set; }
    }
}