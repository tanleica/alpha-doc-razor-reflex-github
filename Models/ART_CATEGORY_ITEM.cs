using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiukaFotoRazor.Models
{
    [Table("art_category_item", Schema = "art")]
    public class ART_CATEGORY_ITEM
    {
        [Key]
        [Column("id")]
        public long ID { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("name_code")]
        public required string NAME_CODE { get; set; }

        [Column("parent_id")]
        public long? PARENT_ID { get; set; }

        [Column("art_category_id")]
        public long? ART_CATEGORY_ID { get; set; }

        [Column("is_public")]
        public bool? IS_PUBLIC { get; set; }

        [Column("is_active")]
        public bool? IS_ACTIVE { get; set; }

        [Column("order_num")]
        public short? ORDER_NUM { get; set; }

        [Column("created_date")]
        public DateTime? CREATED_DATE { get; set; }

        [Column("created_by")]
        public string? CREATED_BY { get; set; }

        [Column("updated_date")]
        public DateTime? UPDATED_DATE { get; set; }

        [Column("updated_by")]
        public string? UPDATED_BY { get; set; }
    }
}
