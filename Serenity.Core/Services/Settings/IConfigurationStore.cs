namespace Serenity.Services
{
    public interface IConfigurationStore
    {
        void Save(string scope, string stateKey, string stateData);
        string Load(string scope, string stateKey);
    }
}