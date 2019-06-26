# Digital Nomad Dave ASP.NET Core v2
[![Build status](https://davidikin.visualstudio.com/Digital%20Nomad%20Dave/_apis/build/status/Digital%20Nomad%20Dave-ASP.NET%20Core-CI)](https://davidikin.visualstudio.com/Digital%20Nomad%20Dave/_build/latest?definitionId=5)

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
.NET Core 2.2
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
Execute batch\test\UnitTests.bat
OR
dotnet test test\DND.UnitTests\DND.UnitTests.csproj
```

### DND.IntegrationTests (WebApplicationFactory/TestServer)

Automatically creates an Integration database on Local\MSSQLLOCALDB, seeds and runs an in process TestServer. Domain Events fired.

```
Execute batch\test\IntegrationTests.bat
OR
dotnet test test\DND.IntegrationTests\DND.IntegrationTests.csproj
```

### DND.UITests (SpecFlow & Selenium)

Automatically creates a Integration database on Local\MSSQLLOCALDB, seeds and launches a Kestral Web Host using dotnet run. On completion database is deleted. Domain Events fired.

```
Set SeleniumUrl in test\DND.UITests\app.config
```
```
Execute batch\test\UITests.bat 
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
* [.NET Core WebApplicationFactory/TestServer](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.2) - InProcess Integration Tests
* [Stackify Prefix](https://stackify.com/prefix/) - Runtime performance profiler for .NET Core
* [MiniProfiler](https://miniprofiler.com/dotnet/AspDotNetCore) - Runtime performance profiler for .NET Core
* [Visual Studio Team Services](https://www.visualstudio.com/team-services/) - Continuous Integration (CI) and Continuous Deployment (CD)
* [Identity Server 4](http://docs.identityserver.io/en/release/) - .NET Core Identity Provider
* [NCrunch](https://www.ncrunch.net/) - Concurrent testing tool for Visual Studio
* [RobotsTxtMiddleware](https://github.com/karl-sjogren/robots-txt-middleware/blob/master/README.md)
* [Conveyor](https://marketplace.visualstudio.com/items?itemName=vs-publisher-1448185.ConveyorbyKeyoti)

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