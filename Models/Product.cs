using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCDemo.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="The Name is Required"),MaxLength(25)]
        public string Name { get; set; }
        [Required(ErrorMessage = "The Quantity is Required")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "The Price is Required")]
        public int Price { get; set; }
        [Required(ErrorMessage = "The Creation Date is Required")]
        public DateTime CreationAt { get; set; }
        [Required(ErrorMessage = "The Categorie Id is Required")]
        public int CategorieId { get; set; }
        [Required(ErrorMessage = "The User Id is Required")]
        public string UserId { get; set; }
        //Navigation Property
       

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
        [ForeignKey("CategorieId")]
        public Categorie Categorie { get; set; }
    }
}
