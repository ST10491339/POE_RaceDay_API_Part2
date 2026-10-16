
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using POE_RaceDay_API.Controllers;
using POE_RaceDay_API.DTOs;
using Xunit;

namespace POE_RaceDay_API.Tests
{
    public class AuthControllerTests
    {
        private readonly IConfiguration _configuration;

        public AuthControllerTests()
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
                        },
                        {
                            "Jwt:Key",
                            "ThisIsATestJwtKeyWithAtLeast32Bytes!"
                        },
                        {
                            "Jwt:Issuer",
                            "RaceDayTest"
                        },
                        {
                            "Jwt:Audience",
                            "RaceDayTestUsers"
                        },
                        {
                            "Jwt:ExpiryMinutes",
                            "60"
                        }
                    })
                .Build();
        }

        private AuthController CreateController()
        {
            return new AuthController(_configuration);
        }

        private RegisterUserDto CreateValidRegisterRequest()
        {
            return new RegisterUserDto
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "TestPassword123!",
                Role = "Participant",
                PhoneNumber = "0123456789"
            };
        }

        // TEST 1: Missing full name
        [Fact]
        public void Register_MissingFullName_ReturnsBadRequest()
        {
            var controller = CreateController();
            var request = CreateValidRegisterRequest();
            request.FullName = "";

            var result = controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 2: Missing email
        [Fact]
        public void Register_MissingEmail_ReturnsBadRequest()
        {
            var controller = CreateController();
            var request = CreateValidRegisterRequest();
            request.Email = "";

            var result = controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 3: Missing password
        [Fact]
        public void Register_MissingPassword_ReturnsBadRequest()
        {
            var controller = CreateController();
            var request = CreateValidRegisterRequest();
            request.Password = "";

            var result = controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 4: Whitespace-only full name
        [Fact]
        public void Register_WhitespaceFullName_ReturnsBadRequest()
        {
            var controller = CreateController();
            var request = CreateValidRegisterRequest();
            request.FullName = "   ";

            var result = controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 5: Whitespace-only email
        [Fact]
        public void Register_WhitespaceEmail_ReturnsBadRequest()
        {
            var controller = CreateController();
            var request = CreateValidRegisterRequest();
            request.Email = "   ";

            var result = controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 6: Whitespace-only password
        [Fact]
        public void Register_WhitespacePassword_ReturnsBadRequest()
        {
            var controller = CreateController();
            var request = CreateValidRegisterRequest();
            request.Password = "   ";

            var result = controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // TEST 7: Missing connection string throws an exception
        [Fact]
        public void Constructor_MissingConnectionString_ThrowsException()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>())
                .Build();

            Assert.Throws<InvalidOperationException>(
                () => new AuthController(configuration));
        }
    }
}
