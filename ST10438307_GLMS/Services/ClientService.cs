using Microsoft.EntityFrameworkCore;
using ST10438307_GLMS.Data;
using ST10438307_GLMS.Models;

namespace ST10438307_GLMS.Services;

//Directly accesses SQLite via DbContextFactory
public class ClientService : IClientService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ClientService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Client>> GetAllClientsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Clients.ToListAsync();
    }

    public async Task<Client?> GetClientByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Clients.FindAsync(id);
    }

    public async Task AddClientAsync(Client client)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Clients.Add(client);
        await context.SaveChangesAsync();
    }

    public async Task UpdateClientAsync(Client client)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Clients.Update(client);
        await context.SaveChangesAsync();
    }

    public async Task DeleteClientAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var client = await context.Clients.FindAsync(id);
        if (client != null)
        {
            context.Clients.Remove(client);
            await context.SaveChangesAsync();
        }
    }
}