using UnityEngine;
using System;
using System.Collections;

public interface ITunnel
{
    bool IsConnected();
    void SendPacket(string packet);
    event Action<string> OnPacketReceived;
}
