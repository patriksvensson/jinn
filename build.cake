var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

////////////////////////////////////////////////////////////////
// Tasks

Task("Clean")
    .Does(context =>
{
    context.CleanDirectory("./.artifacts");
});

Task("Build")
    .Does(context =>
{
    DotNetFormatStyle("./src/Jinn.slnx", new DotNetFormatSettings
    {
        VerifyNoChanges = true,
    });

    DotNetBuild("./src/Jinn.slnx", new DotNetBuildSettings {
        Configuration = configuration,
        NoIncremental = context.HasArgument("rebuild"),
        MSBuildSettings = new DotNetMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(context => 
{
    DotNetTest("./src/Jinn.slnx", new DotNetTestSettings {
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
    });
});

Task("Package")
    .IsDependentOn("Clean")
    .IsDependentOn("Test")
    .Does(context => 
{
    context.DotNetPack($"./src/Jinn.slnx", new DotNetPackSettings {
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
        OutputDirectory = "./.artifacts",
        MSBuildSettings = new DotNetMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
    });
});

Task("Publish-NuGet")
    .WithCriteria(ctx => BuildSystem.IsRunningOnGitHubActions, "Not running on GitHub Actions")
    .IsDependentOn("Package")
    .Does(context => 
{
    var apiKey = Argument<string>("nuget-key", null);
    if(string.IsNullOrWhiteSpace(apiKey)) {
        throw new CakeException("No NuGet API key was provided.");
    }

    // Publish to GitHub Packages
    foreach(var file in context.GetFiles("./.artifacts/*.nupkg")) 
    {
        context.Information("Publishing {0}...", file.GetFilename().FullPath);
        DotNetNuGetPush(file.FullPath, new DotNetNuGetPushSettings
        {
            Source = "https://api.nuget.org/v3/index.json",
            ApiKey = apiKey,
        });
    }
});

////////////////////////////////////////////////////////////////
// Targets

Task("Publish")
    .IsDependentOn("Publish-NuGet");

Task("Default")
    .IsDependentOn("Package");

////////////////////////////////////////////////////////////////
// Execution

RunTarget(target);