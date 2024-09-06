using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCDemo.Data;
using MVCDemo.Dto_s;
using MVCDemo.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MVCDemo.Controllers
{
   
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _context.Products
                    .Include(s=>s.User)
                    .Include(z=>z.Categorie)
                    .Select(c=>new ProductResultDto
                    {
                        Id=c.Id,
                        Name=c.Name,
                        Quantity=c.Quantity,
                        Price=c.Price,
                        CreationAt=c.CreationAt,
                        CategorieId=c.CategorieId,
                        CategorieName=c.Categorie.Name,
                        UserId=c.User.Id,
                        UserName=c.User.UserName
                    })
                    .ToListAsync();
                foreach (var item in list)
                {
                    Console.WriteLine("data : " + item.Id + " |" + item.Name + " | "
                        + item.Quantity + " | " + item.Price + " | " + item.CreationAt 
                        + " | " + item.CategorieId + " | " + item.UserId);
                }
                return View(list);
            }
            catch (Exception e)
            {
                Console.WriteLine("Date : " + DateTime.Now + ", Error on Get Category: " + e.Message);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
          
        }
        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            try
            {
                var categories = await _context.Categories
                    .Select(i => new CategorieDto()
                    {
                        Id = i.Id,
                        Name=i.Name
                    }).ToListAsync();

                if (id>0)
                {
                    var item = await _context.Products.FindAsync(id);
                    if (item!=null)
                    {
                        ProductDto dto = new();
                        dto.Id = item.Id;
                        dto.Name = item.Name;
                        dto.Quantity = item.Quantity;
                        dto.Price = item.Price;
                        dto.CategorieId = item.CategorieId;
                        dto.Categories = categories;
                        return View(dto);
                    }
                    if (item==null)
                    {
                        return NotFound();
                    }
                }

                ProductDto productDto = new()
                {
                    Categories = categories
                };
               
                 return View( productDto);
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Date : " + DateTime.Now + ", Error on Get Category: " + e.Message);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ProductDto product)
        {
            try
            {
                /*  foreach(var i in product.Categories)
                   {
                       Console.WriteLine("Id : " + i.Id + ", Name : " + i.Name);
                   }*/
                Console.WriteLine("Id : " + product.Id);
                Console.WriteLine("Name : " + product.Name);
                Console.WriteLine("Quantity : " + product.Quantity);
                Console.WriteLine("Price : " + product.Price);
                Console.WriteLine("CategorieId : " + product.CategorieId);

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Model is Invalid";
                    return View(product);

                }
                // Obtén el ID del usuario actual
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Product model = new();
                model.Id = product.Id;
                model.Name = product.Name;
                model.Quantity = product.Quantity;
                model.Price = product.Price;
                model.CategorieId = product.CategorieId;
                model.CreationAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                model.UserId = user;
                if (product.Id > 0)
                {
                    _context.Update(model);
                    TempData["SuccessMessage"] = "Category has been update successfully.";
                }
                else
                {
                    await _context.AddAsync(model);
                    TempData["SuccessMessage"] = "Category has been created successfully.";

                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                Console.WriteLine("Date : " + DateTime.Now + ", Error on Get Category: " + e.Message);
                TempData["ErrorMessages "] = e.Message;
                return RedirectToAction("Error");
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id!=null)
                {
                    var item = await _context.Products.FindAsync(id);
                    if (item!=null)
                    {
                        _context.Products.Remove(item);
                       await _context.SaveChangesAsync();
                        // Almacenar el mensaje de error en TempData
                        TempData["ErrorMessage"] = "This product is deleted....";

                        // Redirigir al Index 
                        return RedirectToAction(nameof(Index));
                    }
                    return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }
                return View();
               
            }
            catch (Exception e)
            {
                Console.WriteLine("Date : " + DateTime.Now + ", Error on Get Category: " + e.Message);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}
