using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiukaFotoRazor.Models;

[Table("art_article", Schema = "art")]
public class ART_ARTICLE
{
    [Key]
    [Column("id")]
    public long ID { get; set; }

    [Column("topic_id")]
    public long TOPIC_ID { get; set; }

    [Required]
    [Column("caption")]
    public required string CAPTION { get; set; }

    [Column("image_url")]
    public string? IMAGE_URL { get; set; }

    [Required]
    [Column("body")]
    public required string BODY { get; set; }

    [Column("published_date")]
    public DateTime? PUBLISHED_DATE { get; set; }

    [Column("admin_only")]
    public bool? ADMIN_ONLY { get; set; }

    [Column("login_required")]
    public bool? LOGIN_REQUIRED { get; set; }

    [Column("locked")]
    public bool? LOCKED { get; set; }

    [Column("pinned")]
    public bool? PINNED { get; set; }

    [Column("approved_date")]
    public DateTime? APPROVED_DATE { get; set; }

    [Column("approved_by")]
    public string? APPROVED_BY { get; set; }

    [Column("created_date")]
    public DateTime? CREATED_DATE { get; set; }

    [Column("created_by")]
    public string? CREATED_BY { get; set; }

    [Column("updated_date")]
    public DateTime? UPDATED_DATE { get; set; }

    [Column("updated_by")]
    public string? UPDATED_BY { get; set; }

    // Navigation to paragraphs
    public ICollection<ART_PARAGRAPH>? Paragraphs { get; set; }
}
