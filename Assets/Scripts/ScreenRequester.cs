using System.Collections.Generic;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class ScreenRequester : RunAbleThread
{
    protected override void Run()
    {
        ForceDotNet.Force();
        using (var client = new RequestSocket())
        {
            client.Connect("tcp://localhost:6789");

            for (var i = 0; Running; i++)
            {
                client.SendFrame("Ping");
                byte[] bytes = null;
                var gotMessage = false;
                while (Running)
                {
                    gotMessage = client.TryReceiveFrameBytes(out bytes); // this returns true if it's successful
                    if (gotMessage) break;
                }

                if (!gotMessage) continue;
                var colors = new List<Color>();
                for (var p = 0; p < bytes.Length; p += 4)
                    colors.Add(new Color(bytes[p + 2] / 255f, bytes[p + 1] / 255f, bytes[p] / 255f));

                MainHandler.screenPixels = colors;
                MainHandler.updateScreen = true;
            }
        }

        NetMQConfig.Cleanup();
    }
}