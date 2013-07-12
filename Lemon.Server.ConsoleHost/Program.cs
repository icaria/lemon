using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lemon.Base;
using log4net.Config;

namespace Lemon.Server.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer<DataContext>(new DropCreateDatabaseAlways<DataContext>());
            using (var db = new DataContext())
            {
                db.Database.Initialize(false);
            }

            XmlConfigurator.Configure();

            var service = new Lemon.Server.LemonAppServer();
            service.Start();
            service.Connect("Lemon");
            Console.WriteLine("Service started");
            Console.WriteLine();

            Console.WriteLine("Press <return> to close service");
            Console.ReadLine();
            service.Stop();

            Console.WriteLine();
            Console.WriteLine("Service stopped");
            Console.WriteLine();
            Console.WriteLine("Press <return> to exit");
            Console.ReadLine();

        }
    }
}
