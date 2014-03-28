using DataAccessor.ORM.Attributes;
using System;
using System.Data;

namespace DataAccessor.Entity
{
    [Table(TableName = "SimpleTable")]
    class SecondEntity
    {
        [Id(ColumnName = "Id", ColumnType = DbType.Int32)]
        public int Field { get; set; }

        [Column(ColumnName = "NameColumn", ColumnType = DbType.String)]
        public string UnattributeField { get; set; }

        public override string ToString()
        {
            return String.Format("ID: {0}, Value: {1}", Field, UnattributeField);
        }
    }
}
