

using Microsoft.EntityFrameworkCore;
using ST10438307_GLMS.Components;
using ST10438307_GLMS.Data;
using ST10438307_GLMS.Decorators;
using ST10438307_GLMS.Factories;
using ST10438307_GLMS.Observers;
using ST10438307_GLMS.Services;

var builder = WebApplication.CreateBuilder(args);

//Service Registration
//-----------------------------------------------------------------------------------------------

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//database
//-------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(connectionString));
//-------------------------------------------------------

//Client Service
//-------------------------------------------------------
builder.Services.AddScoped<IClientService, ClientService>();
//-------------------------------------------------------

//Contract Service
//-------------------------------------------------------
builder.Services.AddScoped<IContractService, ContractService>();
//-------------------------------------------------------

// Service Request Service
//-------------------------------------------------------
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
//-------------------------------------------------------

// Abstract Factory Pattern - one registration per concrete factory
//-------------------------------------------------------
builder.Services.AddScoped<StandardContractFactory>();
builder.Services.AddScoped<SLAContractFactory>();
builder.Services.AddScoped<InternationalContractFactory>();
//-------------------------------------------------------

// Observer Pattern
//-------------------------------------------------------
builder.Services.AddScoped<ServiceRequestValidator>();
builder.Services.AddScoped<AuditLogger>();
//-------------------------------------------------------

//Decorator Pattern - wraps FileUploadService with pdf validation
//-------------------------------------------------------
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<IFileUploadService>(provider =>
    new PdfValidationDecorator(
        provider.GetRequiredService<FileUploadService>()
    )
);
//-------------------------------------------------------

//-----------------------------------------------------------------------------------------------

var app = builder.Build();

//Request Pipeline
//-----------------------------------------------------------------------------------------------

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

//-----------------------------------------------------------------------------------------------

//Database Startup applies pending migrations
//-----------------------------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var dbContextFactory = scope.ServiceProvider
        .GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var dbContext = await dbContextFactory.CreateDbContextAsync();
    await dbContext.Database.MigrateAsync(); // creates db if it not exist
}
//-----------------------------------------------------------------------------------------------

app.Run();