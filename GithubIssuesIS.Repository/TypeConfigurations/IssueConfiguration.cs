using GitHubIssuesIS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GithubIssuesIS.Repository.Configurations;

public class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.ToTable("Issues");

        builder.HasKey(issue => issue.Id);

        builder.Property(issue => issue.Number)
            .IsRequired();

        builder.Property(issue => issue.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(issue => issue.Body)
            .HasMaxLength(4000);

        builder.Property(issue => issue.State)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(issue => issue.AuthorLogin)
            .HasMaxLength(150);

        builder.Property(issue => issue.HtmlUrl)
            .HasMaxLength(500);

        builder.Property(issue => issue.CreatedAt)
            .IsRequired();

        builder.HasIndex(issue => issue.Number)
            .IsUnique();
    }
}
