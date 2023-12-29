namespace Serenity.TypeScript;

public struct TypeOption<Type1, Type2>
{
    private Type1 value1;
    private Type2 value2;
    private bool hasValue;
    private bool isType2;

    public TypeOption()
    {
    }

    public TypeOption(Type1 value1)
    {
        this.value1 = value1;
        hasValue = true;
    }
    
    public TypeOption(Type2 value2)
    {
        this.value2 = value2;
        isType2 = true;
        hasValue = true;
    }

    public readonly bool HasValue => hasValue;

    public readonly bool HasValue1 => hasValue && isType2;

    public Type1 Value1
    {
        readonly get => hasValue ? value1 : default;
        set { value1 = value; value2 = default; hasValue = true; isType2 = false;  }
    }

    public readonly bool HasValue2 => hasValue && isType2 == true;

    public Type2 Value2
    {
        readonly get => value2;
        set { value1 = default; value2 = value; hasValue = true; isType2 = true; }
    }

    public void Clear()
    {
        hasValue = false;
        value1 = default;
        value2 = default;
    }

    //public static implicit operator TypeOption<Type1, Type2>(Type1 value1) 
    //{ 
    //    return new TypeOption<Type1, Type2>() { Value1 = value1 }; 
    //}

    //public static implicit operator TypeOption<Type1, Type2>(Type2 value2)
    //{
    //    return new TypeOption<Type1, Type2>() { Value2 = value2 };
    //}

    //public static explicit operator Type1(TypeOption<Type1, Type2> o) => o.Value1;

    //public static explicit operator Type2(TypeOption<Type1, Type2> o) => o.Value2;
}