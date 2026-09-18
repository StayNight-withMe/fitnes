using fitnes.Domain.Entities;
using fitnes.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fitnes.Infrastructure.Persistence.Configurations;

public static class UserConfiguration
{
    public static void ConfigureUser(this EntityTypeBuilder<User> builder)
    {
        builder.ToTable(ConfigurationConstants.UserTableName);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.Language)
            .IsRequired()
            .HasConversion<string>();
    }
}
