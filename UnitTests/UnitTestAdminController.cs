using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using OnlineDiary.Controllers;
using NUnit.Framework;
using OnlineDiary.Models.CRUDViewModels;

namespace UnitTests
{
    [TestClass]
    public class UnitTestAdminController
    {
        [TestMethod]
        public void Index_Return_ActionResults()
        {
            AdminController controller = new AdminController();
            var result = controller.Index(2) as ViewResult;
            var s = (IndexUserViewModel) result.ViewData.Model;
            Assert.AreEqual("all", s.Role);
        }
    }
}
