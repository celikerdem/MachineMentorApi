using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MachineMentorApi.Models
{
    public class CorpusTag
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }

        public TagLevel TagLevelId { get; set; }
        public int CorpusProjectId { get; set; }
    }

    public enum TagLevel
    {
        Document = 1,
        Sentence = 2,
        Word = 3
    }
}