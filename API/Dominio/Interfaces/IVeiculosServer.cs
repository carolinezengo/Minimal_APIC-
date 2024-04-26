using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.DTOs;


namespace minimal_api.Servicos
{
    public interface IVeiculosServer
    {
         List<Veiculo> Todos(int? pagina = 1, string? nome=null, string? marca=null);
          Veiculo? BuscarPorId (int id);
          Veiculo? BuscarPorNome (string nome);
          Veiculo? BuscarPorMarca(string marca);
           public void Incluir(Veiculo veiculo);
         void Atualizar(Veiculo veiculo);
         void Apagar(Veiculo veiculo);
    }
}