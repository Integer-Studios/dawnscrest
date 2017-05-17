using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketSyncFloat : PacketBehaviour {

		public int syncId;
		public float value;

		public PacketSyncFloat() {
			id = 13;
		}

		public PacketSyncFloat(PolyNetBehaviour b, int sid, float v) : base(b){
			id = 13;
			syncId = sid;
			value = v;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			syncId = reader.ReadInt32 ();
			value = (float)reader.ReadDecimal ();
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (syncId);
			writer.Write ((decimal)value);
			base.write (ref writer);
		}

	}

}
