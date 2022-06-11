using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    AbsolutePath WebAngularDirectory=> RootDirectory / "MetaMetricsViewer.Web.Angular";
    
    AbsolutePath WebAngularProject=> WebAngularDirectory / "MetaMetricsViewer.Web.Angular.csproj";
    
    [GitRepository] readonly GitRepository GitRepository;

    AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(WebAngularProject));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(WebAngularProject)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });
    
    Target Publish => _ => _
        .DependsOn(Compile, Clean)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(WebAngularProject)
                .SetConfiguration(Configuration)
                .SetOutput(OutputDirectory)                
                .EnableNoRestore());
        });
    
    
    
    IReadOnlyCollection<Output> SSHCmd(string cmd)
    {
        return SSH($"{this.DeployHost} -p {this.DeployPort} '{cmd}'");
    }
    [PathExecutable(@"ssh")] readonly Tool SSH;
    [PathExecutable(@"scp")] readonly Tool SCP;
    [Parameter()] public readonly string DeployHost = "dev-metakis-srv";
    // [Parameter()] public readonly string DeployUser = @"metait\samoilenko";
    [Parameter()] public readonly string DeployPort = "22";
    [Parameter()] public readonly string ApplicationPool = "MetaMetricsViewer";
    
    [Parameter()] public readonly string DeployTo = @"c:\MetaKIS\HostingMetaMetricsViewer";
    
    Target Deploy => _ => _
        .DependsOn(Publish)
        .Executes(() =>
        {
            SSHCmd(@$"c:\windows\system32\inetsrv\appcmd stop apppool {ApplicationPool} | exit 0");
            
            SCP($"-r -P {this.DeployPort} {OutputDirectory.ToString().Replace("\\", "/").TrimEnd('/')}/* {this.DeployHost}:{this.DeployTo.Replace("\\", "/")}");
            
            SSHCmd(@$"c:\windows\system32\inetsrv\appcmd start apppool {ApplicationPool}");
        });

}
