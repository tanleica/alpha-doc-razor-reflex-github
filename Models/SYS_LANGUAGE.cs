using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;  
namespace MiukaFotoRazor.Models;                                                                          

[Table("sys_language", Schema = "public")]
public class SYS_LANGUAGE
{
    [Key]
    [Column("id")]
    public long ID { get; set; }

    [Required]
    [Column("key")]
    public required string KEY { get; set; }

    [Required]
    [Column("vi")]
    public required string VI { get; set; }

    [Required]
    [Column("en")]
    public required string EN { get; set; }

    [Column("created_date")]
    public DateTime? CREATED_DATE { get; set; }
    [Column("created_by")]
    public string? CREATED_BY { get; set; }
    [Column("updated_date")]
    public DateTime? UPDATED_DATE { get; set; }          
    [Column("updated_by")]
    public string? UPDATED_BY { get; set; }
}
