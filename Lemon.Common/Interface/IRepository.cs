using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Winterspring.Interface
{
    public interface IRepository<T>
    {
        bool All(Expression<Func<T, bool>> cond);
        bool Any(Expression<Func<T, bool>> cond);

        T Single(Expression<Func<T, bool>> cond);
        T SingleOrDefault(Expression<Func<T, bool>> cond);
        IEnumerable<T> Where(Expression<Func<T, bool>> cond);

        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        void Save();
    }
}