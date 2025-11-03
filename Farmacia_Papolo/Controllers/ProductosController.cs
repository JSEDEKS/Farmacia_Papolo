using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 
using Microsoft.EntityFrameworkCore;
using Farmacia_Paolo.Models;

namespace Farmacia_Paolo.Controllers
{
    public class ProductosController : Controller
    {
        private readonly FarmaciaContext _context;

        public ProductosController(FarmaciaContext context)
        {
            _context = context;
        }

        // GET: Productos
        // GET: Productos
        // GET: Productos
        public async Task<IActionResult> Index(string? searchString)
        {
            // Incluye el proveedor para mostrar el nombre en la tabla
            var productosQuery = _context.Productos
                .Include(p => p.Proveedor)
                .AsQueryable();

            // Lógica de búsqueda simple
            if (!string.IsNullOrEmpty(searchString))
            {
                productosQuery = productosQuery.Where(p =>
                    p.Nombre.Contains(searchString) ||
                    p.Codigo.Contains(searchString) ||
                    p.Categoria.Contains(searchString) ||
                    p.Proveedor.NombreProveedor.Contains(searchString));
            }

            // Ejecuta la consulta
            var productos = await productosQuery.ToListAsync();
            s
            return View(productos);
        }


        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            // pueda mostrar toda la información: el proveedor y el stock (lotes).
            var producto = await _context.Productos
                .Include(p => p.Proveedor)
                .Include(p => p.Lotes) // Trae los lotes asociados
                .FirstOrDefaultAsync(m => m.ProductoID == id); 

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {

            ViewData["ProveedorID"] = new SelectList(_context.Proveedores, "ProveedorID", "NombreProveedor");
            return View();
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("ProductoID,Nombre,Codigo,Categoria,Precio,StockMinimo,ProveedorID")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

          
            ViewData["ProveedorID"] = new SelectList(_context.Proveedores, "ProveedorID", "NombreProveedor", producto.ProveedorID);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            // Buscamos por ProductoID
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            // Pasamos la lista de Proveedores 
            ViewData["ProveedorID"] = new SelectList(_context.Proveedores, "ProveedorID", "NombreProveedor", producto.ProveedorID);
            return View(producto);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [Bind("ProductoID,Nombre,Codigo,Categoria,Precio,StockMinimo,ProveedorID")] Producto producto)
        {
            if (id != producto.ProductoID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.ProductoID)) 
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            //  Si hay un error, recargamos la lista de Proveedores.
            ViewData["ProveedorID"] = new SelectList(_context.Proveedores, "ProveedorID", "NombreProveedor", producto.ProveedorID);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            // Incluimos el Proveedor para mostrar un resumen amigable en la confirmación
            var producto = await _context.Productos
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(m => m.ProductoID == id); 

            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) 
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) 
                return NotFound();

            try
            {
                //  Agregamos un try-catch. Si este producto tiene Lotes
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                // El borrado falló, 
                // Enviamos un error a la vista.
                TempData["Error"] = "No se puede eliminar el producto porque tiene lotes de inventario registrados.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ProductoExists(int id)
        {

            return _context.Productos.Any(e => e.ProductoID == id);
        }
    }
}