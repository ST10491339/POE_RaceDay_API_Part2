# POE_RaceDay_API

1. Project Overview

POE_RaceDay_API is an ASP.NET Core Web API project developed using C#. The API provides a backend for managing race day-related information and processes.

The project includes API controllers to handle requests and responses, as well as JWT (JSON Web Token) authentication to help secure access to protected resources.

2. Technologies Used

C#

ASP.NET Core Web API

.NET

JWT Authentication

RESTful API principles

Unit Testing

GitHub Actions for CI/CD

3. Project Features

The project is designed to support the following features:

Processing HTTP requests through API controllers.

Managing race day-related data.

Validating incoming requests.

Securing protected endpoints using JWT authentication.

Handling errors and returning appropriate HTTP responses.

Testing controller functionality through unit tests.

Automating build and test processes using GitHub Actions.

4. Prerequisites

Before running the project, ensure that you have installed:

.NET SDK compatible with the project.

Visual Studio, Visual Studio Code, or another compatible C# IDE.

Git.

A GitHub account for repository hosting and CI/CD.

5. Installation and Setup

Follow these steps to set up the project locally.

Step 1: Clone the repository

Open a terminal and run:

git clone <(https://github.com/ST10491339/POE_RaceDay_API_Part2)>

Navigate to the project directory:

cd POE_RaceDay_API

Replace the repository URL with your actual GitHub repository URL.

Step 2: Restore dependencies

Run the following command from the directory containing the API project file:

dotnet restore
