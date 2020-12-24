using System.Collections;
using System.Collections.Generic;

namespace Serenity.Services
{
    public interface IRetrieveResponse
    {
        object Entity { get; }
        IDictionary Localizations { get; set; }
    }

    public class RetrieveResponse<T> : ServiceResponse, IRetrieveResponse
    {
        public T Entity { get; set; }

        public Dictionary<string, T> Localizations { get; set; }

        object IRetrieveResponse.Entity => Entity;

        IDictionary IRetrieveResponse.Localizations
        {
            get { return Localizations; }
            set { Localizations = (Dictionary<string, T>)value; }
        }
    }
}