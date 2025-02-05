using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMessage
{
    public virtual string DeviceId { get; set; }

    public virtual string ValueAsJson()
    {
        return $"\"\"";
    }

    public virtual string ValueAsString()
    {
        return string.Empty;
    }
}

public class BaseMessage<T> : BaseMessage
{
    public T Value { get; init; }
    protected BaseMessage(T value) : base()
    {
        Value = value;
    }
}

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}