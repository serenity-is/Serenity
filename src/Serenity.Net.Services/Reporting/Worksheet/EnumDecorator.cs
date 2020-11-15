namespace Serenity.Reporting
{
    using System;

    public class EnumDecorator : BaseCellDecorator
    {
        private Type enumType;

        public EnumDecorator(Type enumType)
        {
            this.enumType = enumType;
        }

        public override void Decorate()
        {
            if (Value != null)
            {
                try
                {
                    Value = EnumMapper.FormatEnum(enumType, Value);
                }
                catch
                {
                }
            }
        }
    }
}
