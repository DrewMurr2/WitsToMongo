using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using witsReader3;
namespace CsharpWitsMongo
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient();
            var db = client.GetDatabase("Settings");
            var coll = db.GetCollection<WitsConnectionSettings>("WitsConnection");
            var settings = coll
                .Find(s => s.Baudrate == 9600)
                .Limit(2)
                .ToListAsync()
                .Result;
            Console.WriteLine("Settings:");
            foreach (var set in settings)
            {
                Console.WriteLine(" * " + set.Comport);
            }
            Console.ReadLine();
        }
    }
}
