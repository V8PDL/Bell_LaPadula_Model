using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace BLP_model
{
    static class Model
    {
        public static void Create_Object(string name, string sec_lvl, ObservableCollection<Model_Object> objects, string model, Model_Subject current_subject)
        {
            if (string.IsNullOrWhiteSpace(name) || objects.Any(s => s.Name == name) ||
                !int.TryParse(sec_lvl, out int Security_Level) || Security_Level < 0)
            {
                MessageBox.Show("Данные введены неверно. Избегайте дублей имен. Уровень доступа должен быть целым неотрицательным числом");
                return;
            }
            if (current_subject.Security_Level < Security_Level)
            {
                MessageBox.Show("Данный пользователь не может создать заметку с таким уровнем доступа");
                return;
            }
            objects.Add(new Model_Object(name, model, Security_Level, current_subject.Login));
            File.Create($"{Directory.GetCurrentDirectory()}\\{model}\\{name}.txt").Close();

            Open_Object(objects, objects.Count - 1, current_subject.Security_Level, true);

        }
        public static void Create_Subject(string Login, string sec_lvl, ObservableCollection<Model_Subject> Subjects, string Hashed_Password)
        {
            int Security_Level = -1;

            if (string.IsNullOrWhiteSpace(Login) || Subjects.Any(s => s.Login == Login) ||
                !int.TryParse(sec_lvl, out Security_Level) || Security_Level < 0 ||
                string.IsNullOrWhiteSpace(Hashed_Password))
            {
                MessageBox.Show("Данные введены неверно. Избегайте дублей имен; уровень доступа должен быть целым неотрицательным числом");
                return;
            }
            Subjects.Add(new Model_Subject(Login, Security_Level, Hashed_Password));
        }
        public static void Save_Model(string Model_Name, ObservableCollection<Model_Object> Objects,
            ObservableCollection<Model_Subject> Subjects = null)
        {
            Authorization_Window.Get_Subj_Obj_Dir(Model_Name, out string obj_dir, out string subj_dir);
            File.WriteAllText(obj_dir, JsonConvert.SerializeObject(Objects));
            if (Subjects != null)
                File.WriteAllText(subj_dir, JsonConvert.SerializeObject(Subjects));
        }
        public static void Open_Object(ObservableCollection<Model_Object> Objects, int index, int Subject_Security_Level, bool On_Creating)
        {
            if (index < 0)
            {
                MessageBox.Show("Выберите заметку");
                return;
            }
            Model_Object Object = Objects[index];
            string directory = $"{Directory.GetCurrentDirectory()}\\{Object.Model_Name}\\{Object.Name}.txt";
            if (!File.Exists(directory))
            {
                MessageBox.Show("Заметка не существует!"); Objects.RemoveAt(index);
                return;
            }
            if (Object.Security_Level < Subject_Security_Level && !On_Creating)
            {
                MessageBox.Show("Недостаточно прав!");
                return;
            }
            Object_Editor_Window object_editor = new Object_Editor_Window(directory, Object.Security_Level <= Subject_Security_Level);

            if (object_editor.ShowDialog() == true)
            {
                Objects.RemoveAt(index);
            }
        }
    }
}
