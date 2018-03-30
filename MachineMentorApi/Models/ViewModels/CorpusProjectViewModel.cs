using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MachineMentorApi.Models.ViewModels
{
    public class CorpusProjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfUniqueTaggers { get; set; }
        public int MaxDocumentsForEachTagger { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }

        public int IsSubscribed { get; set; }
        public int TargetCorpusDocumentAssignmentCount { get; set; }
        public decimal CorpusTargetCompletionPercentage { get; set; }
    }
}