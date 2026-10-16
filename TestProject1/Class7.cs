
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using POE_RaceDay_API.Controllers;
using POE_RaceDay_API.DTOs;
using Xunit;

namespace POE_RaceDay_API.Tests
{
    public class EnrolmentControllerTests
    {
        private static IConfiguration CreateConfiguration(
            string? connectionString = null)
        {
            var settings = new Dictionary<string, string?>();

            if (connectionString != null)
            {
                settings["ConnectionStrings:RaceDayConnection"] =
                    connectionString;
            }

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        private static EnrolmentController CreateController(
            string? userId = null,
            string? connectionString = null)
        {
            var controller = new EnrolmentController(
                CreateConfiguration(connectionString));

            var claims = new List<Claim>();

            if (userId != null)
            {
                claims.Add(
                    new Claim(ClaimTypes.NameIdentifier, userId));
            }

            var identity = new ClaimsIdentity(
                claims, "TestAuthentication");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };

            return controller;
        }

        [Fact]
        public async Task CreateEnrolment_InvalidCategoryID_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateController(userId: "1");

            var request = new EnrolmentRequest
            {
                CategoryID = 0
            };

            // Act
            var result = await controller.CreateEnrolment(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateEnrolment_NegativeCategoryID_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateController(userId: "1");

            var request = new EnrolmentRequest
            {
                CategoryID = -1
            };

            // Act
            var result = await controller.CreateEnrolment(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateEnrolment_MissingParticipantID_ReturnsUnauthorized()
        {
            // Arrange
            var controller = CreateController();

            var request = new EnrolmentRequest
            {
                CategoryID = 1
            };

            // Act
            var result = await controller.CreateEnrolment(request);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task CreateEnrolment_InvalidParticipantID_ReturnsUnauthorized()
        {
            // Arrange
            var controller = CreateController(userId: "invalid");

            var request = new EnrolmentRequest
            {
                CategoryID = 1
            };

            // Act
            var result = await controller.CreateEnrolment(request);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task CreateEnrolment_NonNumericParticipantID_ReturnsUnauthorized()
        {
            // Arrange
            var controller = CreateController(userId: "abc");

            var request = new EnrolmentRequest
            {
                CategoryID = 1
            };

            // Act
            var result = await controller.CreateEnrolment(request);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task GetEnrolments_ReturnsOk_WhenDatabaseIsAvailable()
        {
            // This test requires a working test database.
            // Do not run it without configuring a valid connection string.

            var controller = CreateController(
                connectionString: "YOUR_TEST_DATABASE_CONNECTION_STRING");

            var result = await controller.GetEnrolments();

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
