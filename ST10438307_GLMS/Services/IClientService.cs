using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Services;

// Blazor components depend on this interface, not the concrete class
public interface IClientService
{
    Task<List<Client>> GetAllClientsAsync();
    Task<Client?> GetClientByIdAsync(int id);
    Task AddClientAsync(Client client);
    Task UpdateClientAsync(Client client);
    Task DeleteClientAsync(int id);
}