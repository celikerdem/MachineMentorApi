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
    public class CorpusProjectSubscriptionService : ServiceBase
    {
        #region | SQL Queries |
        private const string GetProjectSubscriptionsByTaggerIdQuery =
            @"SELECT 
	            ISNULL(cps.Id, 0) Id
	            , cp.Id CorpusProjectId
	            , ISNULL(cps.CorpusTaggerId, 0) CorpusTaggerId
	            , ISNULL(ct.Username, '') CorpusTaggerUsername
	            , cp.Name CorpusProjectName
	            , cp.Description CorpusProjectDescription
	            , COUNT(DISTINCT cd.Id) AssignedCorpusDocumentCount
	            , SUM(ISNULL(ctr.IsResponded, 0)) RespondedCorpusDocumentCount
                , CASE WHEN cps.Id IS NULL THEN 0 ELSE 1 END IsSubscribed
            FROM CorpusProject cp
            LEFT JOIN CorpusProjectSubscription cps ON cps.CorpusProjectId = cp.Id AND cps.CorpusTaggerId = @TaggerId 
            LEFT JOIN CorpusTagger ct ON ct.Id = cps.CorpusTaggerId
            LEFT JOIN CorpusTagResponse ctr ON ctr.CorpusTaggerId = ct.Id
            LEFT JOIN CorpusDocument cd ON cd.Id = ctr.CorpusDocumentId 
            WHERE 
                cp.IsActive = 1 
                AND cp.IsDeleted = 0 
            GROUP BY 
	            cps.Id
	            , cp.Id
	            , cps.CorpusTaggerId
	            , ct.Username
	            , cp.Name
	            , cp.Description
            ORDER BY (CASE WHEN cps.Id IS NULL THEN 0 ELSE 1 END) DESC";

        private const string GetProjectSubscriptionsByProjectIdQuery =
            @"SELECT 
	            cps.Id
	            , cps.CorpusProjectId
	            , cps.CorpusTaggerId
	            , ct.Username CorpusTaggerUsername
	            , cp.Name CorpusProjectName
	            , cp.Description CorpusProjectDescription
	            , COUNT(DISTINCT ctr.CorpusDocumentId) AssignedCorpusDocumentCount
	            , SUM(ISNULL(ctr.IsResponded, 0)) RespondedCorpusDocumentCount
            FROM CorpusProjectSubscription cps
            INNER JOIN CorpusProject cp ON cp.Id = cps.CorpusProjectId
            INNER JOIN CorpusTagger ct ON ct.Id = cps.CorpusTaggerId
            LEFT JOIN CorpusDocument cd ON cd.CorpusProjectId = cp.Id 
            LEFT JOIN CorpusTagResponse ctr ON ctr.CorpusTaggerId = ct.Id AND ctr.CorpusDocumentId = cd.Id
            WHERE cps.CorpusProjectId = @ProjectId 
            GROUP BY 
	            cps.Id
	            , cps.CorpusProjectId
	            , cps.CorpusTaggerId
	            , ct.Username
	            , cp.Name
	            , cp.Description
            ORDER BY SUM(ISNULL(ctr.IsResponded, 0)) DESC";

        private const string AddSubscriptionQuery =
            @"INSERT INTO CorpusProjectSubscription 
            (CorpusProjectId, CorpusTaggerId) output INSERTED.Id 
            VALUES 
            (@ProjectId, @TaggerId)";
        #endregion

        public ResponseBase<List<CorpusProjectSubscriptionViewModel>> GetProjectSubscriptionsByTagger(int taggerId)
        {
            var response = new ResponseBase<List<CorpusProjectSubscriptionViewModel>>();

            this.RunSafely(() =>
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    var subscriptions = connection.Query<CorpusProjectSubscriptionViewModel>(
                        GetProjectSubscriptionsByTaggerIdQuery, new
                        {
                            TaggerId = taggerId
                        }).ToList();
                    connection.Close();

                    response.Success(subscriptions);
                }
            }, ex =>
            {
                response.Error(ex);
            });

            return response;
        }

        public ResponseBase<List<CorpusProjectSubscriptionViewModel>> GetProjectSubscriptionsByProject(int projectId)
        {
            var response = new ResponseBase<List<CorpusProjectSubscriptionViewModel>>();

            this.RunSafely(() =>
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    var subscriptions = connection.Query<CorpusProjectSubscriptionViewModel>(
                        GetProjectSubscriptionsByProjectIdQuery, new
                        {
                            ProjectId = projectId
                        }).ToList();
                    connection.Close();

                    response.Success(subscriptions);
                }
            }, ex =>
            {
                response.Error(ex);
            });

            return response;
        }

        public ResponseBase<CorpusProjectSubscription> AddSubscription(int projectId, int taggerId)
        {
            var response = new ResponseBase<CorpusProjectSubscription>();
            this.RunSafely(() =>
            {
                var subscription = new CorpusProjectSubscription { CorpusProjectId = projectId, CorpusTaggerId = taggerId };
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    subscription.Id = Convert.ToInt32(connection.ExecuteScalar(
                        AddSubscriptionQuery, new
                        {
                            ProjectId = projectId,
                            TaggerId = taggerId
                        }));
                    connection.Close();

                    response.Success(subscription);
                }
            }, ex =>
            {
                response.Error(ex);
            });

            return response;
        }
    }
}