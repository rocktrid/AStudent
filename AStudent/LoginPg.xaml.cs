using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AStudent
{
    /// <summary>
    /// Логика взаимодействия для LoginPg.xaml
    /// </summary>
    public partial class LoginPg : Page
    {
        public LoginPg()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Ниже, в методе ExecLogin_Click, реализована функция логина в программу по клику на соответствующую кнопку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecLogin_Click(object sender, RoutedEventArgs e)
        {
            string a = "admin";
            int b = a.GetHashCode();
            int c = PbLogText.Password.GetHashCode();
            try
            {
                if (TbLogText.Text == "admin" && c == b)
                {
                    ClassForFrames.NewFrames.Navigate(new ReestrPg());
                }
                else
                {
                    ///
                    /// Обработка последующего нажатия на enter после вывода на экран MessageBox
                    /// В ином случае, будет цикличное повторение вывода MessageBox
                    ///
                    if(System.Windows.Forms.MessageBox.Show("неверный логин или пароль") == System.Windows.Forms.DialogResult.OK)
                    {
                        TbLogText.Clear();
                        PbLogText.Clear();
                        Keyboard.ClearFocus();
                    }                                    
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка: "+ex);
            }          
        }
        /// <summary>
        /// метод ниже отслеживает нажатие клавиши Enter в PasswordBox и вместо перевода фокуса на элемент ниже
        /// метод реализует проверку по эвенту - эквивалент нажатию на кнопку, но без необходимости использования мышки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PbLogText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    ExecLogin_Click(this, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(""+ex);
            }
        }
    }
}
