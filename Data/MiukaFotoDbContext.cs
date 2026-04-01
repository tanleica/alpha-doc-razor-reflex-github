using Microsoft.EntityFrameworkCore;
using MiukaFotoRazor.Models;

namespace MiukaFotoRazor.Data;

public class MiukaFotoDbContext(DbContextOptions<MiukaFotoDbContext> options) : DbContext(options)
{

    public DbSet<SYS_USER> SysUsers { get; set; } = null!;
    public DbSet<SYS_LANGUAGE> SysLanguages { get; set; } = null!;
    public DbSet<ART_ARTICLE> ArtArticles { get; set; } = null!;
    public DbSet<ART_PARAGRAPH> ArtParagraphs { get; set; } = null!;
    public DbSet<ART_CATEGORY_ITEM> ArtCategoryItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ART_PARAGRAPH>()
            .HasOne(p => p.Article)
            .WithMany(a => a.Paragraphs)
            .HasForeignKey(p => p.ARTICLE_ID);
    }
}

