using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proyect_Snake_West.Models;


namespace Proyect_Snake_West.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ✅ Vista para crear usuario con rol
        public async Task<IActionResult> Crear()
        {
            var modelo = new UsuarioConRolViewModel
            {
                RolesDisponibles = _roleManager.Roles.Select(r => r.Name ?? string.Empty).ToList()
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(UsuarioConRolViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.RolesDisponibles = _roleManager.Roles.Select(r => r.Name ?? string.Empty).ToList();
                return View(modelo);
            }

            var usuario = new IdentityUser { UserName = modelo.Email, Email = modelo.Email };
            var resultado = await _userManager.CreateAsync(usuario, modelo.Password);

            if (resultado.Succeeded)
            {
                await _userManager.AddToRoleAsync(usuario, modelo.RolSeleccionado);
                TempData["mensaje"] = "Usuario creado correctamente con el rol asignado.";
                return RedirectToAction("Crear");
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            modelo.RolesDisponibles = _roleManager.Roles.Select(r => r.Name ?? string.Empty).ToList();
            return View(modelo);
        }

        // ✅ Vista para listar usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = _userManager.Users.ToList();
            var lista = new List<UsuarioConRolListViewModel>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                lista.Add(new UsuarioConRolListViewModel
                {
                    Id = usuario.Id,
                    Email = usuario.Email ?? string.Empty,
                    Rol = roles.FirstOrDefault() ?? string.Empty
                   });
            }

            return View(lista);
        }

        // ✅ Acción para eliminar usuario
        [HttpPost]
        public async Task<IActionResult> Eliminar(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
                return NotFound();

            if (usuario.UserName == User.Identity?.Name)
            {
                TempData["error"] = "No puedes eliminar tu propio usuario.";
                return RedirectToAction("Index");
            }

            await _userManager.DeleteAsync(usuario);
            TempData["mensaje"] = "Usuario eliminado correctamente.";
            return RedirectToAction("Index");
        }
    }

    internal class UsuarioConRolListViewModel
    {
        public string Id { get; internal set; }
        public string Email { get; internal set; }
        public string Rol { get; internal set; }
    }
}
