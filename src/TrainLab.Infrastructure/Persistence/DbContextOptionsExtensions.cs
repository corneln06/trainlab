using Microsoft.EntityFrameworkCore;

namespace TrainLab.Infrastructure.Persistence;

public static class DbContextOptionsExtensions
{
    public static DbContextOptionsBuilder UseTrainLabPostgres(
        this DbContextOptionsBuilder optionsBuilder, string connectionString,
        string schema)
    {
        return optionsBuilder.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.UseNetTopologySuite();
                npgsql.MigrationsHistoryTable("__ef_migrations_history",
                    schema);
            })
            .UseSnakeCaseNamingConvention();
    }
}