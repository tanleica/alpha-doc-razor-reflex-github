using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiukaFotoRazor.Models;

[Table("sys_user", Schema = "public")]
public class SYS_USER
{
    [Key]
    [StringLength(36)]
    [Column("id")]
    public required string ID { get; set; }

    [Required]
    [StringLength(256)]
    [Column("username")]
    public required string USERNAME { get; set; }

    [Required]
    [StringLength(256)]
    [Column("email")]
    public required string EMAIL { get; set; }

    [Required]
    [Column("passwordhash")]
    public required string PASSWORDHASH { get; set; }

    [StringLength(100)]
    [Column("fullname")]
    public string? FULLNAME { get; set; }

    [Column("group_id")]
    public long? GROUP_ID { get; set; }

    [Column("is_root")]
    public bool? IS_ROOT { get; set; }

    [Column("is_admin")]
    public bool? IS_ADMIN { get; set; }

    [StringLength(150)]
    [Column("avatar")]
    public string? AVATAR { get; set; }

    [Column("is_locked")]
    public bool? IS_LOCKED { get; set; }

    [Column("webapp_allowed")]
    public bool? WEBAPP_ALLOWED { get; set; }

    [Column("portal_allowed")]
    public bool? PORTAL_ALLOWED { get; set; }

    [Column("mobile_allowed")]
    public bool? MOBILE_ALLOWED { get; set; }

    [Column("is_first_login")]
    public bool? IS_FIRST_LOGIN { get; set; }

    // ❗ Cột này bị anh để Column("") → phải sửa theo đúng tên trong DB
    [StringLength(50)]
    [Column("change_password_code")]
    public string? CHANGE_PASSWORD_CODE { get; set; }

    [Column("emailconfirmed")]
    public bool EMAILCONFIRMED { get; set; }

    [Column("employee_id")]
    public long? EMPLOYEE_ID { get; set; }

    [Column("is_ldap")]
    public bool? IS_LDAP { get; set; }

    [Column("is_mobile")]
    public bool? IS_MOBILE { get; set; }

    [Column("is_portal")]
    public bool? IS_PORTAL { get; set; }

    [Column("is_webapp")]
    public bool? IS_WEBAPP { get; set; }

    [StringLength(255)]
    [Column("otp_hash")]
    public string? OTP_HASH { get; set; }

    [Column("otp_expired")]
    public DateTime? OTP_EXPIRED { get; set; }

    [Column("mfa_enable")]
    public bool? MFA_ENABLE { get; set; }

    [StringLength(150)]
    [Column("mfa_secret")]
    public string? MFA_SECRET { get; set; }

    [StringLength(36)]
    [Column("created_by")]
    public string? CREATED_BY { get; set; }

    [StringLength(36)]
    [Column("updated_by")]
    public string? UPDATED_BY { get; set; }

    [Column("created_date")]
    public DateTime? CREATED_DATE { get; set; }

    [Column("updated_date")]
    public DateTime? UPDATED_DATE { get; set; }
}
