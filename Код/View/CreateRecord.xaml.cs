using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Project.View
{
    /// <summary>
    /// Interaction logic for CreateRecord.xaml
    /// </summary>
    public partial class CreateRecord : Window
    {
        private List<string> tableColumns = new List<string>();
        private DataBaseUtils DBUtils = new DataBaseUtils();
        private YourDbContext context = new YourDbContext();

        static string DictionaryToString(Dictionary<string, Type> dictionary)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kvp in dictionary)
            {
                sb.Append($"{kvp.Key}: {kvp.Value.Name}, ");
            }
            // Удаление последней запятой и пробела
            if (sb.Length > 2)
            {
                sb.Length -= 2;
            }
            return sb.ToString();
        }

        public CreateRecord(string tableName, Dictionary<string, Type> columns)
        {
            InitializeComponent();
            List<object> inputFieldsList = new List<object>();
            int rowIndex = 1;
            int inputIndex = 0;

            bool flag = true;

            foreach (var column in columns)
            {
                if (flag) {
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
                    TextBox textBox = new TextBox();
                    textBox.Width = 200;
                    MainGrid.Children.Add(label);
                    MainGrid.Children.Add(textBox);
                    Grid.SetRow(label, inputIndex); // Установка строки для Label (если используется Grid)
                    Grid.SetColumn(label, 0); // Установка столбца для Label (если используется Grid)
                    Grid.SetRow(textBox, inputIndex); // Установка строки для TextBox (если используется Grid)
                    Grid.SetColumn(textBox, 1);
                    inputIndex++;
                    inputFieldsList.Add(textBox);
                }
                else if (column.Value == typeof(int) && column.Key != null) {

                    Label label = new Label();
                    label.Content = column.Key;
                    tableColumns.Add(column.Key);

                    TextBox textBox = new TextBox();
                    textBox.Width = 200;
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
                    DatePicker inputControl = new DatePicker();
                    inputControl.Width = 200;
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

            }  
            Button button = new Button();
            button.Width = 200;
            button.Height = 30;
            button.Content = "Создать";
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


                string SQLQuery = DBUtils.BuildInsertQuery(tableName, tableColumns, values);
                bool res = DBUtils.ExecuteSql(context, SQLQuery);
                if (res)
                {
                    this.Hide();
                }

            };
        }
        
    }
}