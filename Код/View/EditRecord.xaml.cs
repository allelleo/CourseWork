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
using System.Windows.Shapes;

namespace Project.View
{
    /// <summary>
    /// Логика взаимодействия для EditRecord.xaml
    /// </summary>
    /// 
    public partial class EditRecord : Window
    {
        private List<string> tableColumns = new List<string>();
        private List<Type> tableColumnsType = new List<Type>();
        private DataBaseUtils DBUtils = new DataBaseUtils();
        private YourDbContext context = new YourDbContext();
        public EditRecord(string tableName, int recordID, List<object>  values)
        {
            InitializeComponent();
            int inputIndex = 0;

            Dictionary<string, Type> columns = DBUtils.GetTableColumns(tableName);

            bool flag = true;
            List<object> inputFieldsList = new List<object>();
            int count = 1;
            foreach (var column in columns)
            {
                if (flag)
                {
                    flag = !flag;
                    continue;
                }
                
                MainGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(30)
                }); // Создаем одну строку для всех кнопок
                if (column.Value == typeof(string) && column.Key != null)
                {
                    Label label = new Label();
                    label.Content = column.Key;
                    tableColumns.Add(column.Key);
                    tableColumnsType.Add(column.Value);
                    TextBox textBox = new TextBox();
                    textBox.Width = 200;
                    textBox.Text = values[count].ToString();
                    MainGrid.Children.Add(label);
                    MainGrid.Children.Add(textBox);
                    Grid.SetRow(label, inputIndex); // Установка строки для Label (если используется Grid)
                    Grid.SetColumn(label, 0); // Установка столбца для Label (если используется Grid)
                    Grid.SetRow(textBox, inputIndex); // Установка строки для TextBox (если используется Grid)
                    Grid.SetColumn(textBox, 1);
                    inputIndex++;
                    inputFieldsList.Add(textBox);
                }
                else if (column.Value == typeof(int) && column.Key != null)
                {

                    Label label = new Label();
                    label.Content = column.Key;
                    tableColumns.Add(column.Key);
                    tableColumnsType.Add(column.Value);

                    TextBox textBox = new TextBox();
                    textBox.Width = 200;
                    textBox.Text = values[count].ToString();
                    MainGrid.Children.Add(label);
                    MainGrid.Children.Add(textBox);
                    Grid.SetRow(label, inputIndex); // Установка строки для Label (если используется Grid)
                    Grid.SetColumn(label, 0); // Установка столбца для Label (если используется Grid)
                    Grid.SetRow(textBox, inputIndex); // Установка строки для TextBox (если используется Grid)
                    Grid.SetColumn(textBox, 1);
                    inputIndex++;
                    inputFieldsList.Add(textBox);


                }
                else if (column.Value == typeof(DateTime) && column.Key != null)
                {
                    Label label = new Label();
                    label.Content = column.Key;
                    tableColumnsType.Add(column.Value);

                    DatePicker inputControl = new DatePicker();
                    inputControl.Width = 200;
                    inputControl.Text = values[count].ToString(); 
                    MainGrid.Children.Add(label);
                    MainGrid.Children.Add(inputControl);
                    Grid.SetRow(label, inputIndex); // Установка строки для Label (если используется Grid)
                    Grid.SetColumn(label, 0); // Установка столбца для Label (если используется Grid)
                    Grid.SetRow(inputControl, inputIndex); // Установка строки для TextBox (если используется Grid)
                    Grid.SetColumn(inputControl, 1);
                    inputIndex++;
                    inputFieldsList.Add(inputControl);

                    tableColumns.Add(column.Key);



                }
                else if (column.Value == typeof(bool) && column.Key != null)
                {
                    Label label = new Label();
                    label.Content = column.Key;


                    CheckBox inputControl = new CheckBox();
                    inputControl.Width = 200;
                    MessageBox.Show(String.Join(',', values));
                    MessageBox.Show(count.ToString());
                    if ((bool)values[count] == true)
                    {
                        inputControl.IsChecked = true;
                    }
                    
                    MainGrid.Children.Add(label);
                    MainGrid.Children.Add(inputControl);
                    Grid.SetRow(label, inputIndex); // Установка строки для Label (если используется Grid)
                    Grid.SetColumn(label, 0); // Установка столбца для Label (если используется Grid)
                    Grid.SetRow(inputControl, inputIndex); // Установка строки для TextBox (если используется Grid)
                    Grid.SetColumn(inputControl, 1);
                    inputIndex++;
                    inputFieldsList.Add(inputControl);

                    tableColumns.Add(column.Key);
                    tableColumnsType.Add(column.Value);

                }

                count++;

            }
            Button button = new Button();
            button.Width = 200;
            button.Height = 30;
            button.Content = "Обновить";
            MainGrid.Children.Add(button);
            Grid.SetRow(button, inputIndex); // Установка строки для Label (если используется Grid)
            Grid.SetColumn(button, 0); // Установка столбца для Label (если используется Grid)

            button.Click += (sender, e) =>
            {
                // Ваша стрелочная функция здесь
                // Например:
                string data = "";
                List<object> values = new List<object>();

                for (int i = 0; i < inputFieldsList.Count; i++)
                {
                    var inputControl = inputFieldsList[i];
                    if (inputControl is TextBox textBox)
                    {
                        data += $"{textBox.Text}, ";
                        values.Add(textBox.Text);
                    }
                    else if (inputControl is DatePicker datePicker)
                    {
                        data += $"{datePicker.SelectedDate}, ";
                        values.Add(datePicker.SelectedDate);
                    }
                    else if (inputControl is CheckBox checkBox)
                    {
                        data += $"{checkBox.IsChecked}, ";
                        values.Add(checkBox.IsChecked);
                    }
                }
                try
                {
                    DBUtils.UpdateRecord(context, tableName + 's', recordID, tableColumns, values, tableColumnsType);
                    this.Hide();
                }
                catch { }
            };
        }
    }
}
