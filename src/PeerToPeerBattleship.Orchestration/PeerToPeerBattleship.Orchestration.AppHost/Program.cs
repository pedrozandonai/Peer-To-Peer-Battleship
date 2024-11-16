var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject("backend", @"..\..\PeerToPeerBattleship.ConsoleApp\PeerToPeerBattleship.ConsoleApp.csproj");
builder.AddProject("desktop", @"..\..\PeerToPeerBattleship.Desktop\PeerToPeerBattleship.Desktop.csproj");

builder.Build().Run();
