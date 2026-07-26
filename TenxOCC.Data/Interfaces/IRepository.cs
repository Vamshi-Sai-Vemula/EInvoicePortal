using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TenxOCC.Data.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Delete(TEntity entityToDelete);

        void Delete(object id);

        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        TEntity GetByID(object id);

        IQueryable<TEntity> GetAll();

        IEnumerable<TEntity> GetWithRawSql(string query,
            params object[] parameters);

        TEntity Insert(TEntity entity);

        int Update(TEntity entityToUpdate);

        int Update(TEntity entity, List<Expression<Func<TEntity, object>>> projectionProperties);
    }

}

