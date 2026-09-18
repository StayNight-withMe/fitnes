using fitnes.Domain.Entities;
using fitnes.Infrastructure.Persistence.Context;
using fitnes.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace fitnes.Infrastructure.Persistence.Repositories.Target;

public class UserPgRepository : PgBaseRepository<User, long>
{
    private readonly DbSet<User> _userSet;
    private readonly ApplicationDbContext _dbContext;
    public UserPgRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
    {
        _userSet = applicationDbContext.Users;
        _dbContext = applicationDbContext;
    }

    public async override Task<User> Create(User entity, CancellationToken cancellationToken)
    {
        var existingTracked = _userSet.Local.FirstOrDefault(u => u.Id == entity.Id);

        if (existingTracked is not null)
        {
            return existingTracked; 
        }

        await _userSet.AddAsync(entity, cancellationToken);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            
        }
        finally
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
        }

        return entity;
    }
}
