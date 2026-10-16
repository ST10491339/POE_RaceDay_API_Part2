using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using POE_RaceDay_API.Controllers;
using POE_RaceDay_API.DTOs;
using Xunit;

namespace TestProject1
{
    public class ResultControllerTests
    {
        private readonly IConfiguration _configuration;

        public ResultControllerTests()
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

        private ResultController CreateController(
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

            var controller = new ResultController(_configuration);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };

            return controller;
        }

        private ResultRequest CreateValidRequest()
        {
            return new ResultRequest
            {
                EnrolmentID = 1,
                FinishTime = "00:28:35",
                Position = 1
            };
        }

        [Fact]
        public async Task CreateResult_InvalidEnrolmentID_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.EnrolmentID = 0;

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateResult_NegativeEnrolmentID_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.EnrolmentID = -1;

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateResult_ZeroPosition_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.Position = 0;

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateResult_NegativePosition_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.Position = -1;

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateResult_InvalidFinishTime_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.FinishTime = "invalid";

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateResult_BlankFinishTime_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.FinishTime = "";

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateResult_FinishTimeOf24Hours_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.FinishTime = "24:00:00";

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateResult_NegativeFinishTime_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.FinishTime = "-00:10:00";

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateResult_MissingOrganiserID_ReturnsUnauthorized()
        {
            var controller = CreateController();
            var request = CreateValidRequest();

            var result = await controller.CreateResult(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task CreateResult_InvalidOrganiserID_ReturnsUnauthorized()
        {
            var controller = CreateController("invalid");
            var request = CreateValidRequest();

            var result = await controller.CreateResult(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task CreateResult_SubClaimWithInvalidTime_ReturnsBadRequest()
        {
            var controller = CreateController(subject: "1");
            var request = CreateValidRequest();
            request.FinishTime = "invalid";

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}