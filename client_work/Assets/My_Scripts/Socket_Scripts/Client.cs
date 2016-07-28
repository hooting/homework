using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.IO;

public class Client : MonoBehaviour
{
    private const int NET_HEAD_LENGTH_SIZE = 4;

    String host = "localhost";
    Int32 port = 50000;
    
    internal Boolean socketReady = false;
    TcpClient tcpSocket;
    NetworkStream netStream;

    public BinaryReader reader;
    public BinaryWriter writer;

    private Dictionary<int, Service> serviceMap;

    void Awake ()
	{
		DontDestroyOnLoad (this);
        serviceMap = new Dictionary<int, Service>();
        SetupSocket();
	}

    void Update()
    {
        TryRead();
    }

    void SetupSocket()
    {
        try
        {
            tcpSocket = new TcpClient(host, port);

            netStream = tcpSocket.GetStream();

            reader = new BinaryReader(netStream);
            writer = new BinaryWriter(netStream);
            socketReady = true;
        }
        catch (Exception e)
        {
            // Something went wrong
            Console.WriteLine("Socket error:" + e);
        }
    }

    void CloseSocket()
    {
        if (!socketReady)
            return;

        writer.Close();
        reader.Close();
        tcpSocket.Close();
        socketReady = false;
    }

    private void TryRead()
    {
        while (netStream.DataAvailable)
        {
            Dispatch();
        }
    }

    public void TryWriter(List<System.Object> values)
    {
        int len = 0;
        foreach(System.Object obj in values){
            if (obj is ushort) len += sizeof(ushort);
            else if (obj is uint) len += sizeof(uint);
            else if (obj is int) len += sizeof(int);
            else if (obj is float) len += sizeof(float);
            else if (obj is string)
            {
                len += sizeof(int);
                len += ((string)obj).Length;
            }
        }
        len += 4;
        writer.Write((uint)len);
        foreach (System.Object obj in values)
        {
            if (obj is ushort) writer.Write((ushort)obj);
            else if (obj is uint) writer.Write((uint)obj);
            else if (obj is int) writer.Write((int)obj);
            else if (obj is float) writer.Write((float)obj);
            else if (obj is string)
            {
                writer.Write(((string)obj).Length);
                writer.Write(System.Text.Encoding.UTF8.GetBytes((string)obj));
            }
        }
        writer.Flush();
    }

    public void Register(int sid, Service service)
    {
        this.serviceMap[sid] = service;
    }

    private void Dispatch()
    {
        uint len = reader.ReadUInt32();
        ushort sid = reader.ReadUInt16();
        Service service;
        if(this.serviceMap.TryGetValue(sid,out service))
        {
            service.Process((int)len-6, reader); // 6 = size(len) + size(sid)
        }
    }
}
