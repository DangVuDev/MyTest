namespace MyAPI.Models
{
    public class DBMonggoSetting : IDBMonggoSetting
    {
        public string ConnectionString {get; set;} = string.Empty;
        public string DatabaseName {get; set;} = string.Empty;
    }
}
