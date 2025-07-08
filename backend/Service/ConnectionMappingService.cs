using System.Collections.Concurrent;

namespace SmartContractVehicle.Service;

// This service will be registered as a Singleton to maintain state.
public class ConnectionMappingService
{
    // Using ConcurrentDictionary for thread-safe access.
    private readonly ConcurrentDictionary<string, string> _connections = new();

    public int Count => _connections.Count;

    public void Add(string vin, string connectionId)
    {
        _connections.AddOrUpdate(vin, connectionId, (key, oldValue) => connectionId);
    }

    public string? GetConnectionId(string vin)
    {
        _connections.TryGetValue(vin, out var connectionId);
        return connectionId;
    }

    public void Remove(string vin)
    {
        _connections.TryRemove(vin, out _);
    }

    // This method helps find the VIN associated with a connection ID, useful for disconnect events.
    public string? GetVin(string connectionId)
    {
        return _connections
            .Where(p => p.Value == connectionId)
            .Select(p => p.Key)
            .FirstOrDefault();
    }
}
