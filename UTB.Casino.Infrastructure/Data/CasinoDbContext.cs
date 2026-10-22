using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UTB.Casino.Domain.Entities;

namespace UTB.Casino.Infrastructure.Data;

public class CasinoDbContext : IdentityDbContext<ApplicationUser>
{
    public CasinoDbContext(DbContextOptions<CasinoDbContext> options) : base(options) { }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Bet> Bets => Set<Bet>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<ApplicationUser>().Property(u => u.DisplayName).HasMaxLength(60);

        b.Entity<Game>(e =>
        {
            e.Property(g => g.Name).IsRequired().HasMaxLength(60);
            e.Property(g => g.Slug).IsRequired().HasMaxLength(30);
            e.Property(g => g.Description).HasMaxLength(200);
            e.HasIndex(g => g.Slug).IsUnique();
        });

        b.Entity<Bet>(e =>
        {
            e.Property(x => x.Result).IsRequired().HasMaxLength(100);
            e.HasOne(x => x.User).WithMany(u => u.Bets).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Game).WithMany(g => g.Bets).HasForeignKey(x => x.GameId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<WalletTransaction>(e =>
        {
            e.Property(t => t.Note).IsRequired().HasMaxLength(200);
            e.Property(t => t.Type).HasConversion<string>().HasMaxLength(20);
            e.HasOne(t => t.User).WithMany(u => u.Transactions).HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(t => t.Bet).WithMany(x => x.Transactions).HasForeignKey(t => t.BetId).OnDelete(DeleteBehavior.SetNull);
        });
    }
}
