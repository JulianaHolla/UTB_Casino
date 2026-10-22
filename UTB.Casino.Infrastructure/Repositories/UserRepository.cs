using Microsoft.EntityFrameworkCore;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Domain.Entities;
using UTB.Casino.Infrastructure.Data;

namespace UTB.Casino.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CasinoDbContext _context;
    public UserRepository(CasinoDbContext context) => _context = context;

    public Task<ApplicationUser?> GetByIdAsync(string id) => _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    public async Task<IList<ApplicationUser>> GetAllAsync() => await _context.Users.OrderBy(u => u.UserName).ToListAsync();
}
