using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.API;
using Testcontainers.MsSql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Infrastructure.DAL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace AirAstana.IntegrationTests.TestSetUp;
public class MockWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("SomeRandom!@#$232")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(registeredServices =>
        {
            var dbOrig = registeredServices.SingleOrDefault(x=>x.ServiceType == typeof(AppDbContext));

            if (dbOrig != null)
            {
                registeredServices.Remove(dbOrig);
            }

            registeredServices.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_msSqlContainer.GetConnectionString());
            });
        });
    }
    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
    }

}
