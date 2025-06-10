using System.Diagnostics;

namespace DevTree;

static class WorktreeManager
{
    public static void CreateWorktree(string branch)
    {
        var worktreePath = GetWorktreePath(branch);
        Directory.CreateDirectory(Path.Combine(worktreePath, ".devcontainer"));

        RunGitCommand($"worktree add {worktreePath} --checkout -b {branch}");
    }

    public static void RemoveWorktree(string branch)
    {
        var worktreePath = GetWorktreePath(branch);
        RunGitCommand($"worktree remove {worktreePath} --force");
        Directory.Delete(worktreePath, recursive: true);
    }

    public static string GetWorktreePath(string branch)
    {
        return Path.Combine(Environment.CurrentDirectory, "worktrees", branch);
    }

    private static void RunGitCommand(string args)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };

        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Git command failed: git {args}");
        }
    }
}