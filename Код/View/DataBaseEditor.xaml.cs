
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Project.View
{
    /// <summary>
    /// Логика взаимодействия для DataBaseEditor.xaml
    /// </summary>
    public partial class DataBaseEditor : Window
    {
        public class FieldData
        {
            public string FieldName { get; set; }
            public Type FieldType { get; set; }
            public object FieldValue { get; set; }

            public FieldData(string name, Type type, object value)
            {
                FieldName = name;
                FieldType = type;
                FieldValue = value;
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid senderGrid = sender as DataGrid;
            int itemID = -1;
            if (senderGrid != null && senderGrid.SelectedItem != null)
            {
                object selected = senderGrid.SelectedItem;
                if (selected != null)
                {
                    Type type = selected.GetType();
                    PropertyInfo[] properties = type.GetProperties();

                    // Используйте id и text по вашему усмотрению
                    foreach (PropertyInfo property in properties)
                    {
                        //MessageBox.Show($"Name: {property.Name}, Value: {property.GetValue(selected)}");
                        if (property.Name.ToLower().Contains("id"))
                        {
                            itemID = (int)property.GetValue(selected);
                            break;
                        }
                    }
                }
            }
            if (itemID != -1)
            {
                selectedRowIndex = itemID;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, есть ли сохраненная информация о выделенной строке
            if (selectedRowIndex > 0)
            {
                TabItem selectedTabItem = MainControl.SelectedItem as TabItem;
                string tabName = selectedTabItem.Header.ToString();

                if (tabName.ToLower().Contains("user"))
                {
                    if (selectedRowIndex == AdminID)
                    {
                        MessageBox.Show("Вы не можете удалить себя");
                        return;
                    }
                }
                   DBUtils.DeleteRecordById(context, tabName + "s", selectedRowIndex);
                RefreshDataGrid(selectedTabItem);


            }
            else
            {
                MessageBox.Show("Ни одна строка не выбрана.");
            }
        }

        public void RefreshDataGrid(TabItem selectedTabItem)
        {
            if (selectedTabItem != null)
            {
                Grid pageGrid = selectedTabItem.Content as Grid;
                if (pageGrid != null)
                {
                    DataGrid dataGrid = pageGrid.Children.OfType<DataGrid>().FirstOrDefault();
                    if (dataGrid != null)
                    {
                        string tabName = selectedTabItem.Header.ToString();
                        if (!tabName.Contains("Отчет"))
                        {
                            context = new YourDbContext();
                            dataGrid.ItemsSource = null; // Установка свойства ItemsSource в null
                            List<object> data = DBUtils.GetAllByTableName(context, tabName + 's');
                            dataGrid.ItemsSource = data; // Установка обновленного источника данных
                            dataGrid.Items.Refresh();
                        } else
                        {
                            if (tabName == "Отчет по сообщениям")
                            {
                                gridTable.ItemsSource = null;
                                List<Message> dataOtchet = DBUtils.GetMessagesForUserByDate(context, 1, DateTime.Now.Date);
                                gridTable.ItemsSource = dataOtchet;
                            }
                        }
                        
                    }
                }
            }
        }

        public void RefreshDataGridButton(object sender, RoutedEventArgs e)
        {
            RefreshDataGrid(MainControl.SelectedItem as TabItem);
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            TabItem selectedTabItem = MainControl.SelectedItem as TabItem;
            string tableName = selectedTabItem.Header.ToString();
            Dictionary<string, Type> columns = DBUtils.GetTableColumns(tableName);

            CreateRecord createRecordWindow = new CreateRecord(tableName, columns);
            createRecordWindow.ShowDialog();
        }

        public void UpdateButton_Click(Object sender, RoutedEventArgs e)
        {
            TabItem selectedTabItem = MainControl.SelectedItem as TabItem;
            string tableName = selectedTabItem.Header.ToString();
            if (selectedRowIndex > 0)
            {
                TabItem selectedTabItem1 = MainControl.SelectedItem as TabItem;
                string tabName = selectedTabItem1.Header.ToString();



                List<object> values = DBUtils.GetRecordValuesById(tableName, selectedRowIndex);

                EditRecord editRecordWindow = new EditRecord(tableName, selectedRowIndex, values);
                editRecordWindow.ShowDialog();

            }
        }

        public DataBaseEditor(int UserID)
        {
            InitializeComponent();
            AdminID = UserID;

            List<string?> tables = DBUtils.GetAllTables(context);

            for (int i = 0; i < tables.Count; i++)
            {
                // Создаем новую сетку для каждой вкладки
                Grid PageGrid = new Grid();
                PageGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(50)
                }); // Создаем одну строку для всех кнопок

                TabItem tab = new TabItem();
                tab.Header = tables[i];

                // Создание четырех кнопок
                Button button1 = new Button { Content = "Обновить", Height = 50, Width = 100, Margin = new Thickness(10, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Left };
                button1.Click += RefreshDataGridButton;
                Button button2 = new Button { Content = "Удалить", Height = 50, Width = 100, Margin = new Thickness(120, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Left };
                button2.Click += DeleteButton_Click;
                Button button3 = new Button { Content = "Добавить", Height = 50, Width = 100, Margin = new Thickness(230, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Left };
                button3.Click += CreateButton_Click;
                Button button4 = new Button { Content = "Изменить", Height = 50, Width = 100, Margin = new Thickness(340, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Left };
                button4.Click += UpdateButton_Click;
                PageGrid.Children.Add(button1);
                PageGrid.Children.Add(button2);
                PageGrid.Children.Add(button3);
                PageGrid.Children.Add(button4);

                PageGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(640)
                }); // Создаем одну строку для всех кнопок


                gridTable = new DataGrid
                {
                    Width = 1200,
                    Height = 500,
                };

                List<object> data = DBUtils.GetAllByTableName(context, tables[i] + 's');
                gridTable.ItemsSource = data;
                gridTable.SelectionMode = DataGridSelectionMode.Single;
                gridTable.SelectionChanged += DataGrid_SelectionChanged;

                Grid.SetRow(gridTable, 1);
                PageGrid.Children.Add(gridTable);
                tab.Content = PageGrid;

                MainControl.Items.Add(tab);

                
            }

            // Отчет 1
            Grid PageGridOtchet = new Grid();
            TabItem tabOtchet = new TabItem();
            tabOtchet.Header = "Отчет по сообщениям";
            PageGridOtchet.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(50)
            }); // Создаем одну строку для всех кнопок

            
            DatePicker datePicker = new DatePicker
            {
                Width = 200,
                Height = 30,
                Margin = new Thickness(120, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBox userIdTextBox = new TextBox
            {
                Width = 200,
                Height = 30,
                Margin = new Thickness(360, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Text= "ID Пользователя"
            };

            // Создаем кнопку "Создать отчет"
            Button createReportButton = new Button
            {
                Content = "Создать отчет",
                Height = 50,
                Width = 150,
                Margin = new Thickness(580, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Добавляем обработчик события нажатия кнопки "Создать отчет"
            
            // Добавляем все элементы в Grid
            PageGridOtchet.Children.Add(datePicker);
            PageGridOtchet.Children.Add(userIdTextBox);
            PageGridOtchet.Children.Add(createReportButton);

            PageGridOtchet.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(640)
            }); // Создаем одну строку для всех кнопок
            DataGrid gridTable1 = new DataGrid
            {
                Width = 1200,
                Height = 500,
            };
            createReportButton.Click += (sender, e) =>
            {
                // Получаем выбранную дату и айди пользователя
                DateTime selectedDate = datePicker.SelectedDate ?? DateTime.Now.Date;
                int UserID;
                bool success = int.TryParse(userIdTextBox.Text, out UserID);
                if (!success)
                {
                    MessageBox.Show("ID пользователя - число");
                    return;
                }
                // Загружаем данные для отчета и обновляем DataGrid
                List<Message> reportData1 = DBUtils.GetMessagesForUserByDate(context, UserID, selectedDate);
                gridTable1.ItemsSource = reportData1;
            };

            // List < Message > dataOtchet = DBUtils.GetMessagesForUserByDate(context, 1, DateTime.Now.Date);
            //gridTable.ItemsSource = dataOtchet;
            //gridTable.SelectionMode = DataGridSelectionMode.Single;
            //gridTable.SelectionChanged += DataGrid_SelectionChanged;

            Grid.SetRow(gridTable1, 1);
            PageGridOtchet.Children.Add(gridTable1);
            tabOtchet.Content = PageGridOtchet;

            MainControl.Items.Add(tabOtchet);


            // отчет 2
            Grid PageGridOtchet2 = new Grid();
            TabItem tabOtchet2 = new TabItem();
            tabOtchet2.Header = "Отчет по общим друзьям";
            PageGridOtchet2.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(50)
            }); // Создаем одну строку для всех кнопок


            TextBox userIdTextBox1 = new TextBox
            {
                Width = 200,
                Height = 30,
                Margin = new Thickness(140, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "ID Пользователя 1"
            };

            TextBox userIdTextBox2 = new TextBox
            {
                Width = 200,
                Height = 30,
                Margin = new Thickness(360, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Text = "ID Пользователя 2"
            };

            // Создаем кнопку "Создать отчет"
            Button createReportButton2 = new Button
            {
                Content = "Создать отчет",
                Height = 50,
                Width = 150,
                Margin = new Thickness(580, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Добавляем обработчик события нажатия кнопки "Создать отчет"
            createReportButton2.Click += (sender, e) =>
            {
                // Получаем выбранную дату и айди пользователя
                
                int UserID1;
                int UserID2;
                bool success1 = int.TryParse(userIdTextBox1.Text, out UserID1);
                bool success2 = int.TryParse(userIdTextBox2.Text, out UserID2);
                if (!success1 || !success2)
                {
                    MessageBox.Show("ID пользователя - число");
                    return;
                }
                // Загружаем данные для отчета и обновляем DataGrid
                List<User?> reportData = DBUtils.GetCommonFriends(context, UserID1, UserID2);
                gridTable.ItemsSource = reportData;
            };
            // Добавляем все элементы в Grid
            PageGridOtchet2.Children.Add(userIdTextBox1);
            PageGridOtchet2.Children.Add(userIdTextBox2);
            PageGridOtchet2.Children.Add(createReportButton2);

            PageGridOtchet2.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(640)
            }); // Создаем одну строку для всех кнопок
            gridTable = new DataGrid
            {
                Width = 1200,
                Height = 500,
            };

            // List < Message > dataOtchet = DBUtils.GetMessagesForUserByDate(context, 1, DateTime.Now.Date);
            //gridTable.ItemsSource = dataOtchet;
            //gridTable.SelectionMode = DataGridSelectionMode.Single;
            //gridTable.SelectionChanged += DataGrid_SelectionChanged;

            Grid.SetRow(gridTable, 1);
            PageGridOtchet2.Children.Add(gridTable);
            tabOtchet2.Content = PageGridOtchet2;

            MainControl.Items.Add(tabOtchet2);
        }

        

        private int AdminID;
        private YourDbContext context = new YourDbContext();
        private DataBaseUtils DBUtils = new DataBaseUtils();
        private DataGrid gridTable;
        private int selectedRowIndex = -1;
    }
}