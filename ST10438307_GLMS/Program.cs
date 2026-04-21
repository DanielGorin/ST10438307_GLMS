using Microsoft.EntityFrameworkCore;
using ST10438307_GLMS.Components;
using ST10438307_GLMS.Data;
using ST10438307_GLMS.Decorators;
using ST10438307_GLMS.Factories;
using ST10438307_GLMS.Observers;
using ST10438307_GLMS.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// DbContextFactory for Blazor Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(connectionString));

//CLIENT SERVICE
builder.Services.AddScoped<IClientService, ClientService>();

//CONTRACT SERVICE
builder.Services.AddScoped<IContractService, ContractService>();


// SERVICE REQUEST SERVICE
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();

// ABSTRACT FACTORY PATTERN: Register all three concrete factories

builder.Services.AddScoped<StandardContractFactory>();
builder.Services.AddScoped<SLAContractFactory>();
builder.Services.AddScoped<InternationalContractFactory>();

// OBSERVER PATTERN: Register observers

builder.Services.AddScoped<ServiceRequestValidator>();
builder.Services.AddScoped<AuditLogger>();

// DECORATOR PATTERN: Wrap FileUploadService with PdfValidationDecorator

builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<IFileUploadService>(provider =>
    new PdfValidationDecorator(provider.GetRequiredService<FileUploadService>()));

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Ensure database is created and all migrations applied on startup guarantees the app works on any machine without running Update-Database manually
using (var scope = app.Services.CreateScope())
{
    var dbContextFactory = scope.ServiceProvider
        .GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var dbContext = await dbContextFactory.CreateDbContextAsync();
    await dbContext.Database.MigrateAsync();
}

app.Run();