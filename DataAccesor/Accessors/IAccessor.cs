using DataAccessor.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessor.Accessors
{
    interface IAccessor<T>
    {
        ICollection<T> GetAll();
        T GetById(int id);
        void DeleteById(int id);
        void Insert(T p);
    }
}
