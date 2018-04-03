using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerWorker
{
    public class AuthInProgram
    {
        public string Login { get; private set; } = "Worker";
        public string Pass { get; private set; } = "hex34";
        public bool IsAuthorizate { get; set; } = false;

        public bool Authorization (string login, string pass)
        {
            bool result = false;

            if ((login==Login)&&(pass==Pass))
            {
                result = true;
                IsAuthorizate = true;
            }
            else
            {
                MessageBox.Show("Логин и/или пароль введены неверно", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return result;
        }
    }
}
