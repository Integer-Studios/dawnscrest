using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketSyncVector : PacketBehaviour {

		public int syncId;
		public Vector3 value;

		public PacketSyncVector() {
			id = 25;
		}

		public PacketSyncVector(PolyNetBehaviour b, int sid, Vector3 v) : base(b){
			id = 25;
			syncId = sid;
			value = v;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			syncId = reader.ReadInt32 ();
			value = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (syncId);
			writer.Write ((decimal)value.x);
			writer.Write ((decimal)value.y);
			writer.Write ((decimal)value.z);
			base.write (ref writer);
		}

	}

}
