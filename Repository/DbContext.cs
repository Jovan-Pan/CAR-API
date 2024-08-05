using Microsoft.Extensions.Options;
using Services;
using Microsoft.Data.SqlClient;

namespace Repository;

public class DbContext
{
    private readonly IOptions<AppSettings> _config;

    public DbContext(IOptions<AppSettings> config) => _config = config;

    public SqlConnection MDMConnection() => new(_config.Value.MDMConnectionString);
}
