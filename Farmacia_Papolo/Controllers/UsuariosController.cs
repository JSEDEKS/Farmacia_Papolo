using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Farmacia_Paolo.Models;

namespace Farmacia_Paolo.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly FarmaciaContext _context;

        public UsuariosController(FarmaciaContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {

            var usuarios = _context.Usuarios.Include(u => u.Rol);
            return View(await usuarios.ToListAsync());
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {

            ViewData["RolID"] = new SelectList(_context.Roles, "RolID", "NombreRol");
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioID,NombreUsuario,NombreCompleto,PasswordHash,RolID")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // LÓGICA A IMPLEMENTAR ANTE DE DESPLEGAR:
                // ¡NUNCA GUARDES EL PASSWORD EN TEXTO PLANO!
                // Aquí debes "hashear" el password antes de guardarlo.
                // Ejemplo (usando una función ficticia 'HashPassword'):
                // usuario.PasswordHash = HashPassword(usuario.PasswordHash);
                // Por ahora, lo guardamos directo solo para la demo del CRUD.

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Si el modelo falla, recargamos la lista de Roles
            ViewData["RolID"] = new SelectList(_context.Roles, "RolID", "NombreRol", usuario.RolID);
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();


            ViewData["RolID"] = new SelectList(_context.Roles, "RolID", "NombreRol", usuario.RolID);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioID,NombreUsuario,NombreCompleto,RolID")] Usuario usuario)
        {
            // MODIFICADO: No incluimos el PasswordHash en el Bind.
            // El cambio de contraseña debe ser un proceso separado y seguro.

            if (id != usuario.UsuarioID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtenemos el usuario original de la BD para no perder el hash del password
                    var usuarioFromDb = await _context.Usuarios.FindAsync(id);
                    if (usuarioFromDb == null) return NotFound();

                    // Actualizamos solo los campos que permitimos editar
                    usuarioFromDb.NombreUsuario = usuario.NombreUsuario;
                    usuarioFromDb.NombreCompleto = usuario.NombreCompleto;
                    usuarioFromDb.RolID = usuario.RolID;

                    _context.Update(usuarioFromDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Usuarios.Any(e => e.UsuarioID == usuario.UsuarioID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolID"] = new SelectList(_context.Roles, "RolID", "NombreRol", usuario.RolID);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .Include(u => u.Rol) // Incluimos el Rol para mostrarlo
                .FirstOrDefaultAsync(m => m.UsuarioID == id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}