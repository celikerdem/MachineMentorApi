using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MachineMentorApi.Controllers;
using MachineMentorApi.Services;
using MachineMentorApi.Models;

namespace MachineMentorApi.Tests.Controllers
{
    [TestClass]
    public class CorpusProjectControllerTests
    {
        [TestMethod]
        public void GetCorpusProjects()
        {
            var controller = new CorpusProjectController();
            var response = controller.Get(1);
            Assert.IsTrue(response.Data.Count > 0);
        }

        [TestMethod]
        public void GetCorpusProjectSubscriptions()
        {
            var controller = new CorpusProjectSubscriptionController();
            var response = controller.Get(1);
            Assert.IsTrue(response.Data.Count > 0);
        }

        [TestMethod]
        public void GetAssignedTagResponse()
        {
            var controller = new CorpusTagResponseController();
            var response = controller.Get(2, 4, 0, 0);
            Assert.IsTrue(response.Data.Id > 0);
        }

        [TestMethod]
        public void AssignNewTagDocument()
        {
            var controller = new CorpusTagResponseService();
            var response = controller.AssignNewTagDocument(1, 1);
            Assert.IsTrue(response.Data.Id > 0);
        }

        [TestMethod]
        public void AddNewTager()
        {
            var controller = new CorpusTaggerController();
            var tagger = new CorpusTagger { Email = "besikciozge@gmail", Username = "besikciozge" };
            var response = controller.Post(tagger);
            Assert.IsTrue(response.Messages[0].Key == "TaggerAlreadyExists");
        }
    }
}
