
using minimal_api.Dominio.Emuns;
using minimal_api.Dominio.Entidades;

namespace Test.Domain.Entidades
{
    [TestClass]
    public class AdministradorTest
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        { 
              //Arrange
              var adm = new Administrador();

              //Action
             adm.Id= 1;
             adm.Email ="carolinezengo1@gmail.com";
             adm.Senha="050404";
             adm.Perfil ="Adm";

              //Assert
              Assert.AreEqual(1,adm.Id);
               Assert.AreEqual("carolinezengo1@gmail.com",adm.Email);
                Assert.AreEqual("050404",adm.Senha);
                 Assert.AreEqual("Adm",adm.Perfil);
        }
    }
}