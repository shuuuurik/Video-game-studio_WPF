using System;
using System.Windows;
using VideogameStudio;
using System.Collections.ObjectModel;

namespace Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static GameStudio Studio { get; set; }

        public GameStudio BackupCopy { get; private set; }

        public MainWindow(decimal budget,ObservableCollection<Employee> workers, TodoList todos)
        {
            InitializeComponent();
            Studio = new GameStudio(budget, workers, todos);
            BackupCopy = (GameStudio)Studio.Clone();

            ShowStatistics();
            Studio.StartLogging();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            workersListView.ItemsSource = Studio.Workers;
            todosListView.ItemsSource = Studio.Todos.Items;
        }

        private void SkipMonthButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Studio.SimulateMonthSkipping();
            } catch (NotSupportedException ex)
            {
                if (ex.Message == "Add Todos before calling TakeMostPriorityItem")
                {
                    MessageBox.Show("Для продолжения добавьте новую задачу.", "Программисты бездельничают!");
                }
                else
                {
                    MessageBox.Show(ex.Message, "Недостаточно работников!");
                }
            }
            ShowStatistics();
            Studio.WriteDataToFile();
            if (Studio.Budget == 0)
            {
                if (Studio.Workers.Count > 0)
                {
                    MessageBox.Show("Казна пуста, счастье программистов начинает падать. Нужно срочно выпустить какую-либо игру на продакшн.", "Предупреждение!");
                }
                else
                {
                    MessageBox.Show("Студия обанкротилась, программисты разбежались. Попробуйте еще раз.", "Вы проиграли!");
                    Studio.Dispose();
                    BackupCopy.Dispose();
                    Close();
                }
            }
        }

        private void StopCurrentDevelopmentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Studio.StopCurrentDevelopment();
                Studio.WriteDataToFile();
                ShowStatistics();
            }
            catch(NotSupportedException ex)
            {
                MessageBox.Show(ex.Message, "Предупреждение");
            }
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Studio.Budget == 0)
            {
                MessageBox.Show("Бюджет фирмы нулевой, мы не можем нанять новых работников", "Предупреждение");
                return;
            }
            AddingEmployeeWindow addWorkerWindow = new AddingEmployeeWindow(this);
            addWorkerWindow.ShowDialog();
        }
        private void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            Employee selected = (Employee)workersListView.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Сотрудник не выбран!", "Выберите сотрудника");
            }
            else
            {
                Studio.DeleteEmployee(selected);
                Studio.WriteDataToFile();
                ShowStatistics();
            }
        }


        private void AddTodoButton_Click(object sender, RoutedEventArgs e)
        {
            AddingDevelopmentWindow addDevelopmentWindow = new AddingDevelopmentWindow(this);
            addDevelopmentWindow.ShowDialog();
        }
        private void DeleteTodoButton_Click(object sender, RoutedEventArgs e)
        {
            if (todoTitleTextBox.Text == "")
            {
                MessageBox.Show("Поле для ввода названия разработки должно быть заполнено.", "Данные не введены");
                return;
            }
            try
            {
                Studio.DeleteTodo(todoTitleTextBox.Text);
                Studio.WriteDataToFile();
                todoTitleTextBox.Text = string.Empty;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Некорректное название");
            }
        }
        private void StartSelectedDevelopmentButton_Click(object sender, RoutedEventArgs e)
        {
            Todo selected = (Todo)todosListView.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Разработка не выбрана!", "Выберите разработку");
            }
            else
            {
                Studio.StartSelectedDevelopment(selected);
                ShowStatistics();
                Studio.WriteDataToFile();
            }
        }
        private void RecreateInstanceButton_Click(object sender, RoutedEventArgs e)
        {
            Studio.Dispose();
            Studio = (GameStudio)BackupCopy.Clone();
            workersListView.ItemsSource = Studio.Workers;
            todosListView.ItemsSource = Studio.Todos.Items;
            Studio.WriteDataToFile();
            ShowStatistics();
        }

        public void ShowStatistics()
        {
            budgetLabel.Content = string.Format("Бюджет: {0} руб", (int)Studio.Budget);
            currentExpensesLabel.Content = string.Format("Расход на з/п в месяц: {0} руб", Studio.CurrentExpenses);
            numberOfWorkersLabel.Content = string.Format("Сотрудники: {0}", Studio.Workers.Count);
            numberOfDevelopersLabel.Content = string.Format("Разработчики: {0}", Studio.NumberOfDevelopers);
            numberOfSoftwareTestersLabel.Content = string.Format("Тестировщики: {0}", Studio.NumberOfSoftwareTesters);
            developmentProductivityLabel.Content = string.Format("Мощность разработки: {0}", Studio.DevelopmentProductivity);
            testingProductivityLabel.Content = string.Format("Мощность тестирования: {0}", Studio.TestingProductivity);
            currentDevelopmentExpensesLabel.Content = string.Format("Затраты на разработку: {0} р", Studio.CurrentDevelopmentExpenses);
            currentTestingExpensesLabel.Content = string.Format("Затраты на тестирование: {0} р", Studio.CurrentTestingExpenses);
            if (Studio.CurrentTodo != null)
            {
                currentTodoLabel.Content = string.Format("Текущая разработка: {0}", Studio.CurrentTodo.Title);
                currentTodoComplexityLabel.Content = string.Format("Сложность разработки: {0}", Studio.CurrentTodo.DevelopmentComplexity);
                developmentProgressLabel.Content = string.Format("Прогресс разработки: {0}%", (int)Studio.GetDevelopmentProgress());
                testingProgressLabel.Content = string.Format("Прогресс тестирования: {0}%", (int)Studio.GetTestingProgress());
                currentTodoProfitLabel.Content = string.Format("Вознаграждение: {0}", Studio.CurrentTodo.Profit);
            }
            else
            {
                currentTodoLabel.Content = string.Format("Текущая разработка: ");
                currentTodoComplexityLabel.Content = string.Format("Сложность разработки:");
                developmentProgressLabel.Content = string.Format("Прогресс разработки:");
                testingProgressLabel.Content = string.Format("Прогресс тестирования:");
                currentTodoProfitLabel.Content = string.Format("Вознаграждение:");
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Studio.Dispose();
            BackupCopy.Dispose();
            MessageBox.Show("Студия уничтожена!");
        }
    }
}
