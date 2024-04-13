using System;
using System.Windows;
using VideogameStudio;

namespace Monitor
{
    /// <summary>
    /// Interaction logic for AddingEmployeeWindow.xaml
    /// </summary>
    public partial class AddingEmployeeWindow : Window
    {
        public MainWindow Window { get; private set; }

        public AddingEmployeeWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            Window = mainWindow;
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (workerNameTextBox.Text == "")
                {
                    MessageBox.Show("Поле для ввода имени должно быть заполнено", "Введите имя сотрудника");
                    return;
                }
                string name = workerNameTextBox.Text;
                if (!int.TryParse(workerAgeTextBox.Text, out int age))
                {
                    MessageBox.Show("Поле для ввода возраста должно содержать целое число", "Введите корректный возраст сотрудника");
                    return;
                }
                if (!int.TryParse(workerExpierenceTextBox.Text, out int expierence))
                {
                    MessageBox.Show("Поле для ввода опыта должно содержать целое число", "Введите корректный опыт сотрудника");
                    return;
                }
                Employee worker;
                if (workerSpecialty.Text.ToString() == "Разработчик")
                {
                    worker = new Developer(name, age, expierence);
                }
                else
                {
                    worker = new SoftwareTester(name, age, expierence);
                }
                MainWindow.Studio.AddEmployee(worker);
                MainWindow.Studio.WriteDataToFile();
                Window.ShowStatistics();
                Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Невозможно добавить сотрудника");
            }
        }
    }
}
