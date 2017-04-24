using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketPlayerTransformDenied : PacketBehaviour {

		public Vector3 position;

		public PacketPlayerTransformDenied() {
			id = 5;
		}

		public PacketPlayerTransformDenied(PolyNetBehaviour b, Vector3 pos) : base(b){
			id = 5;
			position = pos;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			position = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write ((decimal)position.x);
			writer.Write ((decimal)position.y);
			writer.Write ((decimal)position.z);

			base.write (ref writer);
		}

	}

}