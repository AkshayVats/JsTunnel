using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class JsTunnel : MonoBehaviour, ITunnel {

    [DllImport("__Internal")]
    private static extern void Init(string objName);

    [DllImport("__Internal")]
    private static extern void SendPacket(string packet);

    [DllImport("__Internal")]
    private static extern int IsConnected();

    void Start() {
        Init(gameObject.name);
    }

    private void OnPacketReceivedInternal(string packet) {
        OnPacketReceived?.Invoke(packet);
    }

    bool ITunnel.IsConnected() {
        return JsTunnel.IsConnected() != 0;
    }

    void ITunnel.SendPacket(string packet) {
        JsTunnel.SendPacket(packet);
    }

    public event Action<string> OnPacketReceived;
}
