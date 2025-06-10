using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace DevTree;

static class DevContainerManager
{
    public static void GenerateConfig(string branch, int port)
    {
        var worktreePath = WorktreeManager.GetWorktreePath(branch);
        var configPath = Path.Combine(worktreePath, ".devcontainer", "devcontainer.json");

        var config = new
        {
            name = $"DevTree - {branch}",
            build = new { dockerfile = "../Dockerfile" },
            forwardPorts = new[] { port },
            portsAttributes = new Dictionary<string, object>
            {
                [port.ToString()] = new { label = "web", protocol = "http" }
            },
            runArgs = new[] { $"--label=devtree.branch={branch}" },
            workspaceFolder = "/workspace"
        };

        File.WriteAllText(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
    }

    public static void StartContainer(string branch)
    {
        var worktreePath = WorktreeManager.GetWorktreePath(branch);
        RunDevContainerCommand($"up --workspace-folder {worktreePath}");
    }

    public static void StopContainer(string branch)
    {
        RunDockerCommand($"stop $(docker ps -q --filter label=devtree.branch={branch})");
    }

    private static void RunDevContainerCommand(string args)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "devcontainer",
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };

        process.Start();
        process.WaitForExit();
    }

    private static void RunDockerCommand(string args)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };

        process.Start();
        process.WaitForExit();
    }
}