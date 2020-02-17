using System;
using System.Linq;
using System.Reflection;

namespace ServerWorker.ConsoleView
{
    public class ConsoleView
    {
        public Action OnProgramClose = delegate { };
        private class CommandResult
        {
            public bool Success;
            public object ReturnValue;
        }

        private ConsoleViewCommands _commands;

        public ConsoleView()
        {
            _commands = new ConsoleViewCommands();
            AppDomain.CurrentDomain.ProcessExit += (arg1,arg2) => OnProgramClose.Invoke();
        }

        public void CommandLineThread()
        {
            string command;
            do
            {
                Console.Write("Command >> ");
                command = Console.ReadLine();

                var result = ProcessMessage(command, null, _commands);

                if (result.Success)
                {
                    if (result.ReturnValue != null) Console.WriteLine("Результат:\n" + result.ReturnValue);
                } 
                else
                {
                    Console.WriteLine("Команда не выполнена");
                }

               
            } while (command.ToLower() != "exit");
        }

        private CommandResult ProcessMessage(string MethodName, object[] parametrs, object classInstance)
        {
            CommandResult result = new CommandResult();
            // ищем запрошенный метод
            MethodInfo method = _commands.GetType().GetMethod(MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            try
            {
                if (method == null)
                {
                    Console.WriteLine(string.Concat("Execute -> ", MethodName, "(", string.Join(", ", parametrs), ")"));
                    throw new Exception(string.Concat("Метод \"", MethodName, "\" недоступен"));
                }

                try
                {
                    // выполняем метод интерфейса
                    result.ReturnValue = method.Invoke(classInstance, parametrs);
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }


                // возвращаем ref и out параметры
                parametrs = method.GetParameters().Select(x => x.ParameterType.IsByRef ? parametrs[x.Position] : null).ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error >> " + ex.Message);
                result.Success = false;
                return result;
            }

            result.Success = true;
            return result;
        }


    }
}
