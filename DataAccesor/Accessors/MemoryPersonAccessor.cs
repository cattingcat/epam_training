using System.Collections.Generic;
using System.Linq;
using DataAccessor.Entity;
using DataAccessor.Data;

namespace DataAccessor.Accessors
{
    class MemoryPersonAccessor: IPersonAccessor
    {
        public ICollection<Person> GetAll()
        {
            return MemoryPersonDB.PersonDB;
        }

        public Entity.Person GetById(int id)
        {
            var res = from p in MemoryPersonDB.PersonDB where p.ID == id select p;
            return res.First<Person>();
        }

        public void DeleteById(int id)
        {
            var res = from p in MemoryPersonDB.PersonDB where p.ID == id select p;
            MemoryPersonDB.PersonDB.Remove(res.First<Person>());
        }

        public void Insert(Person p)
        {
            MemoryPersonDB.PersonDB.Add(p);
        }

        public void Dispose()
        {
            
        }
    }
}
