using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MachineMentorApi.Models
{
    public class CorpusProjectSubscription
    {
        public int Id { get; set; }

        public int CorpusProjectId { get; set; }
        public int CorpusTaggerId { get; set; }
    }
}