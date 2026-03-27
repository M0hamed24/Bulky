using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [Range(1,100, ErrorMessage ="PLease enter between 1 and 100")]
        [DisplayName("Display Name")]
        public int DisplayOrder { get; set; }
    }
}
