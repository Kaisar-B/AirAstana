using Domain.RepositoryAbstraction;
using Infrastructure.DAL;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAstana.IntegrationTests.TestSetUp;
public abstract class BaseIntegrationTest : IClassFixture<MockWebAppFactory>, IDisposable
{
    private readonly IServiceScope _serviceScope;
    protected readonly AppDbContext _databaseContext;
    public BaseIntegrationTest(MockWebAppFactory factory)
    {
        _serviceScope = factory.Services.CreateScope();

        _databaseContext = _serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}
