#addin "Cake.Git"
#tool "nuget:?package=GitVersion.CommandLine"

var args = new
{
    Target = Argument("target", "Default"),
    OutputDirectory = Argument("output", "build"),
    RepositoryPath = Argument("repositoryPath", "."),
    Configuration = Argument("configuration", "Release"),
	  AddCommitToDescription = Argument("addCommitToDescription", false),
    Version = /*GitVersion(new GitVersionSettings{UpdateAssemblyInfo = false})*/ "0.1",
    Nuget = new
	{
		Source = Argument("nugetSource", "https://www.nuget.org/api/v2/package"),
		ApiKey = Argument("nugetApiKey", ""),
	},
};

var buildDirectory = MakeAbsolute(Directory(args.OutputDirectory));

Task("Clean").Does(() =>
{
	CleanDirectories(args.OutputDirectory);
	CleanDirectories("./**/bin");
	CleanDirectories("./**/obj");
});


Task("PrintArguments").Does(() =>
{
    Information($"Arguments:");
    Information($"  * Target: {args.Target}");
    Information($"  * AddCommitToDescription: {args.AddCommitToDescription}");
    Information($"  * Configuration: {args.Configuration}");
    Information($"  * Version: {args.Version}");
    Information($"  * RepositoryPath: {args.RepositoryPath}");
    Information($"  * OutputDirectory: {args.OutputDirectory}");
    Information($"  * NugetApiKey: <*>");
});

Task("Build")
	.IsDependentOn("Clean")
    .Does(() =>
{
    var sln = "ObviousAwait.sln";

    NuGetRestore(sln);
    MSBuild(sln, c =>
    {
        c.Configuration = args.Configuration;
        c.MSBuildPlatform = Cake.Common.Tools.MSBuild.MSBuildPlatform.x86;
    });
});

Task("Pack")
	.IsDependentOn("Build")
	.Does(() =>
{
    if(!DirectoryExists(buildDirectory.FullPath))
        CreateDirectory(buildDirectory.FullPath);

    var nuspecFiles = GetFiles("*.nuspec");
    foreach(var nuspec in nuspecFiles)
    {
        var wd = MakeAbsolute(nuspec).GetDirectory();
        var settings = new NuGetPackSettings
        {
            Version = args.Version/*.FullSemVer*/,
            OutputDirectory = buildDirectory.FullPath,
            BasePath = wd,
        };

		if(args.AddCommitToDescription)
		{
			// Extract description from nuspec and concat current commit hash and datetime
			var description = XmlPeek(nuspec, $"/package/metadata/description");
			var lastCommit = GitLogTip(args.RepositoryPath);
			description = $"{description}\n\nCommit : {lastCommit.Sha}, {lastCommit.Author?.When}";
			Information($"Updated package description : {description}");
			settings.Description = description;
		}

        NuGetPack(nuspec, settings);
    }
});

Task("Push")
    .IsDependentOn("PrintArguments")
	.IsDependentOn("Pack")
	.Does(() =>
{
	if(string.IsNullOrEmpty(args.Nuget.ApiKey))
	{
		throw new ArgumentException("Missing 'nugetApiKey' arguments");
	}

    var packages = GetFiles($"**/*.{args.Version}.nupkg");
    NuGetPush(packages, new NuGetPushSettings
    {
		Source = args.Nuget.Source,
		ApiKey = args.Nuget.ApiKey,
	});
});

Task("Default")
    .IsDependentOn("PrintArguments")
    .IsDependentOn("Pack");

RunTarget(args.Target);