using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MachineMentorApi.Models
{
    public class CorpusDocument
    {
        public Int64 Id { get; set; }
        public string Key { get; set; }
        public string Text { get; set; }
        public DateTime InsertDate { get; set; }
        public int IsDeleted { get; set; }

        public int CorpusProjectId { get; set; }
    }
}