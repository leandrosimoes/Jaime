namespace Jaime.Models {
    public class HttpProxyModel {
        public bool Enabled { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; } 
        public string Password { get; set; } 
    }
}