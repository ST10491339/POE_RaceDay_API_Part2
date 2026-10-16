
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using POE_RaceDay_API.Controllers;
using Xunit;

namespace POE_RaceDay_API.Tests
{
    public class ProfileControllerTests
    {
        private readonly IConfiguration _configuration;

        public ProfileControllerTests()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        {
                            "ConnectionStrings:RaceDayConnection",
                            "Server=localhost;Database=RaceDayTest;" +
                            "Trusted_Connection=True;" +
                            "TrustServerCertificate=True;"
                        }
                    })
                .Build();
        }

        private ProfileController CreateController(
            string? userId = null)
        {
            var claims = new List<Claim>();

            if (userId != null)
            {
                claims.Add(new Claim(
                    ClaimTypes.NameIdentifier, userId));
            }

            var identity = new ClaimsIdentity(
                claims, "TestAuthentication");

            var controller = new ProfileController(_configuration);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };

            return controller;
        }

        // TEST 1: Missing user ID claim
        [Fact]
        public void GetProfile_MissingUserID_ReturnsUnauthorized()
        {
            var controller = CreateController();

            var result = controller.GetProfile();

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // TEST 2: Invalid user ID claim
        [Fact]
        public void GetProfile_InvalidUserID_ReturnsUnauthorized()
        {
            var controller = CreateController("invalid");

            var result = controller.GetProfile();

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // TEST 3: Non-numeric user ID claim
        [Fact]
        public void GetProfile_NonNumericUserID_ReturnsUnauthorized()
        {
            var controller = CreateController("abc123");

            var result = controller.GetProfile();

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // TEST 4: Empty user ID claim
        [Fact]
        public void GetProfile_EmptyUserID_ReturnsUnauthorized()
        {
            var controller = CreateController("");

            var result = controller.GetProfile();

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // TEST 5: Missing connection string
        [Fact]
        public void Constructor_MissingConnectionString_ThrowsException()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>())
                .Build();

            Assert.Throws<InvalidOperationException>(
                () => new ProfileController(configuration));
        }
    }
}
