using System.Collections.Generic;
using System.Linq;

namespace DevTree;

static class PortManager
{
    private static readonly HashSet<int> _allocatedPorts = new();
    private const int StartPort = 3000;
    private const int EndPort = 4000;

    public static int AcquirePort()
    {
        for (int port = StartPort; port <= EndPort; port++)
        {
            if (!_allocatedPorts.Contains(port))
            {
                _allocatedPorts.Add(port);
                return port;
            }
        }
        throw new Exception("No available ports");
    }

    public static void ReleasePort(int port)
    {
        _allocatedPorts.Remove(port);
    }
}