public class ArtCategoryItemDTO {
    public long Id  { get; set; }
    public required string NameCode { get; set; }
    public long? ParentId { get; set; }
    public long? ArtCategoryId { get; set; }
    public bool? IsPublic { get; set; }
    public bool? IsActive { get; set; }
    public short? OrderNum { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }

    public required string Name { get; set; }
}