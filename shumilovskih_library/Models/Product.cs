using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace shumilovskih_library.Models
{
    /// <summary>
    /// Продукция компании
    /// </summary>
    [Table("products", Schema = "app")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("product_name")]
        [MaxLength(255)]
        public string ProductName { get; set; } = string.Empty;

        [Column("article")]
        [MaxLength(100)]
        public string Article { get; set; } = string.Empty;

        [Column("type")]
        [MaxLength(100)]
        public string Type { get; set; } = string.Empty;

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column("min_price")]
        [Range(0, double.MaxValue, ErrorMessage = "Цена должна быть неотрицательной")]
        public decimal MinPrice { get; set; }

        [Column("package_size")]
        [MaxLength(100)]
        public string PackageSize { get; set; } = string.Empty;

        [Column("weight")]
        [Range(0, double.MaxValue, ErrorMessage = "Вес должен быть неотрицательным")]
        public decimal Weight { get; set; }

        [Column("standard_number")]
        [MaxLength(100)]
        public string StandardNumber { get; set; } = string.Empty;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("modified_date")]
        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<SaleHistory> SalesHistory { get; set; } = new List<SaleHistory>();
    }
}