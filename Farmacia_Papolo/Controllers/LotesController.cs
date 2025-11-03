using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Farmacia_Paolo.Models;

namespace Farmacia_Paolo.Controllers
{
    public class LotesController : Controller
    {
        private readonly FarmaciaContext _context;

        public LotesController(FarmaciaContext context)
        {
            _context = context;
        }

        // GET: Lotes/Index/5
        public async Task<IActionResult> Index(int? productoId)
        {
            if (productoId == null)
                return NotFound("Debe especificar un ID de producto.");

            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            ViewData["Producto"] = producto;

            var lotes = await _context.Lotes
                .Where(l => l.ProductoID == productoId)
                .OrderBy(l => l.FechaVencimiento) // Lógica FEFO
                .ToListAsync();

            return View(lotes);
        }

        // GET: Lotes/Create/5
        public async Task<IActionResult> Create(int? productoId)
        {
            if (productoId == null)
                return NotFound("Debe especificar un ID de producto.");

            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            ViewData["Producto"] = producto;

            var lote = new Lote
            {
                ProductoID = producto.ProductoID,
                FechaEntrada = DateTime.Today // Mantene la fecha estática 
            };

            return View(lote);
        }

        // POST: Lotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoteID,NumeroLote,CantidadActual,FechaVencimiento,FechaEntrada,ProductoID")] Lote lote)
        {
            if (ModelState.IsValid)
            {
                //REVISAR ANTES DE DESPLEGAR
                // Esto asegura que o se guarda el Lote Y el Movimiento, o no se guarda ninguno. 
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. Guardar el Lote
                        _context.Add(lote);
                        await _context.SaveChangesAsync(); // Esto genera el lote.LoteID

                        // 2. Crear el Movimiento de "Entrada"
                        var movimiento = new MovimientoInventario
                        {
                            LoteID = lote.LoteID,
                            UsuarioID = 1, // TEMPORAL: Reemplazar con el ID del usuario logueado
                            TipoMovimiento = "Entrada",
                            Cantidad = lote.CantidadActual,
                            FechaMovimiento = DateTime.Now,
                            Motivo = "Registro de lote nuevo"
                        };
                        _context.Add(movimiento);
                        await _context.SaveChangesAsync();

                        // 3. Confirmar la transacción
                        await transaction.CommitAsync();

                        return RedirectToAction(nameof(Index), new { productoId = lote.ProductoID });
                    }
                    catch (Exception)
                    {
                        // Si algo falla, se deshace todo.
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", "No se pudo guardar el lote. Ocurrió un error en la transacción.");
                    }
                }
            }

