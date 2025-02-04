using UnityEngine;

/// <summary>
/// Simple script to demonstrate the use of the MqttMessageProvider 
/// </summary>
public class DeviceMessageSubscriber : MonoBehaviour
{
 
    void Start()
    {
        SubscribeToMessages();
    }

    private async void SubscribeToMessages() {
         BaseMessageProvider mqttMessageProvider = Object.FindFirstObjectByType<BaseMessageProvider>();

        if (mqttMessageProvider != null && !mqttMessageProvider.IsInitialized)
        {
            mqttMessageProvider.OnMessageReceived += HandleMqttMessage;
            await mqttMessageProvider.InitializeAsync();
        }
    }

    // Update is called once per frame
    void HandleMqttMessage(BaseMessageProvider messageProvider, BaseMessage message)
    {
        Debug.Log("New Mqtt Message");
        Debug.Log("Message: " + message.ValueAsString());
    }
}
