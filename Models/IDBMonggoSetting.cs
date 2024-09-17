namespace MyAPI.Models
{
    public interface IDBMonggoSetting
    {
        string ConnectionString {get; set;} 
        string DatabaseName {get; set;} 
    }
}
