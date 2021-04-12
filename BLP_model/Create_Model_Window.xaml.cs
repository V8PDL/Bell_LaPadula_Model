using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace BLP_model
{
    /// <summary>
    /// Логика взаимодействия для Create_Model_Window.xaml
    /// </summary>
    public partial class Create_Model_Window : Window
    {
        public Create_Model_Window()
        {
            InitializeComponent();
        }

        private void Create_model_button_Click(object sender, RoutedEventArgs e)
        {
            string login = Login_textbox.Text;
            string model_name = Model_name_textbox.Text;
            if (string.IsNullOrEmpty(login) ||
                string.IsNullOrEmpty(Passwordbox.Password) ||
                string.IsNullOrEmpty(model_name))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            string directory = Directory.GetCurrentDirectory();
            string model_directory = directory + "\\" + model_name;
            if (Directory.GetDirectories(directory).Contains(model_directory))
            {
                MessageBoxResult result = MessageBox.Show("Директория с таким названием уже существует. Удалить и записать заново?",
                    "Неверное имя", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes)
                    return;
                Directory.Delete(model_directory, true);
            }
            Directory.CreateDirectory(model_directory);

            using (StreamWriter sw = new StreamWriter(model_directory + "\\Object_list.json"))
                sw.Write("");
            Model_Subject model_Subject = new Model_Subject(login, 0, Passwordbox.Password, false);
            string serialized_subjects = JsonConvert.SerializeObject(new ObservableCollection<Model_Subject>() { model_Subject });
            using (StreamWriter sw = new StreamWriter((model_directory + "\\Subject_list.json")))
                sw.Write(serialized_subjects);

            this.Close();
        }
        private void Login_textbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                Passwordbox.Focus();
        }
        private void Passwordbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                Model_name_textbox.Focus();
        }
        private void Model_name_textbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Model_name_textbox.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
                Create_model_button_Click(null, null);
            }
        }
    }
}
