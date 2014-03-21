using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessor.Entity
{
    [Serializable]
    public class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime DayOfBirth { get; set; }

        public override string ToString()
        {
            return String.Format("id: {0}, name: {1}, lastname: {2}, dyaOfBirth: {3}", ID, Name, LastName, DayOfBirth);
        }
    }
}
