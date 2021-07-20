using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using System.Collections.Concurrent;

public class Sender : StopableThread
{
    public readonly ConcurrentQueue<byte[]> fromEventLoop;

    public Sender()
    {
        fromEventLoop = new ConcurrentQueue<byte[]>();
    }

    protected override void Run()
    {
        ForceDotNet.Force();
        var socket = new PushSocket();
        socket.Options.HeartbeatTimeout = System.TimeSpan.FromSeconds(1);
        socket.Options.Linger = System.TimeSpan.FromSeconds(1);
        socket.Connect("tcp://localhost:5556");
        while (Running)
        {
            if (fromEventLoop.TryDequeue(out byte[] img))
            {
                socket.TrySendFrame(System.TimeSpan.FromSeconds(1), img);
            }
        }
    }
}