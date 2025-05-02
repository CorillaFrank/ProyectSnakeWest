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

        public async Task<IActionResult> Crear()
        {
            var modelo = new UsuarioConRolViewModel
            {
                RolesDisponibles = _roleManager.Roles.Select(r => r.Name).ToList()
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(UsuarioConRolViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.RolesDisponibles = _roleManager.Roles.Select(r => r.Name).ToList();
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

            modelo.RolesDisponibles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(modelo);
        }
    }
}
