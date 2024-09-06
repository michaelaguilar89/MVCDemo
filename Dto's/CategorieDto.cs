using System.ComponentModel.DataAnnotations;

namespace MVCDemo.Dto_s
{
    public class CategorieDto
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "The Name is required!"), MaxLength(20)]
        public string Name { get; set; }

  
        
    }
}
