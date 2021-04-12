using Newtonsoft.Json;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace BLP_model
{
    public partial class User_Window : Window
    {
        private ObservableCollection<Model_Object> Objects = new ObservableCollection<Model_Object>();
        private ObservableCollection<Model_Object> All_Objects;

        private readonly Model_Subject Current_User;
        private readonly string Model_Name;
        public User_Window(Model_Subject subject, string model)
        {
            InitializeComponent();
            Current_User = subject;
            this.Model_Name = model;
        }
        private void Create_button_Click(object sender, RoutedEventArgs e)
        {
            Name_textbox.Focus();
            Model.Create_Object(Name_textbox.Text, Security_level_textbox.Text, Objects, Model_Name, Current_User);
            
            Name_textbox.Clear();
            Security_level_textbox.Clear();
        }
        private void Open_button_Click(object sender, RoutedEventArgs e) =>
            Model.Open_Object(Objects, Object_combobox.SelectedIndex, Current_User.Security_Level, false);
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = $"{Model_Name}: пользователь {Current_User.Login}";

            if (!Authorization_Window.Is_Model_Valid($"{Directory.GetCurrentDirectory()}\\{Model_Name}"))
            {
                if (MessageBox.Show("Модель повреждена! Удалить?", "Модель повреждена", MessageBoxButton.YesNo)
                    == MessageBoxResult.Yes)
                    Directory.Delete(Model_Name, true);
                this.Close();
            }
            All_Objects = JsonConvert.DeserializeObject<ObservableCollection<Model_Object>>(File.ReadAllText($"{Model_Name}\\Object_list.json"));
            if (All_Objects == null)
                All_Objects = new ObservableCollection<Model_Object>();
            else
                foreach (Model_Object o in All_Objects)
                    if (o.Security_Level >= Current_User.Security_Level)
                        Objects.Add(o);

            Object_combobox.ItemsSource = Objects;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Сохранить изменения?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Model_Object[] objects = new Model_Object[All_Objects.Count];
                All_Objects.CopyTo(objects, 0);
                foreach (Model_Object o in objects)
                {
                    if (o.Security_Level >= Current_User.Security_Level &&
                        !Objects.Contains(o) || 
                        (Objects.Any(obj => obj.Name == o.Name && obj.Security_Level != o.Security_Level)))
                        All_Objects.Remove(o);
                }
                foreach (Model_Object o in Objects)
                    if (!objects.Contains(o))
                        All_Objects.Add(o);

                Model.Save_Model(Model_Name, All_Objects);
            }
        }
        private void Name_textbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                Security_level_textbox.Focus();
        }
        private void Security_level_textbox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Security_level_textbox.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
                Create_button_Click(sender, null);
            }
        }
    }
}
