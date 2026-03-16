using shumilovskih_library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("partners", Schema = "app")]
public class Partner
{
    [Key]
    [Column("partner_id")]
    public int PartnerId { get; set; }

    [Required]
    [Column("company_name")]
    [MaxLength(255)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [Column("partner_type_id")]
    public int PartnerTypeId { get; set; }

    [Column("inn")]
    [MaxLength(20)]
    public string Inn { get; set; } = string.Empty;

    [Column("logo")]
    [MaxLength(500)]
    public string Logo { get; set; } = string.Empty;

    [Column("sales_locations")]
    public string SalesLocations { get; set; } = string.Empty;

    [Required]
    [Column("rating")]
    [Range(0, int.MaxValue, ErrorMessage = "Рейтинг должен быть неотрицательным числом")]
    public int Rating { get; set; }

    [Column("address")]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Column("director_name")]
    [MaxLength(255)]
    public string DirectorName { get; set; } = string.Empty;

    [Column("phone")]
    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    [Column("email")]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Column("created_date")]
    public DateTime CreatedDate { get; set; }

    [Column("modified_date")]
    public DateTime ModifiedDate { get; set; }

    [ForeignKey(nameof(PartnerTypeId))]
    public virtual PartnerType PartnerType { get; set; }

    public virtual ICollection<SaleHistory> SalesHistory { get; set; } = new List<SaleHistory>();
}