using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketLogin : Packet {

		public int playerId;
		public string session;

		public PacketLogin() {
			id = 3;
		}

		public PacketLogin(int i, string s) {
			id = 3;
			playerId = i;
			session = s;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			playerId = reader.ReadInt32 ();
			session = reader.ReadString ();
			sender.playerId = playerId;
			sender.session = session;
			//continue login handling
			PolyServer.onLogin(sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (playerId);
			writer.Write (session);
		}

	}

}