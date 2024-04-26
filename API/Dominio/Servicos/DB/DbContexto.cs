using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Emuns;
using minimal_api.Dominio.Entidades;




namespace minimal_api.DB
{
    public class DbContexto : DbContext
    {
      private readonly IConfiguration _configurationAppStettings;
      public DbContexto(IConfiguration configurationAppStettings)
      {

             _configurationAppStettings = configurationAppStettings;
      }
        
         public DbSet<Veiculo> Veiculos {get; set;}
              
         public DbSet<Administrador> Administradores {get; set;} 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>().HasData(
              new Administrador{
                Id=1,
                Email="teste@teste.com",
                Senha="050404",
                Perfil= "Adm"

              }
            );
        }
        protected override void OnConfiguring( DbContextOptionsBuilder  optionBuilder)
      {
        var stringConnecting = _configurationAppStettings.GetConnectionString("mysql")?.ToString();
        
        if(!string.IsNullOrEmpty(stringConnecting))
        {
        optionBuilder.UseMySql(stringConnecting,
        ServerVersion.AutoDetect(stringConnecting));
         }
      }
    }
}