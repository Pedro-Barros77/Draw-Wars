using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;

/// <summary>
///     Example of requester who only sends Hello. Very nice guy.
///     You can copy this class and modify Run() to suits your needs.
///     To use this class, you just instantiate, call Start() when you want to start and Stop() when you want to stop.
/// </summary>
public class Requester : RunAbleThread
{
    private string MessageReceived = null;
    public string[] MessagesToSend = new string[1] { "" };

    /// <summary>
    ///     Request Hello message to server and receive message back. Do it 10 times.
    ///     Stop requesting when Running=false.
    /// </summary>
    protected override void Run()
    {
        ForceDotNet.Force(); // this line is needed to prevent unity freeze after one use, not sure why yet
        using (RequestSocket client = new RequestSocket())
        {
            client.Connect("tcp://192.168.18.211:5555");

            foreach (string msg in MessagesToSend)
            {
                client.SendFrame(msg);

                bool gotMessage = false;
                while (Running)
                {
                    gotMessage = client.TryReceiveFrameString(out MessageReceived); // this returns true if it's successful
                    if (gotMessage) break;
                }

                if (gotMessage) Debug.Log("Received " + MessageReceived);
            }
        }

        NetMQConfig.Cleanup(); // this line is needed to prevent unity freeze after one use, not sure why yet
    }
}