using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Application.Services.SOLID.L.Interfaces
{
    public interface IService<T>
    {
        T Get(int id);
        List<T> GetAll(string? includeProperties = null);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
