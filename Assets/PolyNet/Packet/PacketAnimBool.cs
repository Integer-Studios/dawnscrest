using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketAnimBool : PacketBehaviour {

		public int boolId;
		public bool value;

		public PacketAnimBool() {
			id = 7;
		}

		public PacketAnimBool(PolyNetBehaviour b, int bid, bool val) : base(b){
			id = 7;
			boolId = bid;
			value = val;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			boolId = reader.ReadInt32 ();
			value = reader.ReadBoolean ();
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (boolId);
			writer.Write (value);
			base.write (ref writer);
		}

	}

}