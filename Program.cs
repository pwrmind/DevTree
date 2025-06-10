using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace DevTree;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("DevTree - Worktree + DevContainer manager");

        // Создание новой ветки
        var createCmd = new Command("create", "Create new branch with isolated environment");
        var branchOption = new Option<string>("--branch", "Branch name") { IsRequired = true };
        createCmd.AddOption(branchOption);
        createCmd.SetHandler((branch) => CreateBranch(branch), branchOption);

        // Переключение между ветками
        var switchCmd = new Command("switch", "Switch to branch environment");
        switchCmd.AddOption(branchOption);
        switchCmd.SetHandler((branch) => SwitchBranch(branch), branchOption);

        // Удаление ветки
        var removeCmd = new Command("remove", "Remove branch environment");
        removeCmd.AddOption(branchOption);
        removeCmd.SetHandler((branch) => RemoveBranch(branch), branchOption);

        rootCommand.AddCommand(createCmd);
        rootCommand.AddCommand(switchCmd);
        rootCommand.AddCommand(removeCmd);

        return await rootCommand.InvokeAsync(args);
    }

    static void CreateBranch(string branch)
    {
        var state = StateManager.LoadState();
        var port = PortManager.AcquirePort();

        WorktreeManager.CreateWorktree(branch);
        DevContainerManager.GenerateConfig(branch, port);

        state.BranchPorts[branch] = port;
        state.SaveState();

        Console.WriteLine($"Created branch '{branch}' with port {port}");
    }

    static void SwitchBranch(string branch)
    {
        var state = StateManager.LoadState();

        if (state.CurrentBranch != null)
        {
            DevContainerManager.StopContainer(state.CurrentBranch);
        }

        DevContainerManager.StartContainer(branch);
        state.CurrentBranch = branch;
        state.SaveState();

        Console.WriteLine($"Switched to branch '{branch}'");
        Console.WriteLine($"Open in VS Code: code {WorktreeManager.GetWorktreePath(branch)}");
    }

    static void RemoveBranch(string branch)
    {
        var state = StateManager.LoadState();

        DevContainerManager.StopContainer(branch);
        WorktreeManager.RemoveWorktree(branch);

        if (state.BranchPorts.TryGetValue(branch, out var port))
        {
            PortManager.ReleasePort(port);
            state.BranchPorts.Remove(branch);
        }

        state.SaveState();
        Console.WriteLine($"Removed branch '{branch}'");
    }
}