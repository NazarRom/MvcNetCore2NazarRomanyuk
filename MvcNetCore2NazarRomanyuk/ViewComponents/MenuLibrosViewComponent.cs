using Microsoft.AspNetCore.Mvc;
using MvcNetCore2NazarRomanyuk.Models;
using MvcNetCore2NazarRomanyuk.Repositories;

namespace MvcNetCore2NazarRomanyuk.ViewComponents
{
    public class MenuLibrosViewComponent : ViewComponent
    {
        private RepositoryLibros repo;
        public MenuLibrosViewComponent(RepositoryLibros repo)
        {
            this.repo = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Genero> generos = this.repo.GetGeneros();
            return View(generos);
        }
    }
}
