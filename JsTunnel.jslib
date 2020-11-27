JsTunnel = {
  Init: function(obj_name) {
    obj_name_str = Pointer_stringify(obj_name);
    TUNNEL = {
      SendPacket: function(packet) {
         SendMessage(obj_name_str, 'OnPacketReceivedInternal', packet);
      },
      SetOnPacketReceived: function(callback) {
        this.OnPacketReceived = callback;
      }
    };
    new FirebaseWrapper(TUNNEL, firebase);
  },

  SendPacket: function(packet) {
    TUNNEL.OnPacketReceived(Pointer_stringify(packet));
  },

  IsConnected: function() {
    return (typeof TUNNEL !== 'undefined') && (typeof TUNNEL.OnPacketReceived !== 'undefined');
  }
};

mergeInto(LibraryManager.library, JsTunnel);