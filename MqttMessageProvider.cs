using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using M2Mqtt;
using M2Mqtt.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// Attach this component to a gameobject and set the mqtt endpoint details
/// 
/// Subscribe to OnMessageReceived to receive the device messages from the MqttMessageProvider
/// </summary>
public class MqttMessageProvider : BaseMessageProvider
{
    [SerializeField] private string _mqttBrokerEndpoint = "<YOUR_MQTT_BROKER_ADDRESS>";
    [SerializeField] private int _mqttBrokerPort = 8883;
    [SerializeField] private string _mqttSubTopic = "<YOUR_MQTT_Subscription_Topic>";
    [SerializeField] private string _mqttUserName = "<YOUR_MQTT_User_Name>";
    [SerializeField] private string _mqttPassword = "<YOUR_MQTT_Password>";

    private MqttClient _mqttClient;

    private bool _isConnected;

    /// <summary>
    /// Initializes MQTT client or sets up simulated events based on the `UseSimulatedEvents` setting.
    /// </summary>
    public override async Task InitializeAsync()
    {
        Debug.Log("Initializing in MQTT mode...");
        await InitializeMqttClientAsync();
    }

    /// <summary>
    /// Initializes the MQTT client asynchronously.
    /// </summary>
    private async Task InitializeMqttClientAsync()
    {
        try
        {
            string clientId = Guid.NewGuid().ToString();

            _mqttClient = new MqttClient(_mqttBrokerEndpoint, _mqttBrokerPort, true, null, null, MqttSslProtocols.TLSv1_2);

            // Register event for receiving messages
            _mqttClient.MqttMsgPublishReceived += OnMqttMsgPublishReceived;

            Debug.Log("Connecting to MQTT broker...");

            Task connectTask = Task.Run(() => _mqttClient.Connect(clientId, _mqttUserName, _mqttPassword));
            await connectTask;

            if (_mqttClient.IsConnected)
            {
                _isConnected = true;
                SubscribeToTopic(_mqttSubTopic);
                Debug.Log("MQTT Client Connected");
            }
            else
            {
                Debug.LogError("Failed to connect to the MQTT broker.");
            }
        }
        catch (SocketException se)
        {
            Debug.LogError($"[SocketException] Failed to connect to MQTT broker: {se.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Exception] Failed to initialize MQTT client: {e.Message}");
        }
    }

    /// <summary>
    /// Subscribes to the specified MQTT topic.
    /// </summary>
    /// <param name="topic">The topic to subscribe to.</param>
    private void SubscribeToTopic(string topic)
    {
        if (!_isConnected || _mqttClient == null) return;

        try
        {
            _mqttClient.Subscribe(new[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            Debug.Log($"Subscribed to MQTT topic: {topic}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to subscribe to MQTT topic '{topic}': {e.Message}");
        }
    }

    /// <summary>
    /// Handles incoming MQTT messages.
    /// </summary>
    private void OnMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        if (IsPaused) return;

        try
        {
            string message = Encoding.UTF8.GetString(e.Message);
            DeviceMessage deviceMessage = CreateDeviceMessage(message);

            if (deviceMessage != null)
            {
                MessageReceived(deviceMessage);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error processing MQTT message: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a DeviceMessage object from a raw MQTT message.
    /// </summary>
    private DeviceMessage CreateDeviceMessage(string message)
    {
        try
        {
            return new DeviceMessage(message) { DeviceId = "not implemented" };
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create DeviceMessage: {e.Message}");
            Debug.LogError($"Raw message: {message}");
        }

        return null;
    }

    /// <summary>
    /// Cleans up resources when the application quits.
    /// </summary>
    private void OnApplicationQuit()
    {
        DisconnectClient();
    }

    /// <summary>
    /// Properly disconnects the MQTT client.
    /// </summary>
    private void DisconnectClient()
    {
        if (_mqttClient != null)
        {
            try
            {
                if (_mqttClient.IsConnected)
                {
                    _mqttClient.Disconnect();
                    Debug.Log("MQTT client disconnected.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error disconnecting MQTT client: {e.Message}");
            }
            finally
            {
                _mqttClient = null;
            }
        }
    }
}
