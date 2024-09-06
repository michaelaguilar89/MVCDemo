using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCDemo.Data;
using MVCDemo.Dto_s;
using MVCDemo.Models;
using System;
using System.Diagnostics;
using System.Security.Claims;

namespace MVCDemo.Controllers
{
    public class CategorieController : Controller
    {
        private readonly ApplicationDbContext _context;
        

        public List<Categorie> list = new List<Categorie>();

        
        public string ErrorMessages {  get; set; }
        public string StatusMessages { get; set; }

        public Categorie Categorie = new Categorie();

        public CategorieController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _context.Categories
                    .Include(c=>c.User)
                    .Select(i=>new Categorie
                    {
                        Id=i.Id,
                        Name=i.Name,
                        CreationAt=i.CreationAt,
                        UserId=i.User.UserName
                    })
                    .ToListAsync();
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
                if (id != null && id>0)
                {
                    var categorie = await _context.Categories.FindAsync(id);
                    if (categorie==null)
                    {
                        return NotFound();
                    }
                    // Mapea la categoría a CategorieDto
                    var categorieDto = new CategorieDto
                    {
                        Id = categorie.Id,
                        Name = categorie.Name
                    };
                    return View(categorieDto);
                }
                return View(new CategorieDto());
            }
            catch (Exception e)
            {

                
                Console.WriteLine("Date: " + DateTime.Now + ", Error on Get Categorie: " + e.Message);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Redirigir a la vista "Error"
            }

            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Redirigir a la vista "Error"

        }
        [HttpPost]
        public async Task<IActionResult>AddEdit(CategorieDto model)
        {
            try
            {
                
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                    Console.WriteLine("Model is InValid");
                    Console.WriteLine("Categorie : Id :" + model.Id + " Name :  " + model.Name );
                    return View(model);
                   

                }
                else
                {
                    Console.WriteLine("Model is valid");
                    // Obtén el ID del usuario actual
                    var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
                  Categorie categorie = new();
                    categorie.Id = model.Id;
                    categorie.Name = model.Name;
                    categorie.CreationAt= DateTime.SpecifyKind(DateTime.Now,DateTimeKind.Utc);
                    categorie.UserId = user;
                    // Asigna el ID del usuario actual al modelo
                  
                    Console.WriteLine("model.UserId : " + user);
                   
                    if (categorie.Id > 0)
                    {
                        _context.Categories.Update(categorie);
                        TempData["SuccessMessage"] = "Category has been update successfully.";
                    }
                    else
                    {
                        await _context.Categories.AddAsync(categorie);
                        TempData["SuccessMessage"] = "Category has been created successfully.";
                       
                    }

                    await _context.SaveChangesAsync();
                   
                    return RedirectToAction(nameof(Index));
                }
                
            }
            catch (Exception e)
            {

                ErrorMessages = e.Message;
                Console.WriteLine("Date : " + DateTime.Now + ", Error on Get Categorie : " + e.Message);
                return View(ErrorMessages);
            }
        }


        
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id!=null && id>0)
                {
                    var categorie = await _context.Categories.FindAsync(id);
                    if (categorie!=null)
                    {
                        var isInUsed = await _context.Products.AnyAsync(X => X.CategorieId == id);
                        if (isInUsed)
                        {
                            // Almacenar el mensaje de error en TempData
                            TempData["ErrorMessage"] = "This category is in use and cannot be deleted.";

                            // Redirigir al Index 
                            return RedirectToAction(nameof(Index));
                        }
                        
                        _context.Categories.Remove(categorie);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    // Almacenar el mensaje de error en TempData
                    TempData["ErrorMessage"] = "Category not Found! ";

                    // Redirigir al Index 
                    return RedirectToAction(nameof(Index));
                }
                // Almacenar el mensaje de error en TempData
                TempData["ErrorMessage"] = "This category Id is not Found!";

                // Redirigir al Index 
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {

                return View(e.Message);
            }

            return View("Internal Error");
        }
    }
}
