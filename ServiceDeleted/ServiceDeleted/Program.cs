using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeleted
{
    class Program
    {
        public static List<string> services = new List<string> {
            "MicrosoftServiceUpdaterr",
            "MicroUpdater"
        };

        static void Main(string[] args)
        {      

            int number = -1;
            string serviceName;
            Console.WriteLine("Выберите службу для удаления :");
            Console.WriteLine("0 Для удаления службы по имени");

            for (int i = 0; i< services.Count;i++)
            {
                Console.WriteLine((i+1) + ": " + services[i]);
            }

            while (true)
            {
                try
                {
                    number = int.Parse(Console.ReadLine());
                    number--;

                    if (number==-1)
                    {
                        break;
                    }

                    if ((number >= 0) && (number < services.Count))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Такого варианта нет в списке, повторите ввод");
                        continue;
                    }                      
                } catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + " Повторите ввод");
                }
            }

            if (number == -1)
            {
                Console.WriteLine("Введите имя службы");
                serviceName = Console.ReadLine();
                DeleteService(serviceName);
            }
            else
            {
                    DeleteService(services[number]);
            }

            Console.ReadLine();
        }

        private static void DeleteService(string name)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.Verb = "runas";
                proc.StartInfo.Arguments = "/C " + "Sc delete " + name;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.Start();

                Console.WriteLine("Служба была удалена");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
