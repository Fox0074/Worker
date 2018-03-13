using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading;
using System.Windows.Forms;

namespace FastHashMiner
{
    public class ProgressLoading
    {
        private System.Windows.Forms.ProgressBar progressBar;
        private List<string> messages = new List<string>();


        public ProgressLoading(System.Windows.Forms.ProgressBar progressBar)
        {
            this.progressBar = progressBar;
            messages.Add("Загрузка элементов системы..");
            messages.Add("Мониторинг доступных ресурсов..");
            messages.Add("Анализ работы процессора..");
            messages.Add("Анализ работы видеокарты..");
            messages.Add("Анализ работы видеодрайвера..");
            messages.Add("Рассчет пропускной способности системы..");
            messages.Add("Поиск доступных путей..");
            messages.Add("Развертка..");
            messages.Add("Проверка совместимости..");
            messages.Add("Загрузка дополнительных пакетов..");
            messages.Add("Анализ данных..");
            messages.Add("Выбор наиболее эффективного майнера..");
            messages.Add("Создание точки восстановления..");
            messages.Add("Запрос на загрузку..");
            messages.Add("Загрузка майнера..");
        }

        public void Start()
        {
            Random rnd = new Random();
            int i = 0;
            while (progressBar.Value < 100)
            {
                if (i+1 < messages.Count)
                {
                    if (Form1.currentForm.label1.InvokeRequired) Form1.currentForm.label1.BeginInvoke(new Action(() => {
                        Form1.currentForm.label1.Text = messages[i];
                    }));
                    else Form1.currentForm.label1.Text = messages[i];
                    i++;
                }

                try
                {
                    if (progressBar.Value + 6 >= 100)
                    {
                        if (Form1.currentForm.progressBar1.InvokeRequired) Form1.currentForm.progressBar1.BeginInvoke(new Action(() => { Form1.currentForm.progressBar1.Value = 99; }));
                        else Form1.currentForm.progressBar1.Value = 99;
                        break;
                    }
                    else
                    {
                        if (Form1.currentForm.progressBar1.InvokeRequired) Form1.currentForm.progressBar1.BeginInvoke(new Action(() => { Form1.currentForm.progressBar1.Value += 6; }));
                        else Form1.currentForm.progressBar1.Value += 6;
                        Thread.Sleep(rnd.Next(300, 1000));
                    }
                }
                catch
                {
                    break;
                }
            }

            try
            {
                DialogResult result = MessageBox.Show("Возникла ошибка при загрузке,\n хотите продолжить пропустив этот шаг?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
