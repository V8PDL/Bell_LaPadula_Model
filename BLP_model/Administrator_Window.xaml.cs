using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace BLP_model
{
    public partial class Administrator_Window : Window
    {
        readonly Model_Subject Current_User;
        readonly string Model_Name;
        public ObservableCollection<Model_Subject> Subjects;
        public ObservableCollection<Model_Object> Objects;
        public Administrator_Window(Model_Subject subject, string model)
        {
            InitializeComponent();
            Current_User = subject;
            Model_Name = model;
        }
        private void Object_radiobutton_Checked(object sender, RoutedEventArgs e)
        {
            Password_label.Visibility = Visibility.Hidden;
            Passwordbox.Visibility = Visibility.Hidden;
        }
        private void Object_radiobutton_Unchecked(object sender, RoutedEventArgs e)
        {
            Password_label.Visibility = Visibility.Visible;
            Passwordbox.Visibility = Visibility.Visible;
        }
        private void Create_button_Click(object sender, RoutedEventArgs e)
        {
            Name_textbox.Focus();
            if (Subject_radiobutton.IsChecked == true)
            {
                Model.Create_Subject(Name_textbox.Text, Security_level_textbox.Text, Subjects, Model_Subject.Get_Hash(Passwordbox.Password));
                Passwordbox.Clear();
            }
            else
                Model.Create_Object(Name_textbox.Text, Security_level_textbox.Text, Objects, Model_Name, Current_User);
            Name_textbox.Clear();
            Security_level_textbox.Clear();
        }
        private void Remove_subject_button_Click(object sender, RoutedEventArgs e)
        {
            int index = Subject_combobox.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("Выберите пользователя");
                return;
            }
            Model_Subject subject = Subjects[index];
            if (subject.Security_Level == 0 && Subjects.Count(s => s.Security_Level == 0) == 1)
            {
                MessageBox.Show("Невозможно удалить всех администраторов!");
                return;
            }
            Subjects.RemoveAt(index);
        }
        private void Save_button_Click(object sender, RoutedEventArgs e) =>
            Model.Save_Model(Model_Name, Objects, Subjects);
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Сохранить изменения?", "", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
                Model.Save_Model(Model_Name, Objects, Subjects);
            if (result == MessageBoxResult.Cancel)
                e.Cancel = true;
        }
        private void Open_object_button_Click(object sender, RoutedEventArgs e) =>
            Model.Open_Object(Objects, Object_combobox.SelectedIndex, Current_User.Security_Level, false);
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = $"{Model_Name}: администратор {Current_User.Login}";
            if (!Authorization_Window.Is_Model_Valid($"{Directory.GetCurrentDirectory()}\\{Model_Name}"))
            {
                if (MessageBox.Show("Модель повреждена! Удалить?", "Модель повреждена", MessageBoxButton.YesNo)
                    == MessageBoxResult.Yes)
                    Directory.Delete(Model_Name, true);
                this.Close();
            }
            Subjects = JsonConvert.DeserializeObject<ObservableCollection<Model_Subject>>(File.ReadAllText($"{Model_Name}\\Subject_list.json"));
            Objects = JsonConvert.DeserializeObject<ObservableCollection<Model_Object>>(File.ReadAllText($"{Model_Name}\\Object_list.json"));
            if (Objects == null)
                Objects = new ObservableCollection<Model_Object>();
            Subject_combobox.ItemsSource = Subjects;
            Object_combobox.ItemsSource = Objects;
        }
        private void Show_matrix_button_Click(object sender, RoutedEventArgs e)
        {
            Access_Matrix_Window access_Matrix_Window =
                new Access_Matrix_Window(Subjects, Objects) { Owner = this };
            access_Matrix_Window.Show();
        }
        private void Name_textbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                Security_level_textbox.Focus();
        }

        private void Security_level_textbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                Passwordbox.Focus();
        }

        private void Passwordbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Passwordbox.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
                Create_button_Click(null, null);
            }
        }
    }
}
