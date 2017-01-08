using Serenity.Data;
using System.Text;

namespace Serenity
{
    public interface ICriteria
    {
        bool IsEmpty { get; }
        string ToString(IQueryWithParams query);
        void ToString(StringBuilder sb, IQueryWithParams query);
        string ToStringIgnoreParams();
    }
}