using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MASAMADREPROY.Data;
using MASAMADREPROY.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;

namespace MASAMADREPROY.Controllers
{
    public class ProductoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductoesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Productoes
        public async Task<IActionResult> Index(int page = 1, int pageSize = 3)
        {
            ViewData["Title"] = "Productos";

            // Total de productos
            var totalItems = await _context.Productos.CountAsync();

            // Calcular el número total de páginas
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Asegurarse de que la página esté dentro del rango válido
            page = Math.Max(1, Math.Min(page, totalPages));

            // Obtener los productos para la página actual
            var productos = await _context.Productos
                .Skip((page - 1) * pageSize) // Omitir los productos de las páginas anteriores
                .Take(pageSize) // Tomar solo el número de productos según pageSize
                .ToListAsync();

            // Pasar los datos a la vista
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = page;
            ViewData["PageSize"] = pageSize;

            return View(productos);
        }

        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productoes/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Imagen,Precio,CantidadEnStock,Disponible")] Producto producto, IFormFile imagen)
        {
            if (ModelState.IsValid)
            {
                if (imagen != null)
                {
                    string carpetaImagenes = Path.Combine(_webHostEnvironment.WebRootPath, "Imagenes");
                    if (!Directory.Exists(carpetaImagenes))
                    {
                        Directory.CreateDirectory(carpetaImagenes);
                    }

                    string nombreImagen = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                    string rutaImagen = Path.Combine(carpetaImagenes, nombreImagen);

                    using (var stream = new FileStream(rutaImagen, FileMode.Create))
                    {
                        await imagen.CopyToAsync(stream);
                    }

                    producto.Imagen = "/Imagenes/" + nombreImagen;
                }

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }

        // GET: Productoes/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // POST: Productoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Imagen,Precio,CantidadEnStock,Disponible")] Producto producto, IFormFile imagen)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imagen != null)
                    {
                        string carpetaImagenes = Path.Combine(_webHostEnvironment.WebRootPath, "Imagenes");
                        if (!Directory.Exists(carpetaImagenes))
                        {
                            Directory.CreateDirectory(carpetaImagenes);
                        }

                        string nombreImagen = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                        string rutaImagen = Path.Combine(carpetaImagenes, nombreImagen);

                        using (var stream = new FileStream(rutaImagen, FileMode.Create))
                        {
                            await imagen.CopyToAsync(stream);
                        }

                        producto.Imagen = "/Imagenes/" + nombreImagen;
                    }

                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: Productoes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportarExcel()
        {
            var productos = await _context.Productos.ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Productos");

                // Cabeceras de las columnas
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Nombre";
                worksheet.Cell(1, 3).Value = "Descripción";
                worksheet.Cell(1, 4).Value = "Precio";
                worksheet.Cell(1, 5).Value = "Cantidad en Stock";
                worksheet.Cell(1, 6).Value = "Disponible";

                // Rellenar las filas con los datos de productos
                int row = 2;
                foreach (var producto in productos)
                {
                    worksheet.Cell(row, 1).Value = producto.Id;
                    worksheet.Cell(row, 2).Value = producto.Nombre;
                    worksheet.Cell(row, 3).Value = producto.Descripcion;
                    worksheet.Cell(row, 4).Value = producto.Precio;
                    worksheet.Cell(row, 5).Value = producto.CantidadEnStock;
                    worksheet.Cell(row, 6).Value = producto.Disponible ? "Sí" : "No";
                    row++;
                }

                // Ajustar el tamaño de las columnas para el contenido
                worksheet.Columns().AdjustToContents();

                // Crear un MemoryStream para guardar el archivo
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Productos.xlsx");
                }
            }
        }
    }
}
