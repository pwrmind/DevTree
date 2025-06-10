using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class DevTreeState
{
    public string CurrentBranch { get; set; }
    public Dictionary<string, int> BranchPorts { get; set; } = new();
}

static class StateManager
{
    private const string StateFile = ".devtree/state.json";

    public static DevTreeState LoadState()
    {
        if (!File.Exists(StateFile)) 
            return new DevTreeState();
        
        var json = File.ReadAllText(StateFile);
        return JsonSerializer.Deserialize<DevTreeState>(json);
    }

    public static void SaveState(this DevTreeState state)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(StateFile));
        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(StateFile, json);
    }
}