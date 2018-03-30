using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MachineMentorApi.Services
{
    public class ServiceBase
    {
        public string GetConnectionString()
        {
            return Properties.Settings.Default.MachineMentorConnectionString;
            //return Properties.Settings.Default.MachineMentorTestConnectionString;
        }
    }
}