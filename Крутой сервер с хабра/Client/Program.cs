using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Клиент";
            UniservClient client = new UniservClient();
            client.Host = "localhost";
            client.Port = 2000;
            client.Events.OnError = OnError;
            client.Events.OnBark = OnBark;

            Console.WriteLine("Подключение...");
            if (client.Connect(false))
            {
                Console.WriteLine("Подключено!");

                string[] users =  client.Common.GetAvailableUsers();

                string s = "qwerty";
                try
                {
                    Console.WriteLine("<- CutTheText(ref {0})", s);
                    client.Cat.CutTheText(ref s);  // Доступ запрещен
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Concat("-> \"", GetAllNestedMessages(ex), "\""));
                }

                try
                {
                    Console.WriteLine("<- ChangePrivileges(Tom, _password)");
                    client.Common.ChangePrivileges("Tom", "_password");  // логинимся
                }
                catch (Exception ex)
                {
                    // не прошли авторизацию
                    Console.WriteLine(string.Concat("-> \"", GetAllNestedMessages(ex), "\""));
                }

                Console.WriteLine("<- ChangePrivileges(Tom, TheCat)");
                client.Common.ChangePrivileges("Tom", "TheCat");  // логинимся

                Console.WriteLine("<- CutTheText(ref {0})", s);
                client.Cat.CutTheText(ref s);
                Console.WriteLine(@"-> ""{0}""", s);

                object o;
                try
                {
                    Console.WriteLine("<- TryFindObject");
                    client.Dog.TryFindObject(out o); // Доступ запрещен
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Concat("-> \"", GetAllNestedMessages(ex), "\""));
                }

                Console.WriteLine("<- ChangePrivileges(Dog1, _password)");
                client.Common.ChangePrivileges("Dog1", "_password");  // перелогиниваемся

                Console.WriteLine("<- TryFindObject");
                if (client.Dog.TryFindObject(out o))
                {
                    Console.WriteLine(@"-> Объект найден: ""{0}""", o);
                }

                try
                {
                    Console.WriteLine("<- CutTheText({0})", s);
                    client.Cat.CutTheText(ref s);  // Доступ запрещен
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Concat("-> \"", GetAllNestedMessages(ex), "\""));
                }

                Console.WriteLine("<- ChangePrivileges(Dog0, groovy!)");
                client.Common.ChangePrivileges("Dog0", "groovy!");  // перелогиниваемся

                Console.WriteLine("<- Bark(1)");
                client.Dog.Bark(1);
            }


            Console.WriteLine("Press any key to close");
            Console.ReadKey();
        }

        private static string GetAllNestedMessages(Exception ex)
        {
            string s = ex.Message;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                s += string.Concat(Environment.NewLine, ex.Message);
            }
            return s;
        }

        static void OnError(Exception ex)
        {
            Console.WriteLine(GetAllNestedMessages(ex));
        }

        static void OnBark(int nTimes)
        {
            for (int i = 0; i < nTimes; i++)
            {
                Console.WriteLine("-> Bark!");
            }
        }
    }
}
