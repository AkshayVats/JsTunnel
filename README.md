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