using Agapea_Blazor.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Text.Json;

namespace Agapea_Blazor.Server.Models
{
    public class AppDBContext : IdentityDbContext
    {
        /*
         * Esta clase va a servir para q Identity genera a traves de EF el DBContext para generar tablas a partir de clases modelo usando DbSets...
         * como estamos creando un DBContext personalizado pq vamos a añadir tablas propias ademas de las de Identity (como direcciones, pedidos..)
         * EF te obliga a sobrecargar el constructor (sino lo pones te salta un error indicandote q sobrecargues el constructor)
         */

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options){ }

        #region ... PROPS
        //nos definimos un DbSet por cada clase modelo a mapear en el DbContext como prop :

        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Provincia> Provincias { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<PagoPaypal> PagosPaypal { get; set; }
        public DbSet<PagoStripe> PagosStripe { get; set; }
        #endregion

        #region ... METODOS
        //metodo q se ejecuta para crear las tablas a partir de las clases en el momento q se lanza migracion ::
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region /// modificacion tabla IdentityUser ///

            builder.Entity<MiClienteIdentity>();

            #endregion

            #region /// creacion tabla Direcciones ///

            builder.Entity<Direccion>().ToTable("Direcciones"); // .ToTable() es opcional para especificar el nombre de la tabla que quieres, sino será Direcciones
            builder.Entity<Direccion>().HasKey(p => new { p.IdDireccion, p.IdCliente }); // PKS
            builder.Entity<Direccion>().Property((Direccion dir) => dir.Calle).IsRequired().HasMaxLength(250);
            builder.Entity<Direccion>().Property((Direccion dir) => dir.CP).IsRequired().HasMaxLength(5);

            //cuando la clase modelo tiene como prop. un obj de otra clase, EF no puede mapearlo contra un tipo sql, solucion:
            // -> no almacenamos esa prop como col de tabla
            //-> serializas esa prop a un string usando metodo: .HasConversion(1ºparam_lambda_serialize, 2ºparam_lambda_deserialize)

            builder.Entity<Direccion>().Property((Direccion dir) => dir.ProvinciaDirec)
                                       .HasConversion(
                                            prov => JsonSerializer.Serialize<Provincia>(prov,(JsonSerializerOptions)null),
                                            prov => JsonSerializer.Deserialize<Provincia>(prov, (JsonSerializerOptions)null)
                                      ).HasColumnName("Provincia");
            
            builder.Entity<Direccion>().Property((Direccion dir) => dir.MunicipioDirec)
                                       .HasConversion(
                                            muni => JsonSerializer.Serialize<Municipio>(muni, (JsonSerializerOptions)null),
                                            muni => JsonSerializer.Deserialize<Municipio>(muni, (JsonSerializerOptions)null)
                                      ).HasColumnName("Municipio");
            #endregion


            #region /// creacion tabla Libros ///
            builder.Entity<Libro>().ToTable("Libros");
            builder.Entity<Libro>().HasKey("ISBN13");
            builder.Entity<Libro>().Property((Libro lib) => lib.Precio).HasColumnType("DECIMAL(5,2)");
            #endregion

            #region /// creacion tabla Categorias ///
            builder.Entity<Categoria>().ToTable("Categorias");
            builder.Entity<Categoria>().HasNoKey();
            #endregion

            #region /// creacion tabla Pedidos ///
            builder.Entity<Pedido>().ToTable("Pedidos");
            builder.Entity<Pedido>().HasKey("IdPedido");
            builder.Entity<Pedido>().Property((Pedido ped) => ped.GastosEnvio).HasColumnType("DECIMAL(5,2)");
            builder.Entity<Pedido>().Property((Pedido ped) => ped.Subtotal).HasColumnType("DECIMAL(5,2)");
            builder.Entity<Pedido>().Property((Pedido ped) => ped.Total).HasColumnType("DECIMAL(5,2)");
            builder.Entity<Pedido>().Property((Pedido ped) => ped.ElementosPedido)
                                    .HasConversion(
                                        lista => JsonSerializer.Serialize(lista, (JsonSerializerOptions)null),
                                        lista => JsonSerializer.Deserialize<List<Item>>(lista, (JsonSerializerOptions)null)
                                    ).HasColumnName("listaItems");

            #endregion

            builder.Entity<Provincia>().HasNoKey();
            builder.Entity<Municipio>().HasNoKey();
            builder.Entity<PagoPaypal>().ToTable("PagosPayPal").HasKey(p=> new {p.IdCliente, p.IdPedido, p.IdPagoPayPal});
            builder.Entity<PagoStripe>().ToTable("PagosStripe").HasKey(p => new { p.IdCliente, p.IdPedido, p.ChargeId });
        }
        #endregion
    }
}
