using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.DB;
using minimal_api.Dominio.Emuns;
using minimal_api.Dominio.Entidades;
using minimal_api.Servicos;

namespace Test.Domain.Servicos
{
  [TestClass]
  public class AdminstradorServerTest
  {
    
    private DbContexto CriarContextoTeste()
    {
      var assemblypath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      var path = Path.GetFullPath(Path.Combine(assemblypath ?? "", "..", "..", ".."));

      var builder = new ConfigurationBuilder()
      .SetBasePath(path ?? Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddEnvironmentVariables();

      var configuration = builder.Build();

      return new DbContexto(configuration);
    }
    public void TestandoIncluirADM()
    {
      //Arrange
      var context = CriarContextoTeste();
      context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

      var adm = new Administrador();
      adm.Id = 2;
      adm.Email = "teste1@teste.com";
      adm.Senha = "teste";
      adm.Perfil = "Adm";

      var admserver = new AdministradorServer(context);
      //Action

       admserver.Incluir(adm);
      
      //Assert

      
      Assert.AreEqual(1,admserver.Todos(1).Count());
       Assert.AreEqual("teste1@teste.com",adm.Email);
                Assert.AreEqual("teste",adm.Senha);
                 Assert.AreEqual("Adm",adm.Perfil);

      }

    [TestMethod]
    public void TestandoBuscarPorID()
    {
      //Arrange
      var context = CriarContextoTeste();
      context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

      var adm = new Administrador();
     
      adm.Email = "teste1@teste.com";
      adm.Senha = "teste";
      adm.Perfil = "Adm";

      var admserver = new AdministradorServer(context);
      //Action

      admserver.Incluir(adm);
      var admBanco = admserver.BuscarPorId(adm.Id);


      //Assert

      Assert.AreEqual(1, admBanco.Id);
      
    }

  }
}
