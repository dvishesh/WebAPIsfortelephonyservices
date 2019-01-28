Deployment Procedure

Self host application via Kestral:
1. Download and install visual studio code, enable C# extension
2. Download and install SDK 2.1.4 (that installs dot net core 2.0.5) [link: https://dotnet.microsoft.com/download/dotnet-core/2.0]
3. Open the project folder in VS Code, allow C# dependencies to be installed and project libraries to be restored (you might want to restart VS Code and reimport the project folder for Intellisense to be properly updated)
4. In the Terminal window, execute commands "dotnet build" and "dotnet run" to run the application. Program.cs can be configured to determine which port the web server is binded to for receiving http requests.


It is possible to run the application using IIS as well using the .Net Core Hosting Bundle for Windows Server (See https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-2.2)
