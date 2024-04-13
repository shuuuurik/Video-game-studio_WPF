using System;
using System.Windows;
using VideogameStudio;

namespace Monitor
{
    /// <summary>
    /// Interaction logic for AddingDevelopmentWindow.xaml
    /// </summary>
    public partial class AddingDevelopmentWindow : Window
    {
        public MainWindow Window { get; private set; }

        public AddingDevelopmentWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            Window = mainWindow;
        }

        private void AddTodoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (todoTitleTextBox.Text == "")
                {
                    MessageBox.Show("Поле для названия разработки должно быть заполнено.", "Введите название");
                    return;
                }
                string title = todoTitleTextBox.Text;
                if (MainWindow.Studio.CurrentTodo != null && title == MainWindow.Studio.CurrentTodo.Title)
                {
                    MessageBox.Show("Игра с таким названием уже разрабатывается.", "Некорректное название");
                    return;
                }
                if (!int.TryParse(todoDevelopmentComplexityTextBox.Text, out int complexity))
                {
                    MessageBox.Show("Поле для ввода сложности разработки должно содержать целое число.", "Введите корректную сложность");
                    return;
                }
                if (!decimal.TryParse(todoProfitTextBox.Text, out decimal profit))
                {
                    MessageBox.Show("Поле для ввода вознаграждения должно содержать число (возможно дробное).", "Введите корректное вознаграждение");
                    return;
                }
                Todo newTodo = new Todo(title, complexity, profit);
                MainWindow.Studio.AddTodo(newTodo);
                MainWindow.Studio.WriteDataToFile();
                Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Невозможно добавить разработку");
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show(ex.Message, "Некорректное название");
            }
        }
    }
}
