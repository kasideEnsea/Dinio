using System;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json.Serialization;
using WebSocketSharp;
using System.Text.Json;
using System.Net.Http.Headers;

namespace dinio_client
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ws = new WebSocket("ws://localhost:8080/gs-guide-websocket/websocket"))
            {
                int clientId = 999;
                StompMessageSerializer serializer = new StompMessageSerializer();

                ws.OnOpen += (sender, e) =>
                {
                    Console.WriteLine("Spring says: open");


                    var connect = new StompMessage("CONNECT");
                    connect["accept-version"] = "1.1";
                    connect["heart-beat"] = "10000,10000";
                    ws.Send(serializer.Serialize(connect));

                    var sub = new StompMessage("SUBSCRIBE");
                    sub["id"] = "sub-" + clientId;
                    sub["destination"] = "/topic/greetings";
                    ws.Send(serializer.Serialize(sub));

                };

                ws.OnError += (sender, e) =>
                Console.WriteLine("Error: " + e.Message);
                ws.OnMessage += (sender, e) =>
                Console.WriteLine("Spring says: " + e.Data);

                ws.Connect();

                var send = new StompMessage("SEND");
                Message msg = new Message("Anna");
                string json = JsonSerializer.Serialize<Message>(msg);
                send["destination"] = "/app/hello";
                send["content-length"] = json.Length.ToString();

                send.Body = json;
                Console.WriteLine(serializer.Serialize(send));
                ws.Send(serializer.Serialize(send));

                Console.ReadKey(true);
            }
        }
    }

    class Message
    {
        public string name { get; set; }

        public Message(string Name)
        {
            name = Name;
        }
    }
}
