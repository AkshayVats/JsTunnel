JsTunnel = {
  Init: function(obj_name) {
    var SendPacketToUnity = this.SendPacketToUnity;
    this.obj_name_str = Pointer_stringify(obj_name);
    this.plugin = new window[this.obj_name_str]();
    this.plugin.SetSendPacketDelegate(function(packet) {SendPacketToUnity(packet);});
  },

  SendPacketToUnity: function(packet) {
    SendMessage(this.obj_name_str, 'OnPacketReceivedInternal', packet);
  },

  SendPacketToPlugin: function(packet) {
    this.plugin.OnPacketReceived(Pointer_stringify(packet));
  },

  IsConnected: function() {
    return (typeof this.plugin !== 'undefined');
  }
};

mergeInto(LibraryManager.library, JsTunnel);