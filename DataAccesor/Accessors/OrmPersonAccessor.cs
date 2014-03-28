using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using DataAccessor.Entity;
using DataAccessor.ORM;

namespace DataAccessor.Accessors
{
    public class OrmPersonAccessor: IAccessor<Person>
    {
        private MyORM orm;

        public OrmPersonAccessor()
        {
            string connectionString = 
                @"Data Source=(LocalDB)\v11.0;AttachDbFilename=" +
                @"C:\Users\pc-1\Desktop\projects\EpamTraining\DataAccesor\Data\Database.mdf" +
                @";Integrated Security=False";
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SqlClient");

            orm = new MyORM(factory, connectionString, typeof(Person));
        }

        public ICollection<Person> GetAll()
        {
            return orm.SelectAll<Person>();
        }

        public Person GetById(int id)
        {
            return orm.SelectById<Person>(id);
        }

        public void DeleteById(int id)
        {
            orm.Delete<Person>(id);
        }

        public void Insert(Person p)
        {
            orm.Insert(p);
        }
    }
}
