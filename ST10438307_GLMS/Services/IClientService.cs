// Client service interface - defines CRUD for client operations

using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Services;
public interface IClientService
{
    Task<List<Client>> GetAllClientsAsync();
    Task<Client?> GetClientByIdAsync(int id);
    Task AddClientAsync(Client client);
    Task UpdateClientAsync(Client client);
    Task DeleteClientAsync(int id);
}