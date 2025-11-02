using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibreriaDeClases.Modelos;
using LogicaHeladeria.Data;
using Heladeria.Servicios;
using System.Security.Claims;

namespace Heladeria.Controllers
{
    public class HeladoesController : Controller
    {
        private readonly HeladeriaDbContext _context;
        private readonly RolService _rolService;

        public HeladoesController(HeladeriaDbContext context, RolService rolService)
        {
            _context = context;
            _rolService = rolService;
        }

        // GET: Heladoes
        public async Task<IActionResult> Index()
        {
            var heladeriaDbContext = _context.Helados.Include(h => h.Categoria);
            
            var datos = heladeriaDbContext.GroupBy(h => h.Categoria.Nombre).Select(g => new
            {
                Categoria = g.Key,
                Cantidad = g.Count()
            }).ToList();

            ViewBag.DatosHeladosPorCategoria = System.Text.Json.JsonSerializer.Serialize(datos);

            // Verificar si el usuario es administrador para mostrar botones
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            ViewBag.EsAdministrador = await _rolService.EsAdministrador(email ?? "");

            return View(await heladeriaDbContext.ToListAsync());
        }

        // GET: Heladoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var helado = await _context.Helados
                .Include(h => h.Categoria)
                .FirstOrDefaultAsync(m => m.IdHelado == id);
            if (helado == null)
            {
                return NotFound();
            }

            // Verificar si el usuario es administrador para mostrar botón de editar
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            ViewBag.EsAdministrador = await _rolService.EsAdministrador(email ?? "");

            return View(helado);
        }

        // GET: Heladoes/Create
        public async Task<IActionResult> Create()
        {
            // Verificar si el usuario es administrador
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            if (!await _rolService.EsAdministrador(email ?? ""))
            {
                return Forbid(); // Retorna 403 Forbidden
            }

            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "IdCategoria", "Nombre");
            return View();
        }

        // POST: Heladoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdHelado,Sabor,Descripcion,Precio,CantidadEnStock,IdCategoria,FechaCreacion")] Helado helado)
        {
            // Verificar si el usuario es administrador
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            if (!await _rolService.EsAdministrador(email ?? ""))
            {
                return Forbid(); // Retorna 403 Forbidden
            }

            if (ModelState.IsValid)
            {
                _context.Add(helado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "IdCategoria", "Nombre", helado.IdCategoria);
            return View(helado);
        }

        // GET: Heladoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Verificar si el usuario es administrador
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            if (!await _rolService.EsAdministrador(email ?? ""))
            {
                return Forbid(); // Retorna 403 Forbidden
            }

            if (id == null)
            {
                return NotFound();
            }

            var helado = await _context.Helados.FindAsync(id);
            if (helado == null)
            {
                return NotFound();
            }
            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "IdCategoria", "Nombre", helado.IdCategoria);
            return View(helado);
        }

        // POST: Heladoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdHelado,Sabor,Descripcion,Precio,CantidadEnStock,IdCategoria,FechaCreacion")] Helado helado)
        {
            // Verificar si el usuario es administrador
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            if (!await _rolService.EsAdministrador(email ?? ""))
            {
                return Forbid(); // Retorna 403 Forbidden
            }

            if (id != helado.IdHelado)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(helado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HeladoExists(helado.IdHelado))
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
            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "IdCategoria", "Nombre", helado.IdCategoria);
            return View(helado);
        }

        // GET: Heladoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Verificar si el usuario es administrador
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            if (!await _rolService.EsAdministrador(email ?? ""))
            {
                return Forbid(); // Retorna 403 Forbidden
            }

            if (id == null)
            {
                return NotFound();
            }

            var helado = await _context.Helados
                .Include(h => h.Categoria)
                .FirstOrDefaultAsync(m => m.IdHelado == id);
            if (helado == null)
            {
                return NotFound();
            }

            return View(helado);
        }

        // POST: Heladoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Verificar si el usuario es administrador
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
            if (!await _rolService.EsAdministrador(email ?? ""))
            {
                return Forbid(); // Retorna 403 Forbidden
            }

            var helado = await _context.Helados.FindAsync(id);
            if (helado != null)
            {
                _context.Helados.Remove(helado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HeladoExists(int id)
        {
            return _context.Helados.Any(e => e.IdHelado == id);
        }
    }
}
