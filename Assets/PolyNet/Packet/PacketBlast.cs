using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketBlast : Packet {

		public int playerId;

		public PacketBlast() {
			id = 23;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			// clear client to get more packets on server
			// relay back to server if on client
			if (PolyServer.isActive)
				sender.isAccepting = true;
			else
				PacketHandler.queuePacket (this, null);
		}

		public override void write(ref BinaryWriter writer) {
		}

	}

}