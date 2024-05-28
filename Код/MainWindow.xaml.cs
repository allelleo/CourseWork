using Microsoft.VisualBasic.ApplicationServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        YourDbContext context = new YourDbContext();

        if (EmailInp.Text.Length > 0)
        {
            if (PasswordInp.Password.ToString().Length > 0)
            {
                var user = context.Users.FirstOrDefault(u => u.Email == EmailInp.Text && u.Password == PasswordInp.Password.ToString());
                if (user != null)
                {
                    if (user.IsAdmin)
                    {



                        View.DataBaseEditor Page = new View.DataBaseEditor(user.UserID);
                        this.Hide(); // Скрываем текущую форму LoginPage
                        Page.ShowDialog(); // Отображаем форму HomePage и блокируем вызывающий поток
                        this.Close(); // После закрытия формы HomePage закрываем текущую форму LoginPage

                    }
                    else
                    {
                        MessageBox.Show("Пользователь не администратор");
                        //Clear();
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь не найден");
                    //PasswordClear();

                }
            }
            else
            {
                MessageBox.Show("Введите пароль!");
            }
        }
        else
        {
            MessageBox.Show("Введите логин!");
        }
    }
}