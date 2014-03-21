using DataAccessor.Accessors;
using DataAccessor.Entity;
using System;

namespace DataAccessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Commands:\np - print all\np [id] - print one person\ni [id] - insert person with id\nd [id] - delete by id\nx - quit");
            using (IPersonAccessor accessor = new DirectoryPersonAccessor())
            {
                Console.WriteLine("Now using: {0} ", accessor.GetType().Name);
                while (true)
                {
                    string[] command = Console.ReadLine().Split(' ', ',');
                    if (command[0] == "p")
                    {
                        if (command.Length == 1)
                        {
                            foreach (Person p in accessor.GetAll())
                            {
                                Console.WriteLine(p);
                            }
                        }
                        else if (command.Length == 2)
                        {
                            int id = Int32.Parse(command[1]);
                            Person p = accessor.GetById(id);
                            Console.WriteLine(p.ToString());
                        }
                    }
                    else if (command[0] == "d")
                    {
                        int id = Int32.Parse(command[1]);
                        accessor.DeleteById(id);
                    }
                    else if (command[0] == "i")
                    {
                        int id = Int32.Parse(command[1]);
                        Person p = new Person() { ID = id };
                        accessor.Insert(p);
                    }
                    else if (command[0] == "x")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Unknown command");
                    }
                }
            }   
        }
    }
}
