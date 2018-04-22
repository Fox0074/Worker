using ServerWorker.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.UsersRank
{
    public class Ring2 : Ring
    {
        public Ring2(User u) : base(u)
        {

        }

        public string[] GetAvailableUsers()
        {
            return new string[] { "Dog0", "Dog1", "Tom" };
        }

        public void ChangePrivileges(string Animal, string password)
        {
            switch (Animal)
            {
                case "Dog0":
                    if (password != "groovy!") throw new Exception("Не верный пароль");
                    up.ClassInstance = new Dog_Ring0(up);
                    break;
                case "Dog1":
                    if (password != "_password") throw new Exception("Не верный пароль");
                    up.ClassInstance = new Dog_Ring1(up);
                    break;
                case "Tom":
                    if (password != "TheCat") throw new Exception("Не верный пароль");
                    up.ClassInstance = new Cat_Ring0(up);
                    break;
                default:
                    throw new Exception("Такого пользователя не существует");
            }
        }
    }
}
