using DataAccessor.ORM.Attributes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataAccessor.ORM
{
    public class MyORM
    {
        private IDictionary<Type, OrmMap> mappingPool;
        private DbProviderFactory factory;
        private string connectionString;
        public bool RelationsEnabled { get; set; }

        public MyORM(DbProviderFactory factory, string connectionString, params Type[] types)
        {
            mappingPool = new Dictionary<Type, OrmMap>();
            this.factory = factory;
            this.connectionString = connectionString;
            foreach (Type type in types)
            {
                RegisterType(type);
            }
            RelationsEnabled = true;
        }

        public void RegisterType(Type type)
        {
            OrmMap map = null;
            if (!mappingPool.ContainsKey(type))
            {
                map = OrmMap.FromType(type);
                mappingPool[type] = map;
            }
            else
            {
                map = mappingPool[type];
            }
        }

        public ICollection<T> SelectAll<T>() where T : class, new()
        {
            OrmMap map = mappingPool[typeof(T)];
            string selectQuery = map.BuildSelectAllQuery();
            ICollection<T> result = null;
            using (DbConnection connection = GetOpenConnection())
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = map.BuildSelectAllQuery();
                
                using (DbDataReader reader = command.ExecuteReader())
                {
                    DbReaderAdapter adapter = new DbReaderAdapter(reader, map);
                    result = adapter.GetMultipleResult<T>();
                }
                if (RelationsEnabled)
                {
                    foreach(T res in result)
                    {
                        PerformSelectRelation(map, res, connection);
                    }                    
                }
            }
            return result;
        }
        public T SelectById<T>(object id) where T: class, new()
        {            
            OrmMap map = mappingPool[typeof(T)];            
            string whereStatement = String.Format("{0}=@id", map.ID);
            string selectQuery = map.BuildSelectWhereQuery(whereStatement);        

            using (DbConnection connection = GetOpenConnection())
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = selectQuery;

                DbParameter param = command.CreateParameter();
                param.DbType = map.GetDbType(map.ID);
                param.ParameterName = "@id";
                param.Value = id;
                command.Parameters.Add(param);

                T result = null;
                using (DbDataReader reader = command.ExecuteReader())
                {
                    DbReaderAdapter adapter = new DbReaderAdapter(reader, map);
                    result = adapter.GetSingleResult<T>();
                }
                if (RelationsEnabled)
                {
                    PerformSelectRelation(map, result, connection);
                }
                return result;
            }
        }
        public int Insert(object o)
        {
            Type type = o.GetType();
            OrmMap map = mappingPool[type];            
            StringBuilder argListBuilder = new StringBuilder();
            List<KeyValuePair<string, string>> colNameToNamedParam = new List<KeyValuePair<string, string>>();

            foreach (string col in map.Columns)
            {
                string namedParam = '@' + col;
                colNameToNamedParam.Add(new KeyValuePair<string, string>(col, namedParam));
                argListBuilder.Append(namedParam);
                argListBuilder.Append(',');
            }
            argListBuilder.Remove(argListBuilder.Length - 1, 1);

            string insertQuery = map.BuildInsertQuery(argListBuilder.ToString());

            using (DbConnection connection = GetOpenConnection())
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = insertQuery;
                foreach (var pair in colNameToNamedParam)
                {
                    DbParameter param = command.CreateParameter();
                    param.DbType = map.GetDbType(pair.Key);
                    param.ParameterName = pair.Value;
                    param.Value = map[pair.Key].GetValue(o) ?? DBNull.Value;                    
                    command.Parameters.Add(param);
                }
                try
                {
                    return command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
        }
        public int Delete<T>(object id) where T : class, new()
        {
            OrmMap map = mappingPool[typeof(T)];            
            string whereStatement = String.Format("{0}=@id", map.ID);

            string deleteQuery = map.BuildDeleteQuery(whereStatement);

            using (DbConnection connection = GetOpenConnection())
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = deleteQuery;

                DbParameter param = command.CreateParameter();
                param.DbType = map.GetDbType(map.ID);
                param.ParameterName = "@id";
                param.Value = id;
                command.Parameters.Add(param);

                return command.ExecuteNonQuery();
            }
        }

        private DbConnection GetOpenConnection()
        {
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }

        private void PerformSelectRelation(OrmMap containerMap, object containerObj, DbConnection connection)
        {
            foreach (string relation in containerMap.Relations)
            {
                PropertyInfo p = containerMap[relation, ColumnType.Relation];
                RelationAttribute attr = p.GetCustomAttribute<RelationAttribute>();

                string secondTable = attr.SecondTable;
                string secondColumn = attr.SecondColumn;
                string thisColumn = attr.ThisColumn;
                OrmMap innerMap = (from m in mappingPool.Values where m.TableName == secondTable select m).First<OrmMap>();
                object containerId = containerMap.GetId(containerObj);

                if (attr.Type == RelationType.One)
                {                 
                    // SELECT person_id FROM PhoneTbl WHERE id = parentId
                    // sub query
                    string subQueryWhereSection = containerMap.ID + " = @relationVal";
                    string subQuery = containerMap.BuildSubSelectQuery(thisColumn, subQueryWhereSection);

                    string whereStatement = String.Format("{0}={1}", secondColumn ?? innerMap.ID, subQuery);
                    string selectQuery = innerMap.BuildSelectWhereQuery(whereStatement);

                    DbCommand command = connection.CreateCommand();
                    command.CommandText = selectQuery;

                    DbParameter param = command.CreateParameter();
                    param.DbType = innerMap.GetDbType(innerMap.ID);
                    param.ParameterName = "@relationVal";
                    param.Value = containerId;
                    command.Parameters.Add(param);

                    object innerObj = null;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        DbReaderAdapter adapter = new DbReaderAdapter(reader, innerMap);
                        innerObj = adapter.GetSingleResult();
                    }
                    p.SetValue(containerObj, innerObj);
                }
                else if (attr.Type == RelationType.Many)
                {
                    string whereSection = String.Format("{0} = @relationVal", secondColumn);
                    string selectQuery = innerMap.BuildSelectWhereQuery(whereSection);

                    DbCommand command = connection.CreateCommand();
                    command.CommandText = selectQuery;

                    DbParameter param = command.CreateParameter();
                    param.DbType = containerMap.GetDbType(containerMap.ID);
                    param.ParameterName = "@relationVal";
                    param.Value = containerId;
                    command.Parameters.Add(param);

                    ICollection<object> innerCollection = null;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        DbReaderAdapter adapter = new DbReaderAdapter(reader, innerMap);
                        innerCollection = adapter.GetMultipleResult();
                    }
                    foreach (object o in innerCollection)
                    {
                        PerformSelectRelation(innerMap, o, connection);
                    }

                    // // // // // // // // // // // //
                    Type t = typeof(List<>).MakeGenericType(innerMap.ObjectType);
                    dynamic collection = Activator.CreateInstance(t);
                    foreach (object o in innerCollection)
                    {
                        dynamic _do = Convert.ChangeType(o, innerMap.ObjectType);
                        collection.Add(_do);
                    }
                    p.SetValue(containerObj, collection);

                    // code with exception
                    //p.SetValue(containerObj, Convert.ChangeType(innerCollection, p.PropertyType));
                }
            }
        }
    }
}
