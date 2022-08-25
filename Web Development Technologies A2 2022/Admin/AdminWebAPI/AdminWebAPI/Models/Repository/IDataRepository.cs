namespace AdminWebAPI.Models.Repository;

public interface IDataRepository<TEntity, TKey> where TEntity : class {
    Task<IEnumerable<TEntity>> GetAll();
    Task<TEntity> Get(TKey id);
    //Task<TKey> Add(TEntity item);
    Task<TKey> Update(TKey id, TEntity item);
    //Task<TKey> Delete(TKey id);
}
