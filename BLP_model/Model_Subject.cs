using System.Security.Cryptography;
using System.Text;

namespace BLP_model
{
    public class Model_Subject
    {
        public string Login { get; set; }
        public int Security_Level { get; set; }
        public string Password_hash { get; set; }
        public Model_Subject() { }
        public Model_Subject(string name, int security_level, string password, bool hashed = true)
        {
            if (string.IsNullOrWhiteSpace(name) || security_level < 0 || password == null)
                return;
            Login = name;
            Security_Level = security_level;
            Password_hash = hashed ? password : Get_Hash(password);
        }
        public static string Get_Hash(string password)
        {
            if (password == null)
                return null;
            byte[] hash;
            using (HashAlgorithm algorithm = SHA256.Create())
                hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hash)
                stringBuilder.Append(b.ToString("X2"));
            return stringBuilder.ToString();
        }
        public override string ToString()
        {
            return $"{Login}: {Security_Level}";
        }
    }
}
