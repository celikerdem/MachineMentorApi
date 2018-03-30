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
    public class CorpusDocumentService : ServiceBase
    {
        #region | SQL Queries |
        private const string GetCorpusTagsQuery = "SELECT Id, [Key], Text, InsertDate, IsDeleted, CorpusProjectId FROM CorpusDocument WHERE Id = @documentId";
        #endregion

        public ResponseBase<CorpusDocument> GetCorpusDcoument(int documentId)
        {
            var response = new ResponseBase<CorpusDocument>();

            this.RunSafely(() =>
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    response.Data = connection.Query<CorpusDocument>(
                        GetCorpusTagsQuery, new
                        {
                            DocumentId = documentId
                        }).FirstOrDefault();
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