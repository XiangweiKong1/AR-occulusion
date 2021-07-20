using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using System.Collections.Concurrent;
using UnityEngine;

public class Receiver : StopableThread
{
    public readonly ConcurrentQueue<(Data, byte[])> toEventLoop;

    public Receiver()
    {
        toEventLoop = new ConcurrentQueue<(Data, byte[])>();
    }

    protected override void Run()
    {
        ForceDotNet.Force();

        var socket = new PullSocket();
        socket.Connect("tcp://localhost:5555");
        socket.Options.HeartbeatTimeout = System.TimeSpan.FromSeconds(1);
        socket.Options.Linger = System.TimeSpan.FromSeconds(1);
        while (Running)
        {
            if (socket.TryReceiveFrameString(System.TimeSpan.FromSeconds(1), out string dataJson) &&
                socket.TryReceiveFrameBytes(System.TimeSpan.FromSeconds(1), out byte[] frame))
            {
                var data = JsonUtility.FromJson<Data>(dataJson);
//                Debug.Log(dataJson);
 //               Debug.Log(data.dataL.rotations[0]);
                while (!toEventLoop.IsEmpty) toEventLoop.TryDequeue(out _);
                toEventLoop.Enqueue((data, frame));
            }
        }
    }
}