            ViewData["Producto"] = await _context.Productos.FindAsync(lote.ProductoID);
            return View(lote);
        }


        // GET: Lotes/Edit/5
        // La cantidad solo se cambia con un "Ajuste".
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var lote = await _context.Lotes.FindAsync(id);
            if (lote == null)
                return NotFound();

            ViewData["Producto"] = await _context.Productos.FindAsync(lote.ProductoID);
            return View(lote);
        }

        // POST: Lotes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoteID,NumeroLote,FechaVencimiento,FechaEntrada,ProductoID")] Lote lote)
        {
            // LÓGICA CLAVE MODIFICADA:
            // Removimos "CantidadActual" del [Bind]. La cantidad NO se edita, se "Ajusta".
            // Esta acción ahora solo edita los metadatos del lote 

            if (id != lote.LoteID)
                return NotFound();

            // Obtenemos el lote original de la BD para restaurar la CantidadActual
            var loteOriginal = await _context.Lotes.AsNoTracking().FirstOrDefaultAsync(l => l.LoteID == id);
            if (loteOriginal == null)
                return NotFound();


            lote.CantidadActual = loteOriginal.CantidadActual;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Lotes.Any(e => e.LoteID == lote.LoteID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index), new { productoId = lote.ProductoID });
            }
            ViewData["Producto"] = await _context.Productos.FindAsync(lote.ProductoID);
            return View(lote);
        }


        // GET: Lotes/AjustarStock/5
        public async Task<IActionResult> AjustarStock(int? id)
        {
            if (id == null)
                return NotFound();

            var lote = await _context.Lotes.Include(l => l.Producto).FirstOrDefaultAsync(l => l.LoteID == id);
            if (lote == null)
                return NotFound();

            // Usaremos un ViewModel para el formulario de ajuste
            var viewModel = new AjusteStockViewModel
            {
                LoteID = lote.LoteID,
                ProductoNombre = lote.Producto.Nombre,
                LoteNumero = lote.NumeroLote,
                CantidadActual = lote.CantidadActual,
                TipoMovimiento = "Merma", 
                CantidadAjuste = 1
            };

            return View(viewModel); 
        }

        // POST: Lotes/AjustarStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjustarStock(AjusteStockViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var lote = await _context.Lotes.FindAsync(viewModel.LoteID);
                if (lote == null)
                    return NotFound();

                // Validación de negocio
                if (viewModel.CantidadAjuste <= 0)
                {
                    ModelState.AddModelError("CantidadAjuste", "La cantidad a ajustar debe ser mayor a 0.");
                }
                else if (viewModel.TipoMovimiento != "Ajuste Positivo" && viewModel.CantidadAjuste > lote.CantidadActual)
                {
                    ModelState.AddModelError("CantidadAjuste", "No se puede ajustar una cantidad mayor al stock actual.");
                }

                if (!ModelState.IsValid)
                {
                    // Recargar datos si falla la validación
                    var producto = await _context.Productos.FindAsync(lote.ProductoID);
                    viewModel.ProductoNombre = producto.Nombre;
                    viewModel.LoteNumero = lote.NumeroLote;
                    viewModel.CantidadActual = lote.CantidadActual;
                    return View(viewModel);
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. Crear el Movimiento
                        var movimiento = new MovimientoInventario
                        {
                            LoteID = viewModel.LoteID,
                            UsuarioID = 1, // TEMPORAL: Reemplazar con ID de usuario logueado
                            TipoMovimiento = viewModel.TipoMovimiento, 
                            Cantidad = viewModel.CantidadAjuste,
                            FechaMovimiento = DateTime.Now,
                            Motivo = viewModel.Motivo
                        };
                        _context.Add(movimiento);

                        // 2. Actualizar la Cantidad del Lote
                        if (viewModel.TipoMovimiento == "Ajuste Positivo")
                        {
                            lote.CantidadActual += viewModel.CantidadAjuste;
                        }
                        else 
                        {
                            lote.CantidadActual -= viewModel.CantidadAjuste;
                        }
                        _context.Update(lote);

                        // 3. Guardar cambios y confirmar transacción
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return RedirectToAction(nameof(Index), new { productoId = lote.ProductoID });
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", "Error al guardar el ajuste.");
                    }
                }
            }

            // Recargar datos si el modelo no era válido desde el inicio
            if (viewModel.LoteID > 0)
            {
                var lote = await _context.Lotes.Include(l => l.Producto).FirstOrDefaultAsync(l => l.LoteID == viewModel.LoteID);
                if (lote != null)
                {
                    viewModel.ProductoNombre = lote.Producto.Nombre;
                    viewModel.LoteNumero = lote.NumeroLote;
                    viewModel.CantidadActual = lote.CantidadActual;
                }
            }
            return View(viewModel);
        }


        // GET: Lotes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var lote = await _context.Lotes
                .Include(l => l.Producto)
                .FirstOrDefaultAsync(m => m.LoteID == id);

            if (lote == null)
                return NotFound();

            return View(lote);
        }

        // POST: Lotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lote = await _context.Lotes.FindAsync(id);
            if (lote == null)
                return NotFound();

            var productoId = lote.ProductoID;

            // No se puede borrar un lote si tiene stock.
            if (lote.CantidadActual > 0)
            {
                TempData["Error"] = $"No se puede eliminar el lote {lote.NumeroLote} porque aún tiene {lote.CantidadActual} unidades en stock.";
                return RedirectToAction(nameof(Index), new { productoId = productoId });
            }

            try
            {
                // Si CantidadActual es 0, sí se puede borrar 
                _context.Lotes.Remove(lote);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { productoId = productoId });
            }
            catch (DbUpdateException)
            {
                // Este error puede saltar si un MovimientoInventario aún depende de él
                // (Si no configuraste OnDelete.Restrict en MovimientoInventario -> Lote)
                TempData["Error"] = "No se puede eliminar el lote porque tiene un historial de movimientos.";
                return RedirectToAction(nameof(Index), new { productoId = productoId });
            }
        }
    }
}