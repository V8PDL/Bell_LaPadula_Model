namespace BLP_model
{
    public class Model_Object
    {
        public string Name { get; set; }
        public string Model_Name { get; set; }
        public int Security_Level { get; set; }
        public System.DateTime Last_Change_Date { get; set; }
        public string Last_Change_User { get; set; }
        public string Get_Object_Directory() => $"{System.IO.Directory.GetCurrentDirectory()}\\{Model_Name}\\{Name}.json";
        public Model_Object() { }
        public Model_Object(string name, string model_name, int security_level, string last_change_user)
        {
            Name = name;
            Model_Name = model_name;
            Security_Level = security_level;
            Last_Change_Date = System.DateTime.Now;
            Last_Change_User = last_change_user;
        }
        public override string ToString() => $"{Name}: {Security_Level}";
    }
}
