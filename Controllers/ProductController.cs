using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCDemo.Data;
using MVCDemo.Dto_s;
using MVCDemo.Models;
using System.Diagnostics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MVCDemo.Controllers
{

    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [Authorize]
        public async Task<IActionResult> Index(string? searchString,
            int count, int? pageIndex, int? pageSize)
        {
            try
            {
                List<ProductResultDto> list = new();



                // Configurar valores por defecto para el número de página y tamaño de página
                int currentPageIndex = pageIndex ?? 1;
                int currentPageSize = pageSize ?? 10;
                ViewData["CurrentPageIndex"] = currentPageIndex;
                ViewData["CurrentPageSize"] = currentPageSize;

                if (!String.IsNullOrEmpty(searchString))
                {
                    list = await _context.Products
                   .Include(s => s.User)
                   .Include(z => z.Categorie)
                   .Where(x => x.Name.ToLower().Contains(searchString.ToLower()))
                   .Select(c => new ProductResultDto
                   {
                       Id = c.Id,
                       Name = c.Name,
                       Quantity = c.Quantity,
                       Price = c.Price,
                       CreationAt = c.CreationAt,
                       CategorieId = c.CategorieId,
                       CategorieName = c.Categorie.Name,
                       UserId = c.User.Id,
                       UserName = c.User.UserName
                   })
                   .OrderBy(x=>x.CreationAt)
                   .ToListAsync();
                }
                else
                {
                    list = await _context.Products
                   .Include(s => s.User)
                   .Include(z => z.Categorie)
                   .Select(c => new ProductResultDto
                   {
                       Id = c.Id,
                       Name = c.Name,
                       Quantity = c.Quantity,
                       Price = c.Price,
                       CreationAt = c.CreationAt,
                       CategorieId = c.CategorieId,
                       CategorieName = c.Categorie.Name,
                       UserId = c.User.Id,
                       UserName = c.User.UserName
                   })
                   .Skip((currentPageIndex - 1) * currentPageSize)
                   .Take(currentPageSize)
                   .ToListAsync();


                }
                // Obtener el número total de productos filtrados
                int totalRecords = list.Count();
                ViewData["TotalRecords"] = totalRecords;


                return View(list);
            }
            catch (Exception e)
            {
                Console.WriteLine("Date : " + DateTime.Now + ", Error on Get Category: " + e.Message);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
          
        }
        [Authorize]
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
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ProductDto product)
        {
            try
            {
                
                Console.WriteLine("Id : " + product.Id);
                Console.WriteLine("Name : " + product.Name);
                Console.WriteLine("Quantity : " + product.Quantity);
                Console.WriteLine("Price : " + product.Price);
                Console.WriteLine("CategorieId : " + product.CategorieId);

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Model is Invalid";
                    var categories = await _context.Categories
                 .Select(i => new CategorieDto()
                 {
                     Id = i.Id,
                     Name = i.Name
                 }).ToListAsync();
                    product.Categories= categories;
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
                    TempData["SuccessMessage"] = "Product has been update successfully.";
                }
                else
                {
                    await _context.AddAsync(model);
                    TempData["SuccessMessage"] = "Product has been created successfully.";

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

        [Authorize]
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
