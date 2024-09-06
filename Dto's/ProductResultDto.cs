using System.ComponentModel.DataAnnotations;

namespace MVCDemo.Dto_s
{
    public class ProductResultDto
    {
        public int Id { get; set; }
       
        public string Name { get; set; }
       
        public int Quantity { get; set; }
       
        public int Price { get; set; }
       
        public DateTime CreationAt { get; set; }
        
        public int CategorieId { get; set; }
        public string CategorieName { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }

    }
}
