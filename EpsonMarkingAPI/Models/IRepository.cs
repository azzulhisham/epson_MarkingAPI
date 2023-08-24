using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace EpsonMarkingAPI.Models
{
    public interface IRepository<T>
    {
        T Insert(T entity);
        T Update(T entity);
        void Delete(T entity);
        IQueryable<T> FindFor(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll();
        T GetById(Int64 id);
    }
}
