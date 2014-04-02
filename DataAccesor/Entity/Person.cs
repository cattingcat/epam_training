using DataAccessor.ORM.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DataAccessor.Entity
{
    [Serializable]
    [Table(TableName = "PersonTable")]
    public class Person
    {

        [Id(ColumnName = "identificator", ColumnType=DbType.Int32)]
        public int ID { get; set; }
        [Column(ColumnName = "NameColumn", ColumnType=DbType.String)]
        public string Name { get; set; }
        [Column(ColumnName = "LastNameColumn", ColumnType=DbType.String)]
        public string LastName { get; set; }
        [Column(ColumnName= "dob", ColumnType=DbType.Date)]
        public DateTime DayOfBirth { get; set; }

        [Many(SecondTable = "PhoneTbl", SecondColumn = "person_id")]
        public ICollection<Phone> Phones { get; set; }

        public override string ToString()
        {
            return String.Format("id: {0}, name: {1}, lastname: {2}, dyaOfBirth: {3}, Phones: {4}", 
                ID, Name.Trim(), LastName.Trim(), DayOfBirth, Phones.Count);
        }
    }
}
