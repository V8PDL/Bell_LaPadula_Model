using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace BLP_model
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    //Разработайте программу, реализующую модель безопасности Белла-ЛаПадула.
    // Основные функции программы: 
    //     1. регистрация пользователей(при регистрации пользователь получает уровень допуска), 
    //     2. авторизация, создание текстовых заметок(при создании заметка получает уровень секретности), 
    //     3. просмотр и редактирование заметок.
    public partial class Authorization_Window : Window
    {
        public ObservableCollection<string> Models = new ObservableCollection<string>();
        public Authorization_Window()
        {
            InitializeComponent();
            Models = new ObservableCollection<string>();
            Models_Combobox.ItemsSource = Models;
            Upload_Models();
        }
        private void Upload_Models()
        {
            string[] directories = Directory.GetDirectories(Directory.GetCurrentDirectory());

            foreach (string directory in directories)
            {
                string model_name = directory.Remove(0, directory.LastIndexOf("\\") + 1);
                if (Models.Contains(model_name))
                    continue;
                if (Is_Model_Valid(directory))
                    Models.Add(directory.Remove(0, directory.LastIndexOf("\\") + 1));
            }
        }

        public static bool Is_Model_Valid(string directory)
        {
            bool valid = true;
            string[] files = Directory.GetFiles(directory);
            Get_Subj_Obj_Dir(directory, out string obj_dir, out string subj_dir);

            ObservableCollection<Model_Subject> subjects = new ObservableCollection<Model_Subject>();
            if (files.Contains(obj_dir) && files.Contains(subj_dir))
            {
                try
                {
                    string obj_text = File.ReadAllText(obj_dir);
                    string subj_text = File.ReadAllText(subj_dir);
                    if (!string.IsNullOrEmpty(obj_text))
                        JsonConvert.DeserializeObject<ObservableCollection<Model_Object>>(obj_text);
                    subjects = JsonConvert.DeserializeObject<ObservableCollection<Model_Subject>>(subj_text);
                }
                catch (Exception)
                {
                    valid = false;
                }
                var ex = subjects.Select(s => string.IsNullOrWhiteSpace(s.Login)).ToList().Any(a => a == true);
                if (subjects == null || subjects.Count == 0 || subjects.Any(s => string.IsNullOrWhiteSpace(s.Login)) ||
                    subjects.Any(s => s.Security_Level < 0) || subjects.Any(s => string.IsNullOrWhiteSpace(s.Password_hash)))
                    valid = false;
            }
            else
                valid = false;
            return valid;
        }

        public static void Get_Subj_Obj_Dir(string directory, out string obj_dir, out string subj_dir)
        {
            string cur_dir = Directory.GetCurrentDirectory();
            if (directory.LastIndexOf("\\") > 0)
                directory = directory.Remove(0, directory.LastIndexOf("\\"));
            else
                directory = directory.Insert(0, "\\");
            obj_dir = cur_dir + directory + "\\Object_list.json";
            subj_dir = cur_dir + directory + "\\Subject_list.json";
        }

        private void New_model_button_Click(object sender, RoutedEventArgs e)
        {
            Create_Model_Window create_Model_Window = new Create_Model_Window();
            create_Model_Window.ShowDialog();
            Upload_Models();
        }

        private void Log_in_button_Click(object sender, RoutedEventArgs e)
        {
            if (Models_Combobox.SelectedIndex < 0)
            {
                MessageBox.Show("Выберите модель из списка");
                return;
            }
            string model = Models_Combobox.SelectedItem.ToString();
            string Cur_dir = Directory.GetCurrentDirectory();
            string model_dir = Cur_dir + "\\" + model;
            if (!Directory.Exists(model_dir))
            {
                MessageBox.Show("Модель не существует!");
                Models.Remove(model);
                return;
            }
            if (!Is_Model_Valid(model_dir))
            {
                if (MessageBox.Show("Модель повреждена! Удалить каталог?", "Ошибка модели",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Directory.Delete(model_dir, true);
                    Models.Remove(model);
                }
                else
                    Models.Remove(model);
                return;
            }
            string login = Login_textbox.Text;
            List<Model_Subject> subjects = JsonConvert.DeserializeObject<List<Model_Subject>>
                (File.ReadAllText(model_dir + "\\Subject_list.json"));
            Model_Subject subject = subjects.Find(s => s.Login == login);

            if (subject == null || Model_Subject.Get_Hash(Passwordbox.Password) != subject.Password_hash)
            {
                MessageBox.Show("Неверный логин или пароль!");
                return;
            }
            Passwordbox.Clear();

            this.Visibility = Visibility.Hidden;
            if (subject.Security_Level == 0)
            {
                Administrator_Window administrator_Window = new Administrator_Window(subject, model);
                administrator_Window.ShowDialog();
            }
            else
            {
                User_Window user_Window = new User_Window(subject, model);
                user_Window.ShowDialog();
            }
            this.Visibility = Visibility.Visible;
            Login_textbox.Clear();
        }
        private void Login_textbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                Passwordbox.Focus();
        }
        private void Passwordbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.Focus();
                Log_in_button_Click(null, null);
            }
        }

        private void Models_Combobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            => Login_textbox.Focus();
    }
}
