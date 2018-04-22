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
    public class CorpusTaggerService : ServiceBase
    {
        #region | SQL Queries |
        private const string GetTaggersQuery = @"SELECT Id, Email, Username, IsAuthorized, CASE WHEN Id=1 THEN 1 ELSE 0 END IsAdmin FROM CorpusTagger ORDER BY Id DESC";
        private const string GetTaggerByUsernameQuery = @"SELECT Id, Email, Username, IsAuthorized, CASE WHEN Id=1 THEN 1 ELSE 0 END IsAdmin FROM CorpusTagger WHERE Username = @Username";
        private const string CheckUsernameOrEmailExistsQuery = @"SELECT Id FROM CorpusTagger WHERE Username = @Username OR Email = @Email";
        private const string AddNewTaggerQuery =
            @"INSERT INTO CorpusTagger 
            (Email, Username, IsAuthorized) output INSERTED.Id 
            VALUES (@Email, @Username, 0)";
        private const string UpdateTaggerQuery =
            @"UPDATE CorpusTagger 
            SET IsAuthorized = @IsAuthorized
            WHERE Id = @Id";
        #endregion

        public ResponseBase<List<CorpusTagger>> GetTaggers()
        {
            var response = new ResponseBase<List<CorpusTagger>>();

            this.RunSafely(() =>
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    var taggerResponse = connection.Query<CorpusTagger>(GetTaggersQuery).ToList();
                    connection.Close();

                    response.Success(taggerResponse);
                }
            }, ex =>
            {
                response.Error(ex);
            });

            return response;
        }

        public ResponseBase<CorpusTagger> GetTagger(string username)
        {
            var response = new ResponseBase<CorpusTagger>();

            this.RunSafely(() =>
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    var taggerResponse = connection.Query<CorpusTagger>(
                        GetTaggerByUsernameQuery, new
                        {
                            Username = username
                        }).FirstOrDefault();
                    connection.Close();

                    response.Success(taggerResponse);
                }
            }, ex =>
            {
                response.Error(ex);
            });

            return response;
        }

        public ResponseBase<CorpusTagger> AddTagger(CorpusTagger tagger)
        {
            var response = new ResponseBase<CorpusTagger>();
            if (string.IsNullOrEmpty(tagger.Email))
            {
                response.Error("RequiredField", "Email cannot be empty!");
                return response;
            }
            if (string.IsNullOrEmpty(tagger.Username))
            {
                response.Error("RequiredField", "Username cannot be empty!");
                return response;
            }

            this.RunSafely(() =>
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    //Check username or email exists
                    var existingUser = connection.Query<int>(
                        CheckUsernameOrEmailExistsQuery, new
                        {
                            Username = tagger.Username,
                            Email = tagger.Email
                        }).FirstOrDefault();
                    if (existingUser > 0)
                    {
                        response.Error("TaggerAlreadyExists", "Tagger already exists!");
                    }
                    else
                    {
                        tagger.Id = Convert.ToInt32(connection.ExecuteScalar(
                            AddNewTaggerQuery, new
                            {
                                Email = tagger.Email,
                                Username = tagger.Username
                            }));
                        connection.Close();
                        response.Success(tagger);
                    }
                }
            }, ex =>
            {
                response.Error(ex);
            });

            return response;
        }


        public ResponseBase<CorpusTagger> UpdateTagger(CorpusTagger tagger)
        {
            var response = new ResponseBase<CorpusTagger>();
            if (string.IsNullOrEmpty(tagger.Email))
            {
                response.Error("RequiredField", "Email cannot be empty!");
                return response;
            }
            if (string.IsNullOrEmpty(tagger.Username))
            {
                response.Error("RequiredField", "Username cannot be empty!");
                return response;
            }

            this.RunSafely(() =>
            {
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    tagger.Id = Convert.ToInt32(connection.Execute(
                        UpdateTaggerQuery, new
                        {
                            Id = tagger.Id,
                            IsAuthorized = tagger.IsAuthorized
                        }));
                    connection.Close();
                    response.Success(tagger);
                }
            }, ex =>
            {
                response.Error(ex);
            });

            return response;
        }
    }
}