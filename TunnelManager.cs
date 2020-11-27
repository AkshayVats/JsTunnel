using System;
using UnityEngine;

[CreateAssetMenu]
public class TunnelManager : ScriptableObject {
    public string server;
    public int port;   
    public string npmCommand;

    // "/Dream"
    public string directoryPath;

    private ITunnel _inst;
    public ITunnel Inst
    {
        get
        {
            if (_inst != null)
            {
                return _inst;
            }
            var tunnel = new GameObject("JsTunnel");
            if (Application.platform == RuntimePlatform.WebGLPlayer && !Application.isEditor)
            {
                return (_inst = tunnel.AddComponent<JsTunnel>());
            }
            else
            {
                var tcpClient = tunnel.AddComponent<TcpTunnel>();
                tcpClient.server = server;
                tcpClient.port = Application.isEditor ? port + 1 : port;
                tcpClient.directoryPath = directoryPath;
                tcpClient.npmCommand = npmCommand;
                return (_inst = tcpClient);
            }
        }
    }

    public T InitializePlugin<T>() where T:MonoBehaviour
    {
        return ((MonoBehaviour)Inst).gameObject.AddComponent<T>(); ;
    }
}
