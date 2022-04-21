namespace Serenity.Services
{
    /// <summary>
    /// Specifies that the class this attribute attached to is the default handler (list, create, delete, update etc).
    /// and should be used by some behaviors like MasterDetailRelationBehavior instead of creating a generic handler.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DefaultHandlerAttribute : Attribute
    {
        public DefaultHandlerAttribute(bool isDefault = true)
        {
            Value = isDefault;
        }

        public bool Value { get; }
    }
}