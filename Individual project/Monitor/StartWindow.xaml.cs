using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Threading;
using System.Globalization;
using System.Collections.ObjectModel;
using VideogameStudio;

namespace Monitor
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {

        public ObservableCollection<Employee> Workers { get; private set; } = new ObservableCollection<Employee>();

        public TodoList Todos { get; private set; } = new TodoList();

        public StartWindow()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            InitializeComponent();
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = string.Empty;
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.InitialDirectory =
                System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\..\..\..AppData");

            openFileDlg.Title = "Выберите файл со списком сотрудников";
            openFileDlg.Multiselect = false;
            openFileDlg.DefaultExt = ".txt";
            openFileDlg.Filter = "Text Documents (.txt)|*.txt";

            bool? result = openFileDlg.ShowDialog();
            if (result == true)
            {
                fileName = openFileDlg.FileName;
            }
            else
            {
                MessageBox.Show("Не удалось открыть файл.", "Ошибка");
                return;
            }

            try
            {
                using (FileStream file = File.OpenRead(fileName))
                {
                    using(StreamReader reader = new StreamReader(file))
                    {
                        Workers.Clear();
                        
                        int lineNumber = 1;
                        string errorMessage = string.Empty;
                        while (!reader.EndOfStream)
                        {
                            try
                            {
                                string textLine = reader.ReadLine();
                                char[] separators = new char[] { ' ', ',', ';' };
                                string[] subs = textLine.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                if (subs.Length < 4)
                                {
                                    throw new FormatException("Мало данных о сотруднике.");
                                }
                                string name = subs[0];
                                if (!int.TryParse(subs[1], out int age))
                                {
                                    throw new FormatException("Возраст сотрудника не является целым числом.");
                                }
                                if (!int.TryParse(subs[2], out int expierence))
                                {
                                    throw new FormatException("Опыт сотрудника не является целым числом.");
                                }
                                Employee worker;
                                if (subs[3] == "Разработчик")
                                {
                                    worker = new Developer(name, age, expierence);
                                }
                                else if (subs[3] == "Тестировщик")
                                {
                                    worker = new SoftwareTester(name, age, expierence);
                                }
                                else
                                {
                                    throw new FormatException("Профессии сотрудника не существует в нашей студии.");
                                }
                                Workers.Add(worker);
                            } catch (Exception ex)
                            {
                                errorMessage += string.Format("Ошибка в строке {0}: {1}\n", lineNumber, ex.Message);
                            }
                            lineNumber++;
                        }
                        if (errorMessage != "")
                        {
                            string errorTitle = string.Format("Ошибка в файле {0}", fileName.Substring(fileName.LastIndexOf("\\") + 1));
                            MessageBox.Show(errorMessage, errorTitle);
                        }
                    }
                }
            } catch (Exception ex)
            {
                string errorTitle = string.Format("Ошибка в файле {0}", fileName.Substring(fileName.LastIndexOf("\\") + 1));
                MessageBox.Show(ex.Message, errorTitle);
            }
        }

        private void TodoListFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = string.Empty;
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.InitialDirectory =
                System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + @"\..\..\..AppData");

            openFileDlg.Title = "Выберите файл со списком разработок";
            openFileDlg.Multiselect = false;
            openFileDlg.DefaultExt = ".txt";
            openFileDlg.Filter = "Text Documents (.txt)|*.txt";

            bool? result = openFileDlg.ShowDialog();
            if (result == true)
            {
                fileName = openFileDlg.FileName;
            }
            else
            {
                MessageBox.Show("Не удалось открыть файл.", "Ошибка");
                return;
            }

            try
            {
                using (FileStream file = File.OpenRead(fileName))
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        Todos.Clear();

                        int lineNumber = 1;
                        string errorMessage = string.Empty;
                        while (!reader.EndOfStream)
                        {
                            try
                            {
                                string textLine = reader.ReadLine();
                                char[] separators = new char[] { ' ', ',', ';' };
                                string[] subs = textLine.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                if (subs.Length < 3)
                                {
                                    throw new FormatException("Мало данных о разработке.");
                                }
                                string title = subs[0];
                                if (!int.TryParse(subs[1], out int complexity))
                                {
                                    throw new FormatException("Сложность разработки не является целым числом.");
                                }
                                if (!decimal.TryParse(subs[2], out decimal profit))
                                {
                                    throw new FormatException("Вознаграждение не является числом.");
                                }
                                Todo newTodo = new Todo(title, complexity, profit);
                                Todos.Add(newTodo);
                            }
                            catch (Exception ex)
                            {
                                errorMessage += string.Format("Ошибка в строке {0}: {1}\n", lineNumber, ex.Message);
                            }
                            lineNumber++;
                        }
                        if (errorMessage != "")
                        {
                            string errorTitle = string.Format("Ошибка в файле {0}", fileName.Substring(fileName.LastIndexOf("\\") + 1));
                            MessageBox.Show(errorMessage, errorTitle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorTitle = string.Format("Ошибка в файле {0}", fileName.Substring(fileName.LastIndexOf("\\") + 1));
                MessageBox.Show(ex.Message, errorTitle);
            }
        }

        private void CreateInstanceButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(budgetTextBox.Text, out decimal budget))
            {
                MessageBox.Show("Поле для ввода бюджета студии должно содержать число (возможно дробное).", "Введите корректный бюджет");
                return;
            }
            if (budget <= 0)
            {
                MessageBox.Show("Бюджет студии должен быть положительным.", "Введите корректный бюджет");
                return;
            }
            decimal firstExpense = 0;
            foreach (Employee employee in Workers)
            {
                firstExpense += employee.Salary;
            }
            if (budget < firstExpense)
            {
                string message = string.Format("Указанного бюджета не хватит даже на первую з/п сотрудникам. " +
                    "Текущему персоналу требуется {0} руб в месяц.", firstExpense);
                MessageBox.Show(message, "Увеличьте бюджет");
                budgetTextBox.Text = firstExpense.ToString();
                return;
            }
            MainWindow mainWindow = new MainWindow(budget, Workers, Todos);
            mainWindow.Show();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            workersListView.ItemsSource = Workers;
            todosListView.ItemsSource = Todos.Items;
        }
    }
}
