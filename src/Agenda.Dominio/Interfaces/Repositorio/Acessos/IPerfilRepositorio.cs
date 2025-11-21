using Agenda.Dominio.Entidades.Acessos;
using Agenda.Dominio.Entidades.Acessos.DTO;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTO;
using Agenda.Dominio.Interfaces;


namespace Agenda.Dominio.Interfaces.Repositorio.Acessos
{

    //Entidade de Dominio: Perfil
    public interface IPerfilRepositorio 
        : IBaseRepositorio<Perfil>
        , IDisposable
    {
        Task<RetornoGridPaginado<PerfilGridDto>> CarregarGridPerfil(DataTableSearch search, int start, int length, int draw);
        Task<IEnumerable<DataSelect2DTO>> CarregarComboNivelAcesso(string search, int page, int? length = 10);
        Task<IEnumerable<MenusLiberadosDTO>> CarregarMenusLiberadosAcessos(long? idModulo);
    }
}
