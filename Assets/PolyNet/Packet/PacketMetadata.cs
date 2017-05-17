using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketMetadata : PacketBehaviour {

		public int metadata;

		public PacketMetadata() {
			id = 11;
		}

		public PacketMetadata(PolyNetBehaviour b, int m) : base(b){
			id = 11;
			metadata = m;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			metadata = reader.ReadInt32 ();
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (metadata);
			base.write (ref writer);
		}

	}

}
