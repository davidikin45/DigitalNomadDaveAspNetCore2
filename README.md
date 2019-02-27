# Digital Nomad Dave ASP.NET Core v2
[![Build status](https://davidikin.visualstudio.com/Digital%20Nomad%20Dave/_apis/build/status/Digital%20Nomad%20Dave-ASP.NET%20Core%20(.NET%20Framework)-CI)](https://davidikin.visualstudio.com/Digital%20Nomad%20Dave/_build/latest?definitionId=1)

A custom built Content Management System using Bootstrap 4, Angular, C# MVC Core, xUnit, SpecFlow, Selenium, Azure Devops - Previously VSTS, JwtTokens, OpenID Connect, Identity Server 4.

## URLs
* [Website](http://www.digitalnomaddave.com)
* [Swagger UI](http://www.digitalnomaddave.com/swagger)
* [GitHub](https://github.com/davidikin45/DigitalNomadDaveAspNetCore2)
* [Azure DevOps](https://davidikin.visualstudio.com/Digital%20Nomad%20Dave)

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

```
.NET Core 2.1
```

### Installing

```
dotnet run --project src\DND.Web\DND.Web.csproj
```
```
Login to /admin with username: admin password: password
```

## Running the tests

All web host processes and database creation/teardown have been automated using xUnit/NUnit test fixtures.

### DND.UnitTests (In Memory DbContext)

No database required. No Domain Events fired.

```
Execute BatchFiles\Test\UnitTests.bat
OR
dotnet test test\DND.UnitTests.Common\DND.UnitTests.Common.csproj
dotnet test test\DND.UnitTests.Blog\DND.UnitTests.Blog.csproj
dotnet test test\DND.UnitTests.CMS\DND.UnitTests.CMS.csproj
dotnet test test\DND.UnitTests.DynamicForms\DND.UnitTests.DynamicForms.csproj
dotnet test test\DND.UnitTests.FlightSearch\DND.UnitTests.FlightSearch.csproj
```

### DND.IntegrationTestsXUnit (TestServer)

Automatically creates an Integration database on Local\MSSQLLOCALDB, seeds and runs an in process TestServer. On completion database is deleted. Domain Events fired.

```
Execute BatchFiles\Test\IntegrationTestsNUnit.bat
OR
dotnet test test\DND.IntegrationTestsXUnit\DND.IntegrationTestsXUnit.csproj
```
### DND.IntegrationTestsNUnit (Mocking)

Automatically creates a Integration database on Local\MSSQLLOCALDB and seeds. On completion database is deleted. No Domain Events fired.

```
Execute BatchFiles\Test\IntegrationTestsNUnit.bat
OR
dotnet test test\DND.IntegrationTestsNUnit\DND.IntegrationTestsNUnit.csproj
```
### DND.UITests (SpecFlow & Selenium)

Automatically creates a Integration database on Local\MSSQLLOCALDB, seeds and launches a Kestral Web Host using dotnet run. On completion database is deleted. Domain Events fired.

```
Set SeleniumUrl in test\DND.UITests\app.config
```
```
Execute BatchFiles\Test\UITests.bat 
OR
dotnet test test\DND.UITests\DND.UITests.csproj
```

## Deployment

```
Publish DND.Web
```

## Architecture
* [My Architecture](docs/Architecture.md)

## DevOps - Continuous Integration (CI) & Continuous Deployment (CD)
* [My DevOps Process](docs/DevOps.md)

## Built With

* [MVC Core](https://www.asp.net/mvc) - Microsoft .NET framework
* [Bootstrap 4](https://v4-alpha.getbootstrap.com/) - responsive HTML, CSS, JS framework
* [Angular](https://angular.io/) - Google JS framework
* [Automapper](http://automapper.org/) - Convention based object-object mapper
* [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - Microsoft ORM framework
* [MongoDB](https://www.mongodb.com) - NoSQL database
* [Serilog](https://serilog.net/) - Structured Logging
* [Seq](https://getseq.net/) - Structured Logging Query Engine
* [Autofac](http://www.autofac.org/) - Dependency injector for .NET
* [Hangfire](https://rometools.github.io/rome/) - Background job processing
* [Swagger](https://swagger.io/) - API testing
* [Http Cache Headers](https://github.com/KevinDockx/HttpCacheHeaders) - ETags
* [WebSurge](http://websurge.west-wind.com/) - Load Testing
* [xUnit](https://xunit.github.io/) - Open Source .NET Testing framework
* [NUnit](http://nunit.org/) - .NET Testing framework
* [Moq](https://github.com/Moq) - .NET mocking framework
* [SpecFlow](http://specflow.org/) - Behaviour Driven Development (BDD) testing framework
* [Selenium](https://www.seleniumhq.org/) - Browser Automation
* [.NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x) - .NET Core Command Line Tools
* [.NET Core TestServer](https://docs.microsoft.com/en-us/aspnet/core/testing/integration-testing?view=aspnetcore-2.0) - InProcess Integration Tests
* [Stackify Prefix](https://stackify.com/prefix/) - Runtime performance profiler for .NET Core
* [Visual Studio Team Services](https://www.visualstudio.com/team-services/) - Continuous Integration (CI) and Continuous Deployment (CD)
* [Identity Server 4](http://docs.identityserver.io/en/release/) - .NET Core Identity Provider
* [NCrunch](https://www.ncrunch.net/) - Concurrent testing tool for Visual Studio

## Authors

* **Dave Ikin** - [davidikin45](https://github.com/davidikin45)

## License

This project is licensed under the MIT License

## Acknowledgments

* [DevOps Assessment](http://devopsassessment.azurewebsites.net/)
* [Pride Parrot](http://www.prideparrot.com)
* [Mehdi El Gueddari](http://mehdi.me/ambient-dbcontext-in-ef6/)
* [Favicon.io](https://favicon.io/)
* [Favic-o-matic](http://www.favicomatic.com/)
* [Privacy Policies](https://privacypolicies.com)
* [Jwt Tokens](https://jwt.io/)
* [OpenID Connect](http://openid.net/connect/)
* [Productivity Power Pack](https://marketplace.visualstudio.com/items?itemName=VisualStudioProductTeam.ProductivityPowerPack2017)

## Useful Learning Resources
* [My Learning Resources](docs/LearningResources.md)