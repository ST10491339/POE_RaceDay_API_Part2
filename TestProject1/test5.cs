
using System;
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
    public class CategoryControllerTests
    {
        private readonly IConfiguration _configuration;

        public CategoryControllerTests()
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

        private CategoryController CreateController(
            string? userId = null,
            string? subject = null)
        {
            var claims = new List<Claim>();

            if (userId != null)
            {
                claims.Add(new Claim(
                    ClaimTypes.NameIdentifier, userId));
            }

            if (subject != null)
            {
                claims.Add(new Claim("sub", subject));
            }

            var identity = new ClaimsIdentity(
                claims, "TestAuthentication");

            var controller = new CategoryController(_configuration);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };

            return controller;
        }

        private CategoryRequest CreateValidRequest()
        {
            return new CategoryRequest
            {
                CategoryName = "10 km Race",
                Distance = 10,
                EntryFee = 150,
                EventID = 1
            };
        }

        // TEST 1: Missing category name
        [Fact]
        public async System.Threading.Tasks.Task CreateCategory_MissingName_ReturnsBadRequest()
        {
            var controller = CreateController();
            var request = CreateValidRequest();
            request.CategoryName = "";

            var result = await controller.CreateCategory(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 2: Whitespace category name
        [Fact]
        public async System.Threading.Tasks.Task CreateCategory_WhitespaceName_ReturnsBadRequest()
        {
            var controller = CreateController();
            var request = CreateValidRequest();
            request.CategoryName = "   ";

            var result = await controller.CreateCategory(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 3: Zero distance
        [Fact]
        public async System.Threading.Tasks.Task CreateCategory_ZeroDistance_ReturnsBadRequest()
        {
            var controller = CreateController();
            var request = CreateValidRequest();
            request.Distance = 0;

            var result = await controller.CreateCategory(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 4: Negative distance
        [Fact]
        public async System.Threading.Tasks.Task CreateCategory_NegativeDistance_ReturnsBadRequest()
        {
            var controller = CreateController();
            var request = CreateValidRequest();
            request.Distance = -5;

            var result = await controller.CreateCategory(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 5: Negative entry fee
        [Fact]
        public async System.Threading.Tasks.Task CreateCategory_NegativeEntryFee_ReturnsBadRequest()
        {
            var controller = CreateController();
            var request = CreateValidRequest();
            request.EntryFee = -10;

            var result = await controller.CreateCategory(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 6: Zero event ID
        [Fact]
        public async System.Threading.Tasks.Task CreateCategory_ZeroEventID_ReturnsBadRequest()
        {
            var controller = CreateController();
            var request = CreateValidRequest();
            request.EventID = 0;

            var result = await controller.CreateCategory(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 7: Negative event ID
        [Fact]
        public async System.Threading.Tasks.Task CreateCategory_NegativeEventID_ReturnsBadRequest()
        {
            var controller = CreateController();
            var request = CreateValidRequest();
            request.EventID = -1;

            var result = await controller.CreateCategory(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
