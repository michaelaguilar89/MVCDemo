using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCDemo.Models
{
    public class Categorie
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="The Name is required!"),MaxLength(20)]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Creation Date is Required")]
        public DateTime CreationAt { get; set; }
        [Required(ErrorMessage = "The User Id is Required")]
        public string UserId { get; set; }
        //Navigation Property


        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
    }
}
