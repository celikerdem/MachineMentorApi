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
        private const string GetTaggerByUsernameQuery = @"SELECT Id, Email, Username, IsAuthorized FROM CorpusTagger WHERE Username = @Username";
        private const string CheckUsernameOrEmailExists = @"SELECT Id FROM CorpusTagger WHERE Username = @Username OR Email = @Email";
        private const string AddNewTaggerQuery = 
            @"INSERT INTO CorpusTagger 
            (Email, Username, IsAuthorized) output INSERTED.Id 
            VALUES (@Email, @Username, 0)";
        #endregion

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
                        CheckUsernameOrEmailExists, new
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
    }
}