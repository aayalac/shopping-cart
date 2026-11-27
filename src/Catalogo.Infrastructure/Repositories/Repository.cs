using System.Drawing;
using Catalogo.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Infrastructure.Repositories;

internal abstract class Repository<T> where T : Entity
{
    
    protected readonly CatalogoDbContext dbContext;

    protected Repository(CatalogoDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Set<T>()
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Add(T entity)
    {
        dbContext.Add(entity);
    }


}