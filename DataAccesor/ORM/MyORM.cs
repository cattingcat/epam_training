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

        public MyORM(DbProviderFactory factory, string connectionString, params Type[] types)
        {
            mappingPool = new Dictionary<Type, OrmMap>();
            this.factory = factory;
            this.connectionString = connectionString;
            foreach (Type type in types)
            {
                RegisterType(type);
            }
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

            using (DbConnection connection = GetOpenConnection())
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = map.BuildSelectAllQuery();

                DbDataReader reader = command.ExecuteReader();
                DbReaderAdapter adapter = new DbReaderAdapter(reader, map);
                return adapter.GetMultipleResult<T>();
            }
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
                if (map.Relations.Count != 0)
                {
                    ProcessRelation(map, result, connection);
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

        private void ProcessRelation(OrmMap containerMap, object containerObj, DbConnection connection)
        {
            foreach (string relation in containerMap.Relations)
            {
                PropertyInfo p = containerMap[relation, ColumnType.Relation];
                RelationAttribute attr = p.GetCustomAttribute<RelationAttribute>();
                if (attr.Type == RelationType.One)
                {
                    string secondTable = attr.SecondTable;
                    string secondColumn = attr.SecondColumn;
                    string thisColumn = attr.ThisColumn;

                    OrmMap innerMap = (from m in mappingPool.Values where m.TableName == secondTable select m).First<OrmMap>();

                    object containerId = containerMap.GetId(containerObj);
                    // SELECT person_id FROM PhoneTbl WHERE id = parentId
                    // sub query
                    StringBuilder builder = new StringBuilder();
                    builder.Append("(SELECT ");
                    builder.Append(thisColumn);
                    builder.Append(" FROM ");
                    builder.Append(containerMap.TableName);
                    builder.Append(" WHERE ");
                    builder.Append(containerMap.ID);
                    builder.Append(" = @relationVal )");


                    string whereStatement = String.Format("{0}={1}", secondColumn ?? innerMap.ID, builder.ToString());

                    string selectQuery = innerMap.BuildSelectWhereQuery(whereStatement);

                    DbCommand command = connection.CreateCommand();
                    command.CommandText = selectQuery;

                    DbParameter param = command.CreateParameter();
                    param.DbType = innerMap.GetDbType(innerMap.ID);
                    param.ParameterName = "@relationVal";
                    param.Value = containerId;
                    command.Parameters.Add(param);

                    DbDataReader reader = command.ExecuteReader();
                    DbReaderAdapter adapter = new DbReaderAdapter(reader, innerMap);

                    object innerObj = adapter.GetSingleResult();

                    p.SetValue(containerObj, innerObj);
                }
                else if (attr.Type == RelationType.Many)
                {

                }
            }
        }
    }
}
