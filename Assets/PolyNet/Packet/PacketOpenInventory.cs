using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketOpenInventory : PacketBehaviour {

		public GameObject invObj;

		public PacketOpenInventory() {
			id = 18;
		}

		public PacketOpenInventory(PolyNetBehaviour b, GameObject o) : base(b){
			id = 18;
			invObj = o;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			invObj = Packet.unpackageObject(reader.ReadInt32 ());
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (Packet.packageObject(invObj));
			base.write (ref writer);
		}

	}

}
