# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade
3. Upgrade ContosoUniversity.csproj to .NET 10.0

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

No projects are excluded from this upgrade.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                   | Current Version | New Version | Description                                   |
|:-----------------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Antlr                                          |   3.4.1.9004    |             | Replace with new package Antlr4=4.6.6        |
| Antlr4                                         |                 |  4.6.6      | Replacement for Antlr                         |
| Microsoft.AspNet.Mvc                          |   5.2.9         |             | Functionality included with framework reference |
| Microsoft.AspNet.Razor                        |   3.2.9         |             | Functionality included with framework reference |
| Microsoft.AspNet.Web.Optimization             |   1.1.3         |             | NuGet package is incompatible                 |
| Microsoft.AspNet.WebPages                     |   3.2.9         |             | Functionality included with framework reference |
| Microsoft.Bcl.AsyncInterfaces                 |   1.1.1         | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.Bcl.HashCode                        |   1.1.1         | 6.0.0       | Recommended for .NET 10.0                    |
| Microsoft.CodeDom.Providers.DotNetCompilerPlatform | 2.0.1       |             | Functionality included with framework reference |
| Microsoft.Data.SqlClient                      |   2.1.4         | 6.1.4       | Security vulnerability                        |
| Microsoft.EntityFrameworkCore                 |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.EntityFrameworkCore.Abstractions    |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.EntityFrameworkCore.Analyzers       |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.EntityFrameworkCore.Relational      |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.EntityFrameworkCore.SqlServer       |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.EntityFrameworkCore.Tools           |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.Extensions.Caching.Abstractions     |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.Extensions.Caching.Memory           |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.Extensions.Configuration            |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.Extensions.Configuration.Abstractions | 3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.Extensions.Configuration.Binder     |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.Extensions.DependencyInjection      |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.Extensions.DependencyInjection.Abstractions | 3.1.32 | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.Extensions.Logging                  |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.Extensions.Logging.Abstractions     |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.Extensions.Options                  |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.Extensions.Primitives               |   3.1.32        | 10.0.2      | Recommended for .NET 10.0                    |
| Microsoft.Identity.Client                     |   4.21.1        |             | NuGet package is deprecated                   |
| Microsoft.Web.Infrastructure                  |   2.0.1         |             | Functionality included with framework reference |
| NETStandard.Library                           |   2.0.3         |             | Functionality included with framework reference |
| Newtonsoft.Json                               |   13.0.3        | 13.0.4      | Recommended for .NET 10.0                    |
| System.Buffers                                |   4.5.1         |             | Functionality included with framework reference |
| System.Collections.Immutable                  |   1.7.1         | 10.0.2      | Recommended for .NET 10.0                    |
| System.ComponentModel.Annotations             |   4.7.0         |             | Functionality included with framework reference |
| System.Diagnostics.DiagnosticSource           |   4.7.1         | 10.0.2      | Recommended for .NET 10.0                    |
| System.Memory                                 |   4.5.4         |             | Functionality included with framework reference |
| System.Numerics.Vectors                       |   4.5.0         |             | Functionality included with framework reference |
| System.Runtime.CompilerServices.Unsafe        |   4.5.3         | 6.1.2       | Recommended for .NET 10.0                    |
| System.Threading.Tasks.Extensions             |   4.5.4         |             | Functionality included with framework reference |

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### ContosoUniversity.csproj modifications

Project properties changes:
  - Target framework should be changed from `net48` to `net10.0`
  - Convert to SDK-style project

NuGet packages changes:
  - Microsoft.Data.SqlClient should be updated from `2.1.4` to `6.1.4` (*security vulnerability*)
  - Microsoft.EntityFrameworkCore should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.EntityFrameworkCore.Abstractions should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.EntityFrameworkCore.Analyzers should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.EntityFrameworkCore.Relational should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.EntityFrameworkCore.SqlServer should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.EntityFrameworkCore.Tools should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Caching.Abstractions should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Caching.Memory should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Configuration should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Configuration.Abstractions should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Configuration.Binder should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.DependencyInjection should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Logging should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Logging.Abstractions should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Primitives should be updated from `3.1.32` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.Bcl.AsyncInterfaces should be updated from `1.1.1` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.Bcl.HashCode should be updated from `1.1.1` to `6.0.0` (*recommended for .NET 10.0*)
  - Newtonsoft.Json should be updated from `13.0.3` to `13.0.4` (*recommended for .NET 10.0*)
  - System.Collections.Immutable should be updated from `1.7.1` to `10.0.2` (*recommended for .NET 10.0*)
  - System.Diagnostics.DiagnosticSource should be updated from `4.7.1` to `10.0.2` (*recommended for .NET 10.0*)
  - System.Runtime.CompilerServices.Unsafe should be updated from `4.5.3` to `6.1.2` (*recommended for .NET 10.0*)
  - Antlr should be replaced with Antlr4 version `4.6.6`
  - Remove deprecated Microsoft.Identity.Client package
  - Remove incompatible Microsoft.AspNet.Web.Optimization package

Feature upgrades:
  - System.Web.Optimization bundling and minification upgrade: Replace @Scripts.Render and @Styles.Render with direct HTML tags pointing to content files
  - Routes registration via RouteCollection upgrade: Convert to route mappings on the application object for .NET Core
  - System.Messaging MSMQ upgrade: Convert to modern message queue technology (RabbitMQ, Azure Service Bus, etc.)
  - Global.asax.cs application initialization upgrade: Convert to .NET Core startup pattern and clean up Global.asax.cs

Other changes:
  - Legacy configuration system upgrade: Migrate from XML-based web.config to Microsoft.Extensions.Configuration with JSON/environment variables
  - ASP.NET Framework System.Web APIs upgrade: Migrate to ASP.NET Core equivalents
  - Binary incompatible API changes: 92 APIs require code changes
  - Source incompatible API changes: 29 APIs need re-compilation fixes