using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace shumilovskih_library.Models
{
    /// <summary>
    /// История продаж продукции партнёром
    /// </summary>
    [Table("sales_history", Schema = "app")]
    public class SaleHistory
    {
        [Key]
        [Column("sale_id")]
        public int SaleId { get; set; }

        [Required]
        [Column("partner_id")]
        public int PartnerId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public int Quantity { get; set; }

        [Required]
        [Column("sale_date")]
        public DateTime SaleDate { get; set; }

        [Required]
        [Column("total_amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Сумма должна быть неотрицательной")]
        public decimal TotalAmount { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [ForeignKey(nameof(PartnerId))]
        public virtual Partner Partner { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }
    }
}