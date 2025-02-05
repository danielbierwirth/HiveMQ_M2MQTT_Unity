using UnityEngine;

public class DeviceMessage : BaseMessage<string>
{
    public DeviceMessage(string value)
    : base(value)
    {
    }

    public override string DeviceId { get; set; }

    public override string ValueAsJson()
    {
        return $"\"{Value}\"";
    }

    public override string ValueAsString()
    {
        return Value;
    }
}

