
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
    public class EventControllerTests
    {
        private readonly IConfiguration _configuration;

        public EventControllerTests()
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

        private EventController CreateController(
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

            var controller = new EventController(_configuration);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };

            return controller;
        }

        private EventRequest CreateValidRequest()
        {
            return new EventRequest
            {
                EventName = "Race Day",
                Description = "Annual racing event",
                EventDate = DateTime.UtcNow.AddDays(7),
                Location = "Johannesburg"
            };
        }

        // TEST 1: Create event with a missing event name
        [Fact]
        public async Task CreateEvent_MissingEventName_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.EventName = "";

            var result = await controller.CreateEvent(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 2: Create event with a missing location
        [Fact]
        public async Task CreateEvent_MissingLocation_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.Location = "";

            var result = await controller.CreateEvent(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 3: Create event with a past date
        [Fact]
        public async Task CreateEvent_PastDate_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.EventDate = DateTime.UtcNow.AddDays(-1);

            var result = await controller.CreateEvent(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 4: Create event without a valid organiser ID
        [Fact]
        public async Task CreateEvent_InvalidOrganiserId_ReturnsUnauthorized()
        {
            var controller = CreateController("invalid");
            var request = CreateValidRequest();

            var result = await controller.CreateEvent(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // TEST 5: Update event with a missing event name
        [Fact]
        public async Task UpdateEvent_MissingEventName_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.EventName = "";

            var result = await controller.UpdateEvent(1, request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 6: Update event with a missing location
        [Fact]
        public async Task UpdateEvent_MissingLocation_ReturnsBadRequest()
        {
            var controller = CreateController("1");
            var request = CreateValidRequest();
            request.Location = "";

            var result = await controller.UpdateEvent(1, request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 7: Update event without a valid organiser ID
        [Fact]
        public async Task UpdateEvent_InvalidOrganiserId_ReturnsUnauthorized()
        {
            var controller = CreateController("invalid");
            var request = CreateValidRequest();

            var result = await controller.UpdateEvent(1, request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // TEST 8: Delete event without a valid organiser ID
        [Fact]
        public async Task DeleteEvent_InvalidOrganiserId_ReturnsUnauthorized()
        {
            var controller = CreateController("invalid");

            var result = await controller.DeleteEvent(1);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        // TEST 9: Create event using the "sub" claim
        [Fact]
        public async Task CreateEvent_ValidSubClaim_ContinuesToDatabase()
        {
            var controller = CreateController(subject: "1");
            var request = CreateValidRequest();

            // A valid claim passes validation and attempts
            // to connect to the database.
            await Assert.ThrowsAnyAsync<Exception>(
                async () => await controller.CreateEvent(request));
        }

        // TEST 10: Delete event without any organiser claim
        [Fact]
        public async Task DeleteEvent_MissingOrganiserClaim_ReturnsUnauthorized()
        {
            var controller = CreateController();

            var result = await controller.DeleteEvent(1);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
