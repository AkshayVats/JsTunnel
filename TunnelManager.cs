using System;
using UnityEngine;

[CreateAssetMenu]
public class TunnelManager : ScriptableObject {
    public string server = "127.0.0.1";
    public int port = 8085;   
    public string npmCommand = "npm run unity";
    public string pluginName = "TestPlugin";

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
            var tunnel = new GameObject(pluginName);
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
