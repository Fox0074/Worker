using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.Server
{
    public class Proxy<T> : RealProxy where T : class
    {
        User client;

        public Proxy(User client) : base(typeof(T))
        {
            this.client = client;
        }

        public override IMessage Invoke(IMessage msg)
        {
                IMethodCallMessage call = (IMethodCallMessage)msg;
                object[] parameters = call.Args;
                int OutArgsCount = call.MethodBase.GetParameters().Where(x => x.IsOut).Count();
            try
            {
                Unit result = client.Execute(call.MethodName, parameters);
                parameters = parameters.Select((x, index) => result.prms[index] ?? x).ToArray();
                return new ReturnMessage(result.ReturnValue, parameters, OutArgsCount, call.LogicalCallContext, call);
            }
            catch(Exception ex)
            {
                Log.Send(client.UserType + ", ip: " + client.EndPoint + " Передал исключение: " +
                    ex.Message);
                IMessage t = msg;
                return new ReturnMessage(new Unit("",new object[] { }), parameters, OutArgsCount, call.LogicalCallContext, call); ;
                //return new ReturnMessage(ex, call);
            }
        }
    }
}
