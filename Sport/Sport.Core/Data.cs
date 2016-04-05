using Sport.Core.Models;
using Sport.Core.Models.EndPoints;

namespace Sport.Core
{
    public class Data
    {
        public Session Session { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }


        public Settings Settings { get; set; }
    }
}
