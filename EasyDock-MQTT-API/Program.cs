using Newtonsoft.Json;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTTAPIEXAMPLE
{
    class Program
    {
        // MQTT Daten
        static string MQTT_HOST = "192.168.2.200";
        static string MQTT_USER = "easydock";
        static string MQTT_PASSWORD = "2024";
        static int MQTT_PORT = 1883;
        static MqttClient mqtt_client;

        // Dock SN
        static string dock_sn = "8UUXN2R00A10KF";

        // Topics
        static string TOPIC_REQUEST = "easydock-api/sn/" + dock_sn + "/request";
        static string TOPIC_REPLY = "easydock-api/sn/" + dock_sn + "/reply";

        // Request_ID
        static string last_message_id = "";

        public enum API_STATUS_CODE
        {
            ERROR = 0,
            SUCCESS = 1
        }

        public enum SignalType
        {
            REGULAR = 0,
            ALARM = 1,
        }

        static void Main(string[] args)
        {
            // Init Mqtt Client
            InitMqtt();

            // Start Existing Mission
            TriggerMission();

            // Fly To Coordinates
            //TriggerTakeOffToPoint();

            Thread.Sleep(1000000);
        }

        //--------------------------------------------------------------------------------
        //                                  INIT MQTT
        //--------------------------------------------------------------------------------

        static void InitMqtt()
        {
            try
            {
                // Topics definieren, die empfangen werden sollen
                string[] topics =
                {
                    TOPIC_REPLY
                };

                byte[] QOS_Level = new byte[topics.Length];
                for (int i = 0; i < QOS_Level.Length; i++)
                {
                    QOS_Level[i] = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;
                }

                Console.Write("Connecting to Mqtt Server...");
                mqtt_client = new MqttClient(MQTT_HOST, MQTT_PORT, false, null, null, MqttSslProtocols.None);
                mqtt_client.MqttMsgPublishReceived += ReadMqtt;
                mqtt_client.Connect(Guid.NewGuid().ToString(), MQTT_USER, MQTT_PASSWORD);
                mqtt_client.Subscribe(topics, QOS_Level);
                Console.WriteLine("Connected!");
            }
            catch (Exception error)
            {
                Console.WriteLine("Error in InitMqtt(): " + error.Message);
            }
        }

        //--------------------------------------------------------------------------------
        //                                  READ MQTT
        //--------------------------------------------------------------------------------

        static void ReadMqtt(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                // Bytes in String umwandeln
                string payload = Encoding.UTF8.GetString(e.Message);

                // JSON parsen
                dynamic obj = JsonConvert.DeserializeObject(payload);

                // Zugriff auf message_id
                string message_id = obj.reply_to;

                // Zuordnung zum letzten Request herstellen
                if (message_id == last_message_id)
                {

                }
                Console.WriteLine("Easydock Response: Received " + Encoding.UTF8.GetString(e.Message) + "\n");
            }
            catch (Exception error)
            {
                Console.WriteLine("Error in ReadMqtt(): " + error.Message);
            }
        }

        //--------------------------------------------------------------------------------
        //                                TRIGGER MISSION
        //--------------------------------------------------------------------------------

        static void TriggerMission()
        {
            try
            {
                // Flug triggern
                Console.WriteLine("Press Enter to start a mission");
                Console.ReadLine();

                last_message_id = Guid.NewGuid().ToString();

                // Mission
                var message = new
                {
                    message_id = last_message_id,
                    timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    method = "start_mission",
                    dock_sn = dock_sn,
                    data = new
                    {
                        mission_name = "test",
                        mission_by_easydock = true,
                        signal_type = (int)SignalType.ALARM,
                        checklist_confirmed = true,
                        user_id = "123"
                    }
                };

                SendToEasyDock(TOPIC_REQUEST, message);
                Console.WriteLine("Mission Command send");
            }
            catch (Exception error)
            {
                Console.WriteLine("Error in TriggerMission(): " + error.Message);
            }
        }

        //--------------------------------------------------------------------------------
        //                            TRIGGER TAKEOFF TO POINT
        //--------------------------------------------------------------------------------

        static void TriggerTakeOffToPoint()
        {
            try
            {
                // TakeOffToPoint triggern
                Console.WriteLine("Press Enter to start a TakeOffToPoint");
                Console.ReadLine();

                last_message_id = Guid.NewGuid().ToString();

                // Direct-Flight
                var message = new
                {
                    message_id = last_message_id,
                    timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    method = "takeoff_to_point",
                    dock_sn = dock_sn,
                    data = new
                    {
                        lat = 52.084373,
                        lng = 8.5098026,
                        height = 50,
                        signal_type = (int)SignalType.ALARM,
                        checklist_confirmed = true,
                        user_id = "123"
                    }
                };

                SendToEasyDock(TOPIC_REQUEST, message);
                Console.WriteLine("Takeoff Command send");
            }
            catch (Exception error)
            {
                Console.WriteLine("Error in TriggerTakeOffToPoint(): " + error.Message);
            }
        }

        //--------------------------------------------------------------------------------
        //                                 SEND TO EASYDOCK
        //--------------------------------------------------------------------------------

        static void SendToEasyDock(string topic, object data)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(data);
                mqtt_client.Publish(topic, Encoding.UTF8.GetBytes(jsonData), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }
            catch (Exception error)
            {
                Console.WriteLine("Error in SendToEasyDock(): " + error.Message);
            }
        }
    }
}