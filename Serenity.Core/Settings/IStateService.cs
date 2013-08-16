namespace Serenity.Services
{
    public interface IStateService
    {
        void Save(string stateKey, string stateData);
        string Load(string stateKey);
    }
}