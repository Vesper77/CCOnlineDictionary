using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using OnlineDiary.Controllers;
using OnlineDiary.Models.CRUDViewModels;
using OnlineDiary.Models;

namespace Unittests
{
    [TestClass]
    public class UnitTestAdminController
    {
        [TestMethod]
        public void Index_Return_Page_ActionResults()
        {
            AdminController controller = new AdminController();
            var view = controller.Index(2) as ViewResult;
            var result = (IndexUserViewModel)view.ViewData.Model;
            Assert.AreEqual(2, result.page);
        }

        [TestMethod]
        public void Index_Return_Role_ActionResults()
        {
            AdminController controller = new AdminController();
            var view = controller.Index(2) as ViewResult;
            var result = (IndexUserViewModel)view.ViewData.Model;
            Assert.AreEqual("all", result.Role);
        }

        [TestMethod]
        public void Create_Return_NotNullView_ActionResults()
        {
            AdminController controller = new AdminController();
            var view = controller.Create() as ViewResult;
            var result = (CreateUserViewModel)view.ViewData.Model;
            Assert.IsTrue(null != result);
        }

        [TestMethod]
        public void Return_NotNullUser()
        {
            UserViewModel model = new UserViewModel();
            var result = model.GetUser();
            Assert.IsTrue(null != result);
        }

        [TestMethod]
        public void Return_NotNullLessonsList()
        {
            UserViewModel model = new UserViewModel();
            var result = model.GetAllLessons();
            Assert.IsTrue(null != result);
        }

        [TestMethod]
        public void Return_NotNullParentList()
        {
            UserViewModel model = new UserViewModel();
            var result = model.GetAllParent();
            Assert.IsTrue(null != result);
        }

        [TestMethod]
        public void Return_NotNullClasstList()
        {
            UserViewModel model = new UserViewModel();
            var result = model.GetAllClass();
            Assert.IsTrue(null != result);
        }

        //[TestMethod]
        //public void Return_RoleName_admin()
        //{
        //    UserViewModel model = new UserViewModel();
        //    var result = model.GetRoleNameById("417e4127-ecc2-4bd6-a928-b0776cbf8f91");
        //    Assert.AreEqual("admin", result);
        //}

        //[TestMethod]
        //public void Return_RoleName_teacher()
        //{
        //    UserViewModel model = new UserViewModel();
        //    var result = model.GetRoleNameById("a7ee5136-2a0c-48a5-9883-22a13b2f6c3b");
        //    Assert.AreEqual("teacher", result);
        //}

        //[TestMethod]
        //public void Return_RoleName_parent()
        //{
        //    UserViewModel model = new UserViewModel();
        //    var result = model.GetRoleNameById("f95b261d-eb13-4dff-8f4c-c6467cb371df");
        //    Assert.AreEqual("parent", result);
        //}

        //[TestMethod]
        //public void Return_RoleName_children()
        //{
        //    UserViewModel model = new UserViewModel();
        //    var result = model.GetRoleNameById("6f380bb4-8e9b-46ff-833e-994c4ee5b598");
        //    Assert.AreEqual("children", result);
        //}

        [TestMethod]
        public void Return_NotNull_RoleList_admin()
        {
            IndexUserViewModel model = new IndexUserViewModel();
            var result = model.GetAllUsersByRole("admin");
            Assert.IsTrue(null != result);
        }

        [TestMethod]
        public void Return_NotNull_RoleList_teacher()
        {
            IndexUserViewModel model = new IndexUserViewModel();
            var result = model.GetAllUsersByRole("teacher");
            Assert.IsTrue(null != result);
        }

        [TestMethod]
        public void Return_NotNull_RoleList_children()
        {
            IndexUserViewModel model = new IndexUserViewModel();
            var result = model.GetAllUsersByRole("children");
            Assert.IsTrue(null != result);
        }

        [TestMethod]
        public void Return_NotNull_RoleList_parent()
        {
            IndexUserViewModel model = new IndexUserViewModel();
            var result = model.GetAllUsersByRole("parent");
            Assert.IsTrue(null != result);
        }

        [TestMethod]
        public void Return_NotNull_UsersList()
        {
            IndexUserViewModel model = new IndexUserViewModel();
            var result = model.getAllUsers();
            Assert.IsTrue(null != result);
        }

        [TestMethod]
        public void Return_NotNull_AllPages()
        {
            IndexUserViewModel model = new IndexUserViewModel();
            var result = model.getCountAllPages();
            Assert.IsTrue(0 <= result);
        }

        [TestMethod]
        public void Return_NotNull_TeachersList()
        {
            LessonViewModel model = new LessonViewModel();
            var result = model.GetAllTeachers();
            Assert.IsTrue(null != result);
        }
    }
}
