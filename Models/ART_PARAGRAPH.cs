using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiukaFotoRazor.Models;

[Table("art_paragraph", Schema = "art")]
public class ART_PARAGRAPH
{
    [Key]
    [Column("id")]
    public long ID { get; set; }

    [Column("article_id")]
    public long ARTICLE_ID { get; set; }

    [Column("body")]
    public string? BODY { get; set; }

    [Column("image_url")]
    public string? IMAGE_URL { get; set; }

    [Column("queue")]
    public short QUEUE { get; set; }

    [Column("is_wavesurfer")]
    public bool? IS_WAVESURFER { get; set; }

    [Column("wavesurfer_url")]
    public string? WAVESURFER_URL { get; set; }

    [Column("wavesurfer_title")]
    public string? WAVESURFER_TITLE { get; set; }

    [Column("wavesurfer_description")]
    public string? WAVESURFER_DESCRIPTION { get; set; }

    [Column("is_component")]
    public bool? IS_COMPONENT { get; set; }

    [Column("component_name")]
    public string? COMPONENT_NAME { get; set; }

    [Column("created_date")]
    public DateTime? CREATED_DATE { get; set; }

    [Column("created_by")]
    public string? CREATED_BY { get; set; }

    [Column("updated_date")]
    public DateTime? UPDATED_DATE { get; set; }

    [Column("updated_by")]
    public string? UPDATED_BY { get; set; }

    // Optional navigation
    public ART_ARTICLE? Article { get; set; }
}
