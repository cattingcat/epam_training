using DataAccessor.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessor.Accessors
{
    interface IPersonAccessor: IDisposable
    {
        ICollection<Person> GetAll();
        Person GetById(int id);
        void DeleteById(int id);
        void Insert(Person p);
    }
}
