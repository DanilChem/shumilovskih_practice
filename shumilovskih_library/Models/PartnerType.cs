using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace shumilovskih_library.Models
{
    [Table("partner_types", Schema = "app")]
    public class PartnerType
    {
        [Key]
        [Column("partner_type_id")]
        public int PartnerTypeId { get; set; }

        [Column("type_name")]
        [MaxLength(100)]
        public string TypeName { get; set; } = string.Empty;

        public virtual ICollection<Partner> Partners { get; set; } = new List<Partner>();
    }
}