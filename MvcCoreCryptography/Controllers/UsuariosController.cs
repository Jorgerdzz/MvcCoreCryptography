using Microsoft.AspNetCore.Mvc;
using MvcCoreCryptography.Models;
using MvcCoreCryptography.Repositories;

namespace MvcCoreCryptography.Controllers
{
    public class UsuariosController : Controller
    {
        private RepositoryUsuarios repo;

        public UsuariosController(RepositoryUsuarios repo)
        {
            this.repo = repo;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string email, string imagen, string password)
        {
            await this.repo.RegisterUserAsync(nombre, email, imagen, password);
            ViewData["MENSAJE"] = "Usuario en el sistema";
            return View();
        }

        public async Task<IActionResult> LogIn(string email, string password)
        {
            Usuario user = await this.repo.LogInUserAsync(email, password);
            if(user == null)
            {
                ViewData["MENSAJE"] = "Credenciales correctas";
                return View();
            }
            else
            {
                return View(user);
            }
        }
    }
}
