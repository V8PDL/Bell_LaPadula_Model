using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace BLP_model
{
    public partial class Access_Matrix_Window : Window
    {
        ObservableCollection<Model_Subject> Subjects;
        ObservableCollection<Model_Object> Objects;
        public Access_Matrix_Window(ObservableCollection<Model_Subject> subjects, ObservableCollection<Model_Object> objects)
        {
            InitializeComponent();
            Subjects = subjects;
            Objects = objects;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e) => Upload_Matrix();
        void Upload_Matrix()
        {
            DataTable table = new DataTable("Access matrix");
            table.Columns.Add("O\\S");
            foreach (var s in Subjects)
                table.Columns.Add(s.ToString());
            foreach (var o in Objects)
            {
                List<string> row = new List<string>();
                row.Add(o.ToString());
                foreach (var s in Subjects)
                {
                    if (o.Security_Level == s.Security_Level)
                        row.Add("rw");
                    else
                    if (o.Security_Level < s.Security_Level)
                        row.Add("w");
                    else
                        row.Add("r");
                }
                table.Rows.Add(row.ToArray());
            }
            Access_matrix_datagrid.ItemsSource = table.AsDataView();
        }
        private void Refresh_button_Click(object sender, RoutedEventArgs e) => Upload_Matrix();
    }
}
