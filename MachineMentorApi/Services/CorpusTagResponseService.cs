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
    public class CorpusTagResponseService : ServiceBase
    {
        #region | SQL Queries |
        private const string GetTagResponseQuery =
            @"SELECT TOP 1 
                ctr.Id
                , ctr.AssignDate
                , ctr.RespondDate
                , ctr.SkipDate
                , ctr.IsResponded
                , ctr.IsSkipped
                , ctr.IsObsolete
                , ctr.ObsoleteReferenceId
                , ctr.CorpusDocumentId
                , ctr.CorpusTagId
                , ctr.CorpusTaggerId 
            FROM CorpusTagResponse ctr 
            WHERE 
                ctr.Id = @Id";

        private const string GetTagLastResponseQuery =
            @"SELECT TOP 1 
                ctr.Id
                , ctr.AssignDate
                , ctr.RespondDate
                , ctr.SkipDate
                , ctr.IsResponded
                , ctr.IsSkipped
                , ctr.IsObsolete
                , ctr.ObsoleteReferenceId
                , ctr.CorpusDocumentId
                , ctr.CorpusTagId
                , ctr.CorpusTaggerId 
            FROM CorpusTagResponse ctr 
		    INNER JOIN CorpusDocument cd ON cd.Id = ctr.CorpusDocumentId 
            WHERE 
				cd.CorpusProjectId = @ProjectId 
				AND ctr.CorpusTaggerId = @TaggerId 
                AND ctr.IsObsolete = 0 
            ORDER BY ctr.Id DESC";

        private const string GetAssignedTagResponseQuery =
            @"SELECT TOP 1 * 
            FROM ( 
			    SELECT TOP {0} 
					    ctr.Id
					    , ctr.AssignDate
					    , ctr.RespondDate
					    , ctr.SkipDate
					    , ctr.IsResponded
					    , ctr.IsSkipped
					    , ctr.IsObsolete
					    , ctr.ObsoleteReferenceId
					    , ctr.CorpusDocumentId
					    , ctr.CorpusTagId
					    , ctr.CorpusTaggerId 
				    FROM CorpusTagResponse ctr 
				    INNER JOIN CorpusDocument cd ON cd.Id = ctr.CorpusDocumentId 
				    WHERE 
					    cd.CorpusProjectId = @ProjectId 
					    AND ctr.CorpusTaggerId = @TaggerId 
					    AND ((@OriginId > 0 AND ((@Iteration > 0 AND ctr.Id > @OriginId) OR (@Iteration = 0 AND ctr.Id = @OriginId) OR (@Iteration < 0 AND ctr.Id < @OriginId))) OR (@OriginId <= 0 AND ctr.IsResponded = 0)) 
					    AND ctr.IsObsolete = 0 
				    ORDER BY ctr.Id {1} 
		    ) sq 
            ORDER BY sq.Id {2}";

        private const string AssignNewTagDocumentQuery =
            @"INSERT INTO CorpusTagResponse
	            (ctr.AssignDate
	            , ctr.RespondDate
	            , ctr.SkipDate
	            , ctr.IsResponded
	            , ctr.IsSkipped
	            , ctr.IsObsolete
	            , ctr.ObsoleteReferenceId
	            , ctr.CorpusDocumentId
	            , ctr.CorpusTagId
	            , ctr.CorpusTaggerId ) output INSERTED.Id 
            SELECT TOP 1
	            GETDATE()
	            , NULL
	            , NULL
	            , 0
	            , 0
	            , 0
	            , 0
	            , cd.Id
	            , 0
	            , @TaggerId
            FROM CorpusDocument cd
            INNER JOIN CorpusProject cp ON cp.Id = cd.CorpusProjectId
            LEFT JOIN CorpusTagResponse ctr ON ctr.CorpusDocumentId = cd.Id AND ctr.IsObsolete = 0
            WHERE
	            cd.CorpusProjectId = @ProjectId
	            AND cd.Id NOT IN (SELECT
							            cd.Id
						            FROM CorpusDocument cd
						            INNER JOIN CorpusTagResponse ctr ON ctr.CorpusDocumentId = cd.Id AND ctr.IsObsolete = 0
						            WHERE
							            ctr.CorpusTaggerId = @TaggerId)
            GROUP BY
	            cd.Id
	            , cp.NumberOfUniqueTaggers
            HAVING
	            cp.NumberOfUniqueTaggers > SUM(CASE WHEN ISNULL(ctr.Id,0) > 0 THEN 1 ELSE 0 END)
            ORDER BY
	            COUNT(ctr.Id) ASC";

        private const string SubmitTagResponseQuery =
            @"UPDATE CorpusTagResponse
            SET IsResponded = 1
                , RespondDate = GETDATE()
                , CorpusTagId = @tagId
            WHERE Id = @tagResponseId";
        #endregion

        public ResponseBase<CorpusTagResponse> GetTagResponse(Int64 id)
        {
            var response = new ResponseBase<CorpusTagResponse>();

            this.RunSafely(() =>
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    response.Data = connection.Query<CorpusTagResponse>(
                        GetTagResponseQuery, new
                        {
                            Id = id,
                        }).FirstOrDefault();
                    connection.Close();
                }
            }, ex =>
            {
                response.Error(ex);
            });

            return response;
        }

        public ResponseBase<CorpusTagResponse> GetAssignedTagResponse(int projectId, int taggerId, Int64 originId, int iteration)
        {
            var response = new ResponseBase<CorpusTagResponse>();
            if (originId == 0)
                iteration = 0;

            this.RunSafely(() =>
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    var respData = connection.Query<CorpusTagResponse>(
                        string.Format(GetAssignedTagResponseQuery
                            , iteration > 0 ? iteration : (iteration < 0 ? Math.Abs(iteration) : 1)
                            , iteration >= 0 ? "ASC" : "DESC"
                            , iteration >= 0 ? "DESC" : "ASC"), new
                            {
                                ProjectId = projectId,
                                TaggerId = taggerId,
                                OriginId = originId,
                                Iteration = iteration
                            }).FirstOrDefault();
                    if (respData != null)
                        response.Success(respData);
                    //İleri geri hareketlerde, atanmış farklı bir doküman bulunmazsa, aynı dokümanı getir
                    else if (originId != 0)
                    {
                        respData = connection.Query<CorpusTagResponse>(
                            GetTagResponseQuery, new
                            {
                                Id = originId
                            }).FirstOrDefault();
                        response.Success(respData);
                    }
                    //Atanmış ve cevaplanmamış doküman kalmadıysa, son atanmış dokümanı getir
                    else
                    {
                        respData = connection.Query<CorpusTagResponse>(
                            GetTagLastResponseQuery, new
                            {
                                ProjectId = projectId,
                                TaggerId = taggerId
                            }).FirstOrDefault();
                        response.Success(respData);
                    }
                    connection.Close();
                }
            }, ex =>
            {
                response.Error(ex);
            });

            return response;
        }

        public ResponseBase<CorpusTagResponse> AssignNewTagDocument(int projectId, int taggerId)
        {
            var response = new ResponseBase<CorpusTagResponse>();

            this.RunSafely(() =>
            {
                var tagResponse = new CorpusTagResponse();
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    tagResponse.Id = Convert.ToInt64(connection.ExecuteScalar(
                        AssignNewTagDocumentQuery, new
                        {
                            ProjectId = projectId,
                            TaggerId = taggerId
                        }));
                    connection.Close();
                    response.Success(tagResponse);
                }
            }, ex =>
            {
                response.Error(ex);
            });

            return response;
        }

        public ResponseBase<bool> SubmitTagResponse(Int64 tagResponseId, int tagId)
        {
            var response = new ResponseBase<bool>();

            this.RunSafely(() =>
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    connection.Execute(
                        SubmitTagResponseQuery, new
                        {
                            TagResponseId = tagResponseId,
                            TagId = tagId
                        });
                    connection.Close();
                }
                response.Success(true);
            }, ex =>
            {
                response.Error(ex);
            });

            return response;
        }
    }
}