using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcNetCore2NazarRomanyuk.Models;
using MvcNetCore2NazarRomanyuk.Repositories;
using System.Security.Claims;

namespace MvcNetCore2NazarRomanyuk.Controllers
{
    public class ManagedController : Controller
    {
        private RepositoryLibros repo;
        public ManagedController(RepositoryLibros repo)
        {
            this.repo = repo;
        }

        public IActionResult ErrorAcces()
        {
            return View();
        }

        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(string user, string pass)
        {
            Usuario usuario = await this.repo.ExisteUsuario(user, pass);
            if (usuario != null)
            {
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                Claim claimId = new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString());
                identity.AddClaim(claimId);

                Claim claimName = new Claim(ClaimTypes.Name, usuario.Nombre.ToString());
                identity.AddClaim(claimName);

                Claim claimApellido = new Claim(("Apellidos"), usuario.Apellidos.ToString());
                identity.AddClaim(claimApellido);

                Claim claimEmail = new Claim(ClaimTypes.Email, usuario.Email.ToString());
                identity.AddClaim(claimEmail);

                Claim claimPass = new Claim(("Pass"), usuario.Pass.ToString());
                identity.AddClaim(claimPass);

                Claim claimFoto = new Claim(("Foto"), usuario.Foto.ToString());
                identity.AddClaim(claimFoto);


                ClaimsPrincipal usePrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, usePrincipal);
                string controller = TempData["controller"].ToString();
                string action = TempData["action"].ToString();
                return RedirectToAction(action, controller);

            }
            else
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }


        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
           // HttpContext.Session.Remove("IdProductos");
            return RedirectToAction("Index", "Libros");
        }
    }
}
