using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerWorker.Server
{
    public static class ProcessMessages
    {
        public static void GuideMessage(Unit unit, User user)
        {
            if (unit.EndPoint != null)
            {
                var endUser = ServerNet.ConnectedUsers.ToArray().FirstOrDefault(x => x.EndPoint == unit.EndPoint);
                 if (!unit.IsSync)
                 {
                     ServerNet.SendMessage(endUser.nStream, unit);
                 }
            }

            if (unit.IsDelegate)
            {
                ExecuteDelegate(unit);
                return;
            }

            if (unit.IsAnswer)
            {
                if (unit.IsSync)
                {  // получен результат синхронной процедуры
                    user.SyncResult(unit);
                }
                else
                {
                    //TODO: реализовать асинхронные вызовы
                    //Ошибка выполнения асинхронного вызова
                    Log.Send(user.UserType.ToString() + " " + user.EndPoint + " <- " + unit.Command + " " + unit.Exception);                
                }
            }
            else
            {        
                    ProcessMessage(unit, user);
            }
        }

        private static void ExecuteDelegate(Unit unit)
        {
            // асинхронный вызов события
            try
            {
                // ищем соответствующий Action
                var pi = typeof(IEvents).GetProperty(unit.Command, BindingFlags.Instance | BindingFlags.Public);
                if (pi == null) throw new Exception(string.Concat("Свойство \"", unit.Command, "\" не найдено"));
                var delegateRef = pi.GetValue(Program.server, null) as Delegate;

                // инициализируем событие
                if (delegateRef != null) ThreadPool.QueueUserWorkItem(state => delegateRef.DynamicInvoke(unit.prms));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat("Не удалось выполнить делегат \"", unit.Command, "\""), ex);
            }
        }

        private static void ProcessMessage(Unit unit, User user)
        {
            string MethodName = unit.Command;
            if (MethodName == "OnPing") return;

            // ищем запрошенный метод в кольце текущего уровня
            MethodInfo method = user.RingType.GetMethod(MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            try
            {
                if (method == null)
                {
                    Log.Send(string.Concat(user.UserType.ToString(), " -> ", MethodName, "(", string.Join(", ", unit.prms), ")"));
                    throw new Exception(string.Concat("Метод \"", MethodName, "\" недоступен"));
                }

                UserType startUserType = user.UserType;
                try
                {
                    // выполняем метод интерфейса
                    unit.ReturnValue = method.Invoke(user.ClassInstance, unit.prms);
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }

                if (Program.authSystem.sessionLoginData.userType == UserType.System || MethodName != "ChangePrivileges")
                {
                    Log.Send(string.Concat(startUserType.ToString(), " ", user.EndPoint, " -> ", MethodName, "(", string.Join(", ", unit.prms), ")"));
                }
                else
                {
                    Log.Send(string.Concat(startUserType.ToString(), " ", user.EndPoint, " -> ", MethodName, "(Login, Pass)"));
                }

                // возвращаем ref и out параметры
                unit.prms = method.GetParameters().Select(x => x.ParameterType.IsByRef ? unit.prms[x.Position] : null).ToArray();
            }
            catch (Exception ex)
            {
                unit.Exception = ex;
            }
            finally
            {
                if (unit.IsSync)
                {
                    // возвращаем результат выполнения запроса
                    unit.IsAnswer = true;
                    ServerNet.SendMessage(user.nStream, unit);
                }
                else
                {
                    if (unit.Exception != null)
                    {
                        unit.IsAnswer = true;
                        ServerNet.SendMessage(user.nStream,unit);
                        Log.Send("Отправлена ошибка: " + unit.Exception);
                    }
                }
            }
        }
    }
}
