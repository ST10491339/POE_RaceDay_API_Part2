
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

namespace POE_RaceDay_API.Tests
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

        // TEST 1: Invalid enrolment ID
        [Fact]
        public async Task CreateResult_InvalidEnrolmentID_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.EnrolmentID = 0;

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 2: Negative enrolment ID
        [Fact]
        public async Task CreateResult_NegativeEnrolmentID_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.EnrolmentID = -1;

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 3: Invalid position
        [Fact]
        public async Task CreateResult_ZeroPosition_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.Position = 0;

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 4: Negative position
        [Fact]
        public async Task CreateResult_NegativePosition_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.Position = -1;

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 5: Invalid finish time
        [Fact]
        public async Task CreateResult_InvalidFinishTime_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.FinishTime = "invalid";

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 6: Negative finish time
        [Fact]
        public async Task CreateResult_NegativeFinishTime_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.FinishTime = "-00:10:00";

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 7: Finish time greater than or equal to 24 hours
        [Fact]
        public async Task CreateResult_FinishTimeOver24Hours_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.FinishTime = "24:00:00";

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 8: Missing organiser ID
        [Fact]
        public async Task CreateResult_MissingOrganiserID_ReturnsUnauthorized()
        {
            var controller = CreateController();
            var request = CreateValidRequest();

            var result = await controller.CreateResult(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // TEST 9: Invalid organiser ID
        [Fact]
        public async Task CreateResult_InvalidOrganiserID_ReturnsUnauthorized()
        {
            var controller = CreateController("invalid");
            var request = CreateValidRequest();

            var result = await controller.CreateResult(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // TEST 10: Use the "sub" claim as the organiser ID
        [Fact]
        public async Task CreateResult_SubClaimWithInvalidTime_ReturnsBadRequest()
        {
            var controller = CreateController(subject: "1");
            var request = CreateValidRequest();
            request.FinishTime = "invalid";

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 11: Blank finish time
        [Fact]
        public async Task CreateResult_BlankFinishTime_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.FinishTime = "";

            var result = await controller.CreateResult(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
