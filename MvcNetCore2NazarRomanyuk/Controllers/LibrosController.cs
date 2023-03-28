using Microsoft.AspNetCore.Mvc;
using MvcNetCore2NazarRomanyuk.Extensions;
using MvcNetCore2NazarRomanyuk.Filters;
using MvcNetCore2NazarRomanyuk.Models;
using MvcNetCore2NazarRomanyuk.Repositories;
using System.Security.Claims;

namespace MvcNetCore2NazarRomanyuk.Controllers
{
    public class LibrosController : Controller
    {
        private RepositoryLibros repo;

        public LibrosController(RepositoryLibros repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            List<Libro> libros = this.repo.GetLibros();
            return View(libros);
        }

        public IActionResult Details(int idlibro, int? idlibroCarrito)
        {
            if (idlibroCarrito != null)
            {
                List<int> idsLibros;
                if (HttpContext.Session.GetObject<List<int>>("IdLibros") == null)
                {

                    idsLibros = new List<int>();
                }
                else
                {
                    idsLibros = HttpContext.Session.GetObject<List<int>>("IdLibros");
                }
                idsLibros.Add(idlibroCarrito.Value);
                HttpContext.Session.SetObject("IdLibros", idsLibros);
            }

            Libro libro = this.repo.GetLibroById(idlibro);
            return View(libro);
        }

        public IActionResult LibrosByGenero(int idgenero)
        {
            List<Libro> libros = this.repo.GetLibrosByGenero(idgenero);
            return View(libros);
        }
        //paginacion

        public IActionResult LibrosPaginacion(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 0;
            }
            int numeroLibros = 0;
            Libro libro = this.repo.GetLibroPaginacion(posicion.Value, ref numeroLibros);
            ViewData["DATOS"] = "Libro " + (posicion + 1)
                + " de " + numeroLibros;
            int siguiente = posicion.Value + 1;
            if (siguiente >= numeroLibros)
            {
                siguiente = 0;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 0)
            {
                anterior = numeroLibros - 1;
            }
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            return View(libro);
        }

        /*seguridad//////////////////////////////////////////*/
        [AuthorizeUsuarios]
        public IActionResult PerfilUsuario()
        {
            return View();
        }
        [AuthorizeUsuarios]
        public IActionResult CarritoLibros()
        {
            List<int> idsLibros = HttpContext.Session.GetObject<List<int>>("IdLibros");
            if (idsLibros == null)
            {
                ViewData["MENSAJE"] = "No hay productos";
                return View();
            }
            
            List<Libro> productosSession = this.repo.GetProductosSession(idsLibros);
            return View(productosSession);
            
        }

        //insert///////////////////////////
        
        public IActionResult InsertarCompra()
        {
            List<int> idsLibro = HttpContext.Session.GetObject<List<int>>("IdLibros");
            int iduser = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            foreach(int ids in idsLibro)
            {
                this.repo.InsertPedido(ids, iduser);
            }
            HttpContext.Session.Remove("IdLibros");
            return RedirectToAction("VistaPedidosResult");
        }

       public IActionResult VistaPedidosResult()
        {
            int iduser = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<VistaPedidos> pedidos = this.repo.GetVistasPedidos(iduser);
            return View(pedidos);
        }
    }
}
