C# library for Unity that allows interactions between Unity code and Js plugins (scripts).

**Background**

Unity has certain limitations when it comes to Web platform. Right now, there is no support for multi-threads and a lot of system.net libraries don't work. Few other libraries targeting Unity, may not support the Web platform. For example, firebase has currently no support for Unity Web.

This library primarily focuses on the Web plarform use-cases. It aims to solve the above scenarios, as well as allow a more closer development with the browser based APIs. It will allow building web-apps having a central portion created in Unity, where both the Unity app and js app can closely interact.

**Roadmap**
- Send messages between Js plugins.
- Build from a script.
- Support for non-Web platforms.

**Setup**
- Create a folder named `Plugins` in the `Assets` folder of your Unity project.
- Clone this repo inside the `Plugins` folder.
- It will add a new context menu item in the `Project` tab under `Create` menu called `Tunnel Manager`. Use that to create a new `Tunnel Manager` object in your assets. This scriptable object will be used for referencing the tunnel.

**Editor Play support**

The key feature of this library is to allow running js code in Editor play mode. Here's how to setup it:
- Create a new folder ourside your `Assets` which is supposed to store the output of your Unity project. For example: [Project_Name]/Bin
- Run `npm init && npm install jstunnel-unity-client`
- Create your js plugin that handles the tunnel messages. For example (test-plugin.ts):
  ```
    import {ITunnel} from 'jstunnel-unity-client';

    export class TestPlugin implements ITunnel {
        SendPacket?: (packet: string) => void;
        OnPacketReceived(packet: string) {
            console.log("Rcvd: " + packet);
            if (this.SendPacket != null)
            this.SendPacket("Hello from the other side");
        }
        SetSendPacketDelegate(sendPacket: (packet: string) => void) {
            this.SendPacket = sendPacket;
        }
    }

    declare var window: any;
    if(typeof window !== 'undefined') {
        // Make sure to set it, in case the class names are modified
        // by external plugins like webpack.
        window["TestPlugin"] = TestPlugin;
    }
  ```

- Make sure to set your class to the window object. This allows the unity web player to reference it.
- Create a client class to initialize the plugin for the editor environment. For example (unity-client.ts):
  ```
    import {TunnelEnd} from 'jstunnel-unity-client/tunnel_end';
    import {TestPlugin} from './test-plugin';

    new TunnelEnd(new TestPlugin());
  ```
- Now create a scrict configuration in your package.json file.
  `"unity": "tsc && node unity-client.js"`. Use tsc if you're using typescript.
- Next, configure the `Tunnel Manager` asset created in the previous step. Select the asset and make sure the configuration in the inspector is correct.
    * Server: The ip address of the server. It should be 127.0.0.1
    * Port: Port to use. You can use any value here.
    * Npm Command: Command to trigger the script, that you setup in step above.
    * Plugin Name: Name of the plugin that was set in the window.
    * Directory Path: path to the root of the npm package.

**Testing with a script**
- Create a new component script to an empty gameobject in Unity.
  ```
    using System.Collections;
    using UnityEngine;

    public class TunnelTest : MonoBehaviour
    {
        public TunnelManager manager;
        private ITunnel tunnel;
        void Start()
        {
            tunnel = manager.Inst;
            StartCoroutine(CheckReady());
        }

        IEnumerator CheckReady()
        {
            while (!tunnel.IsConnected())
            {
                yield return new WaitForEndOfFrame();
            }
            tunnel.OnPacketReceived += Tunnel_OnPacketReceived;
            tunnel.SendPacket("Hello World");
        }

        private void Tunnel_OnPacketReceived(string obj)
        {
            Debug.Log(obj);
        }
    }
  ```
- Create/Modify the HTML template.
    * Create a folder `Assets\WebGL Templates\[Project Name]`
    * Put the index.html template in this folder. [index.html]
      ```
         <!DOCTYPE html>
        <html lang="en-us">
        <head>
            <meta charset="utf-8">
            <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
            <title>Unity WebGL Player | {{{ PRODUCT_NAME }}}</title>
            <script>
            var exports = {};
            </script>
            <script src="lib/main.js"></script>
        </head>
        <body style="text-align: center">
            <canvas id="unity-canvas" style="width: {{{ WIDTH }}}px; height: {{{ HEIGHT }}}px; background: {{{ BACKGROUND_FILENAME ? 'url(\'Build/' + BACKGROUND_FILENAME.replace(/'/g, '%27') + '\') center / cover' : BACKGROUND_COLOR }}}"></canvas>
            <script src="Build/{{{ LOADER_FILENAME }}}"></script>
            <script>
            createUnityInstance(document.querySelector("#unity-canvas"), {
                dataUrl: "Build/{{{ DATA_FILENAME }}}",
                frameworkUrl: "Build/{{{ FRAMEWORK_FILENAME }}}",
                codeUrl: "Build/{{{ CODE_FILENAME }}}",
        #if MEMORY_FILENAME
                memoryUrl: "Build/{{{ MEMORY_FILENAME }}}",
        #endif
        #if SYMBOLS_FILENAME
                symbolsUrl: "Build/{{{ SYMBOLS_FILENAME }}}",
        #endif
                streamingAssetsUrl: "StreamingAssets",
                companyName: "{{{ COMPANY_NAME }}}",
                productName: "{{{ PRODUCT_NAME }}}",
                productVersion: "{{{ PRODUCT_VERSION }}}",
            });
            </script>
        </body>
        </html>
      ```
    * Make sure to include the two script tage in the `<head>`.
- Run or build your project!