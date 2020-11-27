using System;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TcpTunnel:MonoBehaviour,ITunnel {
    public event Action<string> OnPacketReceived;
    public string server;
    public int port;
    private TcpClient client;
    private NetworkStream stream;
    private bool _connected;
    private CancellationTokenSource cts;
    public string npmCommand;

    // "/Dream"
    public string directoryPath;

    private System.Diagnostics.Process process;
    private string log = "";

    void runCommand()
    {
        process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c " + npmCommand + " " + port; // Note the /c command (*)
        process.StartInfo.UseShellExecute = false;
        //process.StartInfo.RedirectStandardOutput = true;
        //process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WorkingDirectory = directoryPath;
        //* Set your output and error (asynchronous) handlers
        //process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(OutputHandler);
        //process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(OutputHandler);
        //* Start process and handlers
        process.Start();
        //process.BeginOutputReadLine();
        //process.BeginErrorReadLine();
    }

    void LogOutput() {
        if (log != "") Debug.Log(log);
        log = "";
    }

    void OutputHandler(object sendingProcess, System.Diagnostics.DataReceivedEventArgs outLine) 
    {
        log+=outLine.Data;
    }
    void TryConnect() {
        try {
        client = new TcpClient(server, port);
        } catch(Exception ex) {
            TryConnect();
        }
    }
    void Start() {
        //InvokeRepeating("LogOutput", 2, 2);
        runCommand();
        TryConnect();
        stream = client.GetStream();
        _connected = true;
        cts = new CancellationTokenSource();
        StartCoroutine(StartReading());
    }

    IEnumerator StartReading() {
        byte[] meta = new byte[sizeof(Int32)];
        while(!cts.IsCancellationRequested) {
            while (!cts.IsCancellationRequested && stream.DataAvailable)
            {
                var bytesRead = stream.Read(meta, 0, meta.Length);
                if (bytesRead > 0)
                {
                    var packetSize = BitConverter.ToInt32(meta, 0);
                    byte[] data = new byte[packetSize];
                    var startIndex = 0;
                    while (startIndex < packetSize && !cts.IsCancellationRequested)
                    {
                        bytesRead = stream.Read(data, startIndex, packetSize - startIndex);
                        startIndex += bytesRead;
                    }
                    OnPacketReceived?.Invoke(System.Text.Encoding.ASCII.GetString(data));
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void OnDestroy() {
       KillProcessTree(process);
        cts.Cancel();
        stream.Close();
        client.Close();
    }

    public bool IsConnected() {
        return _connected;
    }

    public async void SendPacket(string packet) {
        byte[] data = System.Text.Encoding.ASCII.GetBytes(packet);
        byte[] meta = BitConverter.GetBytes((Int32)data.Length);
        byte[] combined = new byte[data.Length + meta.Length];
        System.Buffer.BlockCopy(meta, 0, combined, 0, meta.Length);
        System.Buffer.BlockCopy(data, 0, combined, meta.Length, data.Length);
        await stream.WriteAsync(combined, 0, combined.Length);
    }

    void KillProcessTree(System.Diagnostics.Process process)
    {
    string taskkill = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "taskkill.exe");
    using (var procKiller = new System.Diagnostics.Process()) {
        procKiller.StartInfo.FileName = taskkill;
        procKiller.StartInfo.Arguments = string.Format("/PID {0} /T /F", process.Id);
        procKiller.StartInfo.CreateNoWindow = true;
        procKiller.StartInfo.UseShellExecute = false;
        procKiller.Start();
        procKiller.WaitForExit(1000);   // wait 1sec
    }
    }
}
