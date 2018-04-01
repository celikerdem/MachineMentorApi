using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MachineMentorApi.Models
{
    public class CorpusProject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfUniqueTaggers { get; set; }
        public int MaxDocumentsForEachTagger { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
        public string HelpText { get; set; }
    }
}