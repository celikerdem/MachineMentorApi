using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MachineMentorApi.Models
{
    public class CorpusTagger
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public int IsAuthorized { get; set; }
    }
}