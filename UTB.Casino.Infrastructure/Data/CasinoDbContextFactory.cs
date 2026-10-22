using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UTB.Casino.Infrastructure.Data;

/// <summary>Used only by "dotnet ef" so migrations never need to start the web app.</summary>
public class CasinoDbContextFactory : IDesignTimeDbContextFactory<CasinoDbContext>
{
    public CasinoDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<CasinoDbContext>().UseSqlite("Data Source=casino.db").Options);
}
