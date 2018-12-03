using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Web.Controllers;

namespace SFA.DAS.Reservations.UnitTests.Controllers.Home
{
    public class WhenRenderingTheHomeController
    {
        private HomeController _homeController;

        [SetUp]
        public void Arrange()
        {
            _homeController = new HomeController();
        }

        [Test]
        public async Task Then_The_Correct_View_Is_Displayed()
        {
            //Act
            var actual = await _homeController.Index();

            //Assert
            Assert.IsNotNull(actual);
            var result = actual as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }
    }
}
