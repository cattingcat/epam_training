using DataAccessor.Data;
using DataAccessor.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace DataAccessor.Accessors
{
    class FilePersonAccessor: IPersonAccessor
    {
        private static XmlSerializer PersonArraySerializer = new XmlSerializer(typeof(List<Person>), new[] { typeof(Person) });

        private ICollection<Person> data;
        private string fileName;

        public FilePersonAccessor()
        {
            fileName = @"Data\FilePersonDB.txt";
            data = DeserializeCollection();
        }

        public ICollection<Person> GetAll()
        {
            return data;
        }
        public Person GetById(int id)
        {
            var res = from p in data where p.ID == id select p;
            return res.First<Person>();
        }
        public void DeleteById(int id)
        {
            var res = from p in data where p.ID == id select p;
            data.Remove(res.First<Person>());            
        }
        public void Insert(Person p)
        {
            data.Add(p);            
        }
        public void Dispose()
        {
            SerializeCollection(data);
        }        

        private void SerializeCollection(ICollection<Person> collection)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                PersonArraySerializer.Serialize(sw, collection.ToList<Person>());
            }
        }
        private ICollection<Person> DeserializeCollection()
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                return (List<Person>)PersonArraySerializer.Deserialize(sr);
            }
        }
    }
}
