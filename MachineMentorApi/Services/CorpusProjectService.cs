using Dapper;
using MachineMentorApi.Helpers;
using MachineMentorApi.Models;
using MachineMentorApi.Models.Commons;
using MachineMentorApi.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MachineMentorApi.Services
{
    public class CorpusProjectService : ServiceBase
    {
        #region | SQL Queries |
        private const string GetProjectByIdQuery =
            @"SELECT 
	            cp.Id
	            , cp.Name
	            , cp.Description
	            , cp.NumberOfUniqueTaggers
	            , cp.MaxDocumentsForEachTagger
	            , cp.IsActive
	            , cp.IsDeleted 
	            , COUNT(DISTINCT cd.Id) * cp.NumberOfUniqueTaggers TargetCorpusDocumentAssignmentCount
	            , CASE WHEN EXISTS(SELECT 1 FROM CorpusDocument WHERE IsDeleted = 0 AND CorpusProjectId = cp.Id) THEN SUM(ISNULL(ctr.IsResponded, 0)) * 1.0 / (COUNT(DISTINCT cd.Id) * cp.NumberOfUniqueTaggers) ELSE 0 END CorpusTargetCompletionPercentage
                , cp.HelpText
            FROM CorpusProject cp
            LEFT JOIN CorpusDocument cd ON cd.CorpusProjectId = cp.Id AND cd.IsDeleted = 0
            LEFT JOIN CorpusTagResponse ctr ON ctr.CorpusDocumentId = cd.Id
            WHERE cp.Id = @Id
            GROUP BY
	            cp.Id
	            , cp.Name
	            , cp.Description
	            , cp.NumberOfUniqueTaggers
	            , cp.MaxDocumentsForEachTagger
	            , cp.IsActive
	            , cp.IsDeleted
                , cp.HelpText";
        #endregion

        public ResponseBase<CorpusProjectViewModel> GetProject(int id)
        {
            var response = new ResponseBase<CorpusProjectViewModel>();

            this.RunSafely(() =>
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    response.Data = connection.Query<CorpusProjectViewModel>(
                        GetProjectByIdQuery, new
                        {
                            Id = id
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