// unit tests covering currency math, file validation, observer blocking, and edge cases
//This File was created with the assistance of AI

using Microsoft.EntityFrameworkCore;
using Moq;
using ST10438307_GLMS.Data;
using ST10438307_GLMS.Models;
using ST10438307_GLMS.Observers;
using ST10438307_GLMS.Services;
using ST10438307_GLMS.Decorators;
using Microsoft.AspNetCore.Components.Forms;

namespace ST10438307_GLMS.Tests;

public class GLMSTests
{
    //-----------------------------------------------------------------------------------------------
    // Currency Math - usd to zar and back
    //-----------------------------------------------------------------------------------------------

    [Fact]
    public void CurrencyConversion_UsdToZar_CalculatesCorrectly()
    {
        var currencyService = new CurrencyService(new HttpClient());

        var result = currencyService.ConvertUsdToZar(100m, 18.50m);

        // 100 usd at 18.50 should equal 1850.00 zar
        Assert.Equal(1850.00m, result);
    }

    [Fact]
    public void CurrencyConversion_ZarToUsd_CalculatesCorrectly()
    {
        var currencyService = new CurrencyService(new HttpClient());

        var result = currencyService.ConvertZarToUsd(1850.00m, 18.50m);

        // 1850 zar at 18.50 should equal 100.00 usd
        Assert.Equal(100.00m, result);
    }

    //-----------------------------------------------------------------------------------------------
    // Currency Edge Cases - zero, negative, rounding
    //-----------------------------------------------------------------------------------------------

    [Fact]
    public void CurrencyConversion_ZeroUsdAmount_ReturnsZero()
    {
        var currencyService = new CurrencyService(new HttpClient());

        var result = currencyService.ConvertUsdToZar(0m, 18.50m);

        // zero input should always return zero regardless of rate
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CurrencyConversion_ZeroRate_ReturnsZero()
    {
        var currencyService = new CurrencyService(new HttpClient());

        // zero rate means api returned nothing - should return zero not crash
        var result = currencyService.ConvertZarToUsd(1850m, 0m);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void CurrencyConversion_NegativeAmount_ReturnsNegative()
    {
        var currencyService = new CurrencyService(new HttpClient());

        var result = currencyService.ConvertUsdToZar(-100m, 18.50m);

        Assert.Equal(-1850.00m, result);
    }

    [Fact]
    public void CurrencyConversion_RoundingPrecision_MaxTwoDecimalPlaces()
    {
        var currencyService = new CurrencyService(new HttpClient());

        // result should never have more than 2 decimal places
        var result = currencyService.ConvertUsdToZar(1m, 18.5678m);

        Assert.Equal(Math.Round(result, 2), result);
    }

    //-----------------------------------------------------------------------------------------------
    // File Validation - decorator rejects non-pdf files
    //-----------------------------------------------------------------------------------------------

    [Fact]
    public async Task FileUpload_NonPdfFile_ThrowsException()
    {
        var mockFile = new Mock<IBrowserFile>();
        mockFile.Setup(f => f.Name).Returns("malware.exe");

        var mockInnerService = new Mock<IFileUploadService>();
        var decorator = new PdfValidationDecorator(mockInnerService.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => decorator.UploadAsync(mockFile.Object)
        );
    }

    [Fact]
    public async Task FileUpload_PdfFile_PassesValidation()
    {
        var mockFile = new Mock<IBrowserFile>();
        mockFile.Setup(f => f.Name).Returns("agreement.pdf");

        var mockInnerService = new Mock<IFileUploadService>();
        mockInnerService
            .Setup(s => s.UploadAsync(It.IsAny<IBrowserFile>()))
            .ReturnsAsync("uploads/agreement.pdf");

        var decorator = new PdfValidationDecorator(mockInnerService.Object);
        var result = await decorator.UploadAsync(mockFile.Object);

        Assert.Equal("uploads/agreement.pdf", result);
    }

    [Fact]
    public async Task FileUpload_ExeFile_ThrowsException()
    {
        var mockFile = new Mock<IBrowserFile>();
        mockFile.Setup(f => f.Name).Returns("virus.exe");

        var mockInnerService = new Mock<IFileUploadService>();
        var decorator = new PdfValidationDecorator(mockInnerService.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => decorator.UploadAsync(mockFile.Object)
        );
    }

    [Fact]
    public async Task FileUpload_JpgFile_ThrowsException()
    {
        var mockFile = new Mock<IBrowserFile>();
        mockFile.Setup(f => f.Name).Returns("photo.jpg");

        var mockInnerService = new Mock<IFileUploadService>();
        var decorator = new PdfValidationDecorator(mockInnerService.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => decorator.UploadAsync(mockFile.Object)
        );
    }

    [Fact]
    public async Task FileUpload_DocxFile_ThrowsException()
    {
        var mockFile = new Mock<IBrowserFile>();
        mockFile.Setup(f => f.Name).Returns("contract.docx");

        var mockInnerService = new Mock<IFileUploadService>();
        var decorator = new PdfValidationDecorator(mockInnerService.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => decorator.UploadAsync(mockFile.Object)
        );
    }

    [Fact]
    public async Task FileUpload_UppercasePdfExtension_PassesValidation()
    {
        // decorator should treat .PDF the same as .pdf
        var mockFile = new Mock<IBrowserFile>();
        mockFile.Setup(f => f.Name).Returns("AGREEMENT.PDF");

        var mockInnerService = new Mock<IFileUploadService>();
        mockInnerService
            .Setup(s => s.UploadAsync(It.IsAny<IBrowserFile>()))
            .ReturnsAsync("uploads/AGREEMENT.PDF");

        var decorator = new PdfValidationDecorator(mockInnerService.Object);
        var result = await decorator.UploadAsync(mockFile.Object);

        Assert.Equal("uploads/AGREEMENT.PDF", result);
    }

    //-----------------------------------------------------------------------------------------------
    // Observer Blocking - expired and on hold contracts block new requests
    //-----------------------------------------------------------------------------------------------

    [Fact]
    public async Task ServiceRequest_ExpiredContract_IsBlocked()
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var client = new Client
        {
            Name = "Test Client",
            ContactDetails = "test@test.com",
            Region = "Test Region"
        };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var contract = new Contract
        {
            ClientId = client.Id,
            ServiceLevel = "Standard",
            Status = ContractStatus.Expired,
            StartDate = DateTime.Today.AddYears(-2),
            EndDate = DateTime.Today.AddYears(-1)
        };
        context.Contracts.Add(contract);
        await context.SaveChangesAsync();

        var validator = new ServiceRequestValidator();
        var auditLogger = new AuditLogger();
        var contextFactory = new TestDbContextFactory(connection);
        var service = new ServiceRequestService(contextFactory, validator, auditLogger);

        var serviceRequest = new ServiceRequest
        {
            ContractId = contract.Id,
            Description = "should be blocked",
            CostZAR = 500m,
            Status = ServiceRequestStatus.Pending
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AddServiceRequestAsync(serviceRequest)
        );
    }

    [Fact]
    public async Task ServiceRequest_OnHoldContract_IsBlocked()
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var client = new Client
        {
            Name = "Test Client",
            ContactDetails = "test@test.com",
            Region = "Test Region"
        };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var contract = new Contract
        {
            ClientId = client.Id,
            ServiceLevel = "Standard",
            Status = ContractStatus.OnHold,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1)
        };
        context.Contracts.Add(contract);
        await context.SaveChangesAsync();

        var validator = new ServiceRequestValidator();
        var auditLogger = new AuditLogger();
        var contextFactory = new TestDbContextFactory(connection);
        var service = new ServiceRequestService(contextFactory, validator, auditLogger);

        var serviceRequest = new ServiceRequest
        {
            ContractId = contract.Id,
            Description = "should be blocked by on hold",
            CostZAR = 500m,
            Status = ServiceRequestStatus.Pending
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AddServiceRequestAsync(serviceRequest)
        );
    }

    [Fact]
    public async Task ServiceRequest_ActiveContract_IsAllowed()
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var client = new Client
        {
            Name = "Test Client",
            ContactDetails = "test@test.com",
            Region = "Test Region"
        };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var contract = new Contract
        {
            ClientId = client.Id,
            ServiceLevel = "Standard",
            Status = ContractStatus.Active,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1)
        };
        context.Contracts.Add(contract);
        await context.SaveChangesAsync();

        var validator = new ServiceRequestValidator();
        var auditLogger = new AuditLogger();
        var contextFactory = new TestDbContextFactory(connection);
        var service = new ServiceRequestService(contextFactory, validator, auditLogger);

        var serviceRequest = new ServiceRequest
        {
            ContractId = contract.Id,
            Description = "should go through",
            CostZAR = 500m,
            Status = ServiceRequestStatus.Pending
        };

        var exception = await Record.ExceptionAsync(
            () => service.AddServiceRequestAsync(serviceRequest)
        );

        Assert.Null(exception);
    }

    [Fact]
    public async Task ServiceRequest_DraftContract_IsAllowed()
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var client = new Client
        {
            Name = "Test Client",
            ContactDetails = "test@test.com",
            Region = "Test Region"
        };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var contract = new Contract
        {
            ClientId = client.Id,
            ServiceLevel = "Standard",
            Status = ContractStatus.Draft,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1)
        };
        context.Contracts.Add(contract);
        await context.SaveChangesAsync();

        var validator = new ServiceRequestValidator();
        var auditLogger = new AuditLogger();
        var contextFactory = new TestDbContextFactory(connection);
        var service = new ServiceRequestService(contextFactory, validator, auditLogger);

        var serviceRequest = new ServiceRequest
        {
            ContractId = contract.Id,
            Description = "request on draft contract",
            CostZAR = 250m,
            Status = ServiceRequestStatus.Pending
        };

        var exception = await Record.ExceptionAsync(
            () => service.AddServiceRequestAsync(serviceRequest)
        );

        Assert.Null(exception);
    }

    [Fact]
    public async Task ServiceRequest_ZeroCost_IsAllowedOnActiveContract()
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var client = new Client
        {
            Name = "Test Client",
            ContactDetails = "test@test.com",
            Region = "Test Region"
        };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var contract = new Contract
        {
            ClientId = client.Id,
            ServiceLevel = "Standard",
            Status = ContractStatus.Active,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1)
        };
        context.Contracts.Add(contract);
        await context.SaveChangesAsync();

        var validator = new ServiceRequestValidator();
        var auditLogger = new AuditLogger();
        var contextFactory = new TestDbContextFactory(connection);
        var service = new ServiceRequestService(contextFactory, validator, auditLogger);

        // zero cost - observer only checks contract status not cost
        var serviceRequest = new ServiceRequest
        {
            ContractId = contract.Id,
            Description = "zero cost request",
            CostZAR = 0m,
            Status = ServiceRequestStatus.Pending
        };

        var exception = await Record.ExceptionAsync(
            () => service.AddServiceRequestAsync(serviceRequest)
        );

        Assert.Null(exception);
    }

    //-----------------------------------------------------------------------------------------------
    // TDD Tests - these tests define behaviour the service should enforce
    // they are written first to prove the gap exists, then the service is fixed to pass them
    //-----------------------------------------------------------------------------------------------

    [Fact]
    public async Task ServiceRequest_EmptyDescription_ThrowsException()
    {
        // the service layer should reject blank descriptions
        // not just the ui - this is a data integrity rule
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var client = new Client
        {
            Name = "Test Client",
            ContactDetails = "test@test.com",
            Region = "Test Region"
        };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var contract = new Contract
        {
            ClientId = client.Id,
            ServiceLevel = "Standard",
            Status = ContractStatus.Active,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1)
        };
        context.Contracts.Add(contract);
        await context.SaveChangesAsync();

        var validator = new ServiceRequestValidator();
        var auditLogger = new AuditLogger();
        var contextFactory = new TestDbContextFactory(connection);
        var service = new ServiceRequestService(contextFactory, validator, auditLogger);

        var serviceRequest = new ServiceRequest
        {
            ContractId = contract.Id,
            Description = "", // empty description - should be rejected at service layer
            CostZAR = 500m,
            Status = ServiceRequestStatus.Pending
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AddServiceRequestAsync(serviceRequest)
        );
    }

    [Fact]
    public async Task ServiceRequest_NegativeCost_ThrowsException()
    {
        // negative cost makes no business sense - service should reject it
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var client = new Client
        {
            Name = "Test Client",
            ContactDetails = "test@test.com",
            Region = "Test Region"
        };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var contract = new Contract
        {
            ClientId = client.Id,
            ServiceLevel = "Standard",
            Status = ContractStatus.Active,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1)
        };
        context.Contracts.Add(contract);
        await context.SaveChangesAsync();

        var validator = new ServiceRequestValidator();
        var auditLogger = new AuditLogger();
        var contextFactory = new TestDbContextFactory(connection);
        var service = new ServiceRequestService(contextFactory, validator, auditLogger);

        var serviceRequest = new ServiceRequest
        {
            ContractId = contract.Id,
            Description = "negative cost request",
            CostZAR = -100m, // negative cost - should be rejected at service layer
            Status = ServiceRequestStatus.Pending
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AddServiceRequestAsync(serviceRequest)
        );
    }
}

//-----------------------------------------------------------------------------------------------
// Test Helper - gives the service a real dbcontextfactory backed by the test connection
//-----------------------------------------------------------------------------------------------

public class TestDbContextFactory : IDbContextFactory<AppDbContext>
{
    private readonly Microsoft.Data.Sqlite.SqliteConnection _connection;

    public TestDbContextFactory(Microsoft.Data.Sqlite.SqliteConnection connection)
    {
        _connection = connection;
    }

    public AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        return new AppDbContext(options);
    }

    public Task<AppDbContext> CreateDbContextAsync()
    {
        return Task.FromResult(CreateDbContext());
    }
}