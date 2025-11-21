using Agenda.Aplicacao.Interfaces;
using Agenda.Dominio;
using Agenda.Dominio.Entidades.Acessos;
using Agenda.Dominio.Entidades.Acessos.DTO;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTO;


namespace Agenda.Aplicacao.Interfaces.Acessos
{
    //Entidade de Dominio: Perfil
    public interface IPerfilAppServicos
        : IBaseAppServicos<Perfil>
        , IDisposable
    {
        Task<RetornoGridPaginado<PerfilGridDto>> CarregarGridPerfil(DataTableSearch search, int start, int length, int draw);
        Task<IEnumerable<DataSelect2DTO>> CarregarComboNivelAcesso(string search, int page, int? length = 10);
        Task<IEnumerable<MenusLiberadosDTO>> CarregarMenusLiberadosAcessos(long? idModulo);
    }
}
