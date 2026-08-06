using System.Data;
using VoyLlegando.Infrastructure.Database;

namespace VoyLlegando.Infrastructure.Repositories;

public abstract class RepositoryBase
{
    private readonly DbConnectionFactory _factory;

    protected RepositoryBase(DbConnectionFactory factory)
    {
        _factory = factory;
    }

protected IDbConnection Connection
    => _factory.CreateConnection();
}

