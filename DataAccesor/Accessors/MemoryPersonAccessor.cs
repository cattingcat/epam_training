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
            Person exPerson = res.FirstOrDefault<Person>();
            if (exPerson != null)
            {
                MemoryPersonDB.PersonDB.Remove(exPerson);
            }
        }

        public void Insert(Person p)
        {
            var tmp = from ep in MemoryPersonDB.PersonDB where ep.ID == p.ID select ep;
            Person existPerson = tmp.FirstOrDefault<Person>();
            if (existPerson != null)
            {
                MemoryPersonDB.PersonDB.Remove(existPerson);
            }
            MemoryPersonDB.PersonDB.Add(p);            
        }

        public void Dispose()
        {
            
        }
    }
}
