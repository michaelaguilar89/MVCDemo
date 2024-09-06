using MVCDemo.Models;
using System.ComponentModel.DataAnnotations;

namespace MVCDemo.Dto_s
{
    public class ProductDto
    {
        
        public int Id { get; set; }
        [Required(ErrorMessage = "The Name is Required"), MaxLength(25)]
        public string Name { get; set; }
        [Required(ErrorMessage = "The Quantity is Required")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "The Price is Required")]
        public int Price { get; set; }
       
        [Required(ErrorMessage = "The Categorie Id is Required")]
        public int CategorieId { get; set; }

        public List<CategorieDto>? Categories { get; set; } // Lista de categorías



    }
}
