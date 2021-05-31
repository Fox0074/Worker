using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ServerWorker.ConsoleView
{
    public class ConsoleView
    {
        public Action OnProgramClose = delegate { };
        private List<string> _commandsLog = new List<string>();
        private int _positionCommandLog;
        private class CommandRequest
        {
            public string Source;
            public string Command;
            public List<object> ObjectParametrs = new List<object>();
            public List<string> StringParametrs = new List<string>();

            public CommandRequest(string stringRequest)
            {
                Source = stringRequest;
                StringParametrs.AddRange(stringRequest.Split(' ').ToList());
                //TODO: привидение типов
                ObjectParametrs.AddRange(stringRequest.Split(' ').ToList());
                Command = StringParametrs[0];
                StringParametrs.RemoveAt(0);
                ObjectParametrs.RemoveAt(0);
            }
        }
        private class CommandResult
        {
            public bool Success;
            public object ReturnValue;
        }

        private ConsoleViewCommands _commands;

        public ConsoleView()
        {
            _commands = new ConsoleViewCommands();
            _positionCommandLog = _commandsLog.Count-1;
            AppDomain.CurrentDomain.ProcessExit += (arg1,arg2) => OnProgramClose.Invoke();
        }

        public void CommandLineThread()
        {
            CommandRequest commandRequest;
            do
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Command >> ");
                Console.ForegroundColor = ConsoleColor.White;
                commandRequest = new CommandRequest(SymbolycInputRead("Command >> ", ConsoleColor.Red));

                if (!string.IsNullOrWhiteSpace(commandRequest.Command)) 
                {
                    _commandsLog.Add(commandRequest.Source);
                    _positionCommandLog++;
                }

                var aviableMethods = _commands.GetType().GetMethods().ToList();

                aviableMethods.ForEach(x => 
                commandRequest.Command = x.Name.ToLower() == commandRequest.Command.ToLower() ? 
                x.Name : commandRequest.Command);

                var result = ProcessMessage(
                    commandRequest.Command, 
                    commandRequest.ObjectParametrs.Count > 0 ? commandRequest.ObjectParametrs.ToArray() : null, 
                    _commands);

                if (result.Success)
                {
                    if (result.ReturnValue != null) Console.WriteLine("Результат:\n" + result.ReturnValue);
                } 
                else
                {
                    Console.WriteLine("Команда не выполнена");
                }

               
            } while (commandRequest.Command.ToLower() != "exit");
        }

        private string SymbolycInputRead(string startInput, System.ConsoleColor startMessageColor)
        {
            string inputResult = "";
            ConsoleKeyInfo inputKey;

            var startCursorPosX = Console.CursorLeft;    
            do
            {
                var currentCursorPosX = Console.CursorLeft;    
                var currentCursorPosY = Console.CursorTop;    
                inputKey = Console.ReadKey();
                if (inputKey == null) continue;
                    switch(inputKey.Key)
                    {
                        case ConsoleKey.Enter:
                            continue;
                        case ConsoleKey.Backspace:
                            if (inputResult.Length > 0)
                            {
                                inputResult = inputResult.Length > 0 ? inputResult.Remove(inputResult.Length - 1,1) : inputResult;
                            } else
                                Console.SetCursorPosition(currentCursorPosX, Console.CursorTop);
                            break;
                        case ConsoleKey.UpArrow:
                            if (_positionCommandLog > 0)
                            {
                                _positionCommandLog--;

                                Console.SetCursorPosition(startCursorPosX ,Console.CursorTop);
                                for (int i = startCursorPosX; i< Console.WindowWidth;i++) Console.Write(" ");
                                Console.SetCursorPosition(startCursorPosX ,Console.CursorTop);

                                inputResult = _commandsLog[_positionCommandLog];
                                Console.Write(inputResult);
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            if (_positionCommandLog < _commandsLog.Count-1)
                            {
                                _positionCommandLog++;

                                Console.SetCursorPosition(startCursorPosX ,Console.CursorTop);
                                for (int i = startCursorPosX; i< Console.WindowWidth;i++) Console.Write(" ");
                                Console.SetCursorPosition(startCursorPosX ,Console.CursorTop);

                                inputResult = _commandsLog[_positionCommandLog];
                                Console.Write(inputResult);
                            }
                            break;
                        case ConsoleKey.Tab:
                                MethodInfo[] methods = _commands.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                                var searchResults = methods.Select(x => x.Name).Where(x => x.ToLower().Contains(inputResult)).ToList();
                                if (searchResults.Count > 0)
                                {
                                    if (searchResults.Count == 1 && searchResults[0] != inputResult) 
                                    {
                                        Console.SetCursorPosition(currentCursorPosX - inputResult.Length,Console.CursorTop);
                                        inputResult = searchResults[0]; 
                                        Console.Write(inputResult);
                                    }
                                    else
                                    {
                                        Console.WriteLine();
                                        foreach(var result in searchResults) 
                                        {
                                            Console.WriteLine(result);
                                        } 

                                        Console.ForegroundColor = startMessageColor;             
                                        Console.Write(startInput); 
                                        Console.ForegroundColor = ConsoleColor.White; 

                                        Console.Write(inputResult);          
                                    }
                                }
                                else
                                    Console.SetCursorPosition(currentCursorPosX, Console.CursorTop);
                            break;
                        default:
                            inputResult += inputKey.KeyChar;
                        break;
                    }
            } while(inputKey.Key != ConsoleKey.Enter);

            _positionCommandLog = Math.Max(0,_commandsLog.Count-1);
            Console.SetCursorPosition(Console.CursorLeft,Console.WindowHeight-1);
            return inputResult;
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
