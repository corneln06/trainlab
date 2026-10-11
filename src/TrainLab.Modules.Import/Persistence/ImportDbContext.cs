using Microsoft.EntityFrameworkCore;

namespace TrainLab.Modules.Import.Persistence;

public class ImportDbContext(DbContextOptions<ImportDbContext> options) : DbContext(options)
{
    public const string Schema = "import";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ImportDbContext).Assembly);
    }
}