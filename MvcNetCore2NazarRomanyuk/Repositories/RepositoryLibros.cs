using MvcNetCore2NazarRomanyuk.Data;
using MvcNetCore2NazarRomanyuk.Models;

namespace MvcNetCore2NazarRomanyuk.Repositories
{
    public class RepositoryLibros
    {
        private LibrosContext context;
         public RepositoryLibros(LibrosContext context)
        {
            this.context = context;
        }

        public List<Libro> GetLibros()
        {
            return this.context.Libros.ToList();
        }

        public List<Genero> GetGeneros()
        {
            return this.context.Generos.ToList();
        }

        public Libro GetLibroById(int idlibro)
        {
            return this.context.Libros.FirstOrDefault(x => x.IdLibro == idlibro);
        }

        public List<Libro> GetLibrosByGenero(int idgenero)
        {
            var consulta = from data in this.context.Libros
                           where data.IdGenero == idgenero
                           select data;
            return consulta.ToList();
        }

        //seguridad
        public async Task<Usuario> ExisteUsuario(string email, string pass)
        {
            var consulta = this.context.Usuarios.Where(x => x.Email == email && x.Pass == pass);
            return consulta.FirstOrDefault();
        }

        //paginacion

        public Libro GetLibroPaginacion(int posicion, ref int numeroLibros)
        {
            List<Libro> libroList = this.GetLibros();
            numeroLibros = libroList.Count;

            Libro libro = libroList.Skip(posicion).Take(1).FirstOrDefault();
            return libro;
        }

        //carrito
        public List<Libro> GetProductosSession(List<int> ids)
        {
            var consulta = from datos in this.context.Libros
                           where ids.Contains(datos.IdLibro)
                           select datos;
            if (consulta.Count() == 0)
            {
                return null;
            }
            return consulta.ToList();
        }
        //insert
        private int GetMaxIdPedido()
        {
            if (this.context.Pedidos.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.Pedidos.Max(z => z.IdPedido) + 1;
            }
        }
        private int GetMaxIdFactura()
        {
            if (this.context.Pedidos.Count() == 0)
            {
                return 1;
            }
            else
            {
                return this.context.Pedidos.Max(z => z.IdFactura) + 1;
            }
        }

        public void InsertPedido(int idLibro, int idusuario)
        {
            Pedido pedido = new Pedido();
            pedido.IdPedido = this.GetMaxIdPedido();
            pedido.IdFactura = this.GetMaxIdFactura();
            pedido.Fecha = DateTime.Today;
            pedido.IdLibro = idLibro;
            pedido.IdUsuario = idusuario;
            pedido.Cantidad = 1;
            this.context.Pedidos.Add(pedido);
            this.context.SaveChanges();
        }
        //vistapedidos
        public List<VistaPedidos> GetVistasPedidos(int iduser)
        {
            var consulta = from data in this.context.VistaPedidos
                           where data.IdUser == iduser
                           select data;
            return consulta.ToList();
        }
    }
}
