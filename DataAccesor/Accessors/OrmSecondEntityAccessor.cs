using DataAccessor.Entity;
using DataAccessor.ORM;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessor.Accessors
{
    class OrmSecondEntityAccessor: IAccessor<SecondEntity>
    {
        private MyORM orm;

        public OrmSecondEntityAccessor()
        {
            string connectionString = 
                @"Data Source=(LocalDB)\v11.0;AttachDbFilename=" +
                @"C:\Users\pc-1\Desktop\projects\EpamTraining\DataAccesor\Data\Database.mdf" +
                @";Integrated Security=False";
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SqlClient");

            orm = new MyORM(factory, connectionString, typeof(SecondEntity));
        }

        public ICollection<SecondEntity> GetAll()
        {
            return orm.SelectAll<SecondEntity>();
        }

        public SecondEntity GetById(object id)
        {
            return orm.SelectById<SecondEntity>(id);
        }

        public void DeleteById(object id)
        {
            orm.Delete<SecondEntity>(id);
        }

        public void Insert(SecondEntity p)
        {
            orm.Insert(p);
        }
    }
}
