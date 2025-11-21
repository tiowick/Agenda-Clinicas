using Agenda.Dominio.Entidades.DTOS;

namespace Agenda.Aplicacao.Interfaces
{
    public interface IBaseAppServicos<TEntity> : IDisposable where TEntity : class
    {
        TransferenciaIdentidadeDTO Identidade { get; }
        bool ErrorRepositorio { get; }
        string MessageError { get; }
        public long ID { get; set; }
        public bool IDCreated { get; set; }
        Task<long> CreateOrUpdate(TEntity entity);
        Task<bool> CreateList(IEnumerable<TEntity> entity);
        Task<bool> Delete(long id);
        Task<bool> DeleteList(IEnumerable<long> id);
        Task<IEnumerable<TEntity>> GetData(long id);
        Task<IEnumerable<TEntity>> GetData();
    }
}
