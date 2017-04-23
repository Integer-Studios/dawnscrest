using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketLogin : Packet {

		public int playerId;

		public PacketLogin() {
			id = 3;
		}

		public PacketLogin(int i) {
			id = 3;
			playerId = i;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			playerId = reader.ReadInt32 ();
			sender.playerId = playerId;
			//continue login handling
			PolyServer.onLogin(sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (playerId);
		}

	}

}