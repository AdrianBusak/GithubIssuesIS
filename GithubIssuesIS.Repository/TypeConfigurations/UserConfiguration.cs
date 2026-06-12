using GitHubIssuesIS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GithubIssuesIS.Repository.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Username)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(user => user.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(user => user.PasswordSalt)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(user => user.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.HasIndex(user => user.Username)
            .IsUnique();
    }
}
