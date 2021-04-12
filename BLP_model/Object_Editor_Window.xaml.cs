using System.IO;
using System.Windows;

namespace BLP_model
{
    public partial class Object_Editor_Window : Window
    {
        private readonly string directory;
        private int need_save = 1;
        public Object_Editor_Window(string dir, bool editable)
        {
            InitializeComponent();
            directory = dir;
            if (!editable)
            {
                need_save = 0;
                Textbox.IsReadOnly = true;
                Delete_button.IsEnabled = false;
                Save_button.IsEnabled = false;
                Warning_label.Content = "Редактирование запрещено";
            }
            Textbox.Text = File.ReadAllText(directory);
        }
        private void Save_button_Click(object sender, RoutedEventArgs e) => File.WriteAllText(directory, Textbox.Text);
        private void Delete_button_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(directory);
            need_save = -1;
            Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (need_save < 1)
            {
                if (need_save == -1)
                    this.DialogResult = true;
                else
                    this.DialogResult = false;
                return;
            }
            MessageBoxResult result = MessageBox.Show("Сохранить заметку?", "Сохранение",
                MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
                File.WriteAllText(directory, Textbox.Text);
            else
                if (result == MessageBoxResult.Cancel)
                e.Cancel = true;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = directory.Remove(0, directory.LastIndexOf("\\") + 1);
        }
    }
}
