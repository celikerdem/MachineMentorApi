using Dapper;
using MachineMentorApi.Helpers;
using MachineMentorApi.Models;
using MachineMentorApi.Models.Commons;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MachineMentorApi.Services
{
    public class CorpusTagService : ServiceBase
    {
        #region | SQL Queries |
        private const string GetCorpusTagsQuery = "SELECT Id, Tag, Icon, Color, TagLevelId, CorpusProjectId, HelpText FROM CorpusTag WHERE CorpusProjectId = @ProjectId";
        #endregion

        public ResponseBase<List<CorpusTag>> GetCorpusTags(int projectId)
        {
            var response = new ResponseBase<List<CorpusTag>>();

            this.RunSafely(() =>
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    response.Data = connection.Query<CorpusTag>(
                        GetCorpusTagsQuery, new
                        {
                            ProjectId = projectId
                        }).ToList();
                    connection.Close();
                }
            }, ex =>
            {
                response.Error(ex);
            });

            return response;
        }
    }
}