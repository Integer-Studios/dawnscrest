using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketSyncInt : PacketBehaviour {

		public int syncId;
		public int value;

		public PacketSyncInt() {
			id = 16;
		}

		public PacketSyncInt(PolyNetBehaviour b, int sid, int v) : base(b){
			id = 16;
			syncId = sid;
			value = v;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			syncId = reader.ReadInt32 ();
			value = reader.ReadInt32 ();
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (syncId);
			writer.Write (value);
			base.write (ref writer);
		}

	}

}
