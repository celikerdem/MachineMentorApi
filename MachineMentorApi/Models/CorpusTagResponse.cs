using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MachineMentorApi.Models
{
    public class CorpusTagResponse
    {
        public Int64 Id { get; set; }
        public DateTime AssignDate { get; set; }
        public DateTime RespondDate { get; set; }
        public DateTime SkipDate { get; set; }
        public int IsResponded { get; set; }
        public int IsSkipped { get; set; }
        public int IsObsolete { get; set; }
        public Int64 ObsoleteReferenceId { get; set; }

        public Int64 CorpusDocumentId { get; set; }
        public int CorpusTagId { get; set; }
        public int CorpusTaggerId { get; set; }
    }
}