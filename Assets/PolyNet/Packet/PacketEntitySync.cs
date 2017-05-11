using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketEntitySync : PacketBehaviour {

		public float vertical;
		public float horizontal;
		public Vector3 position;
		public Vector3 euler;

		public PacketEntitySync() {
			id = 24;
		}

		public PacketEntitySync(PolyNetBehaviour b, float v, float h) : base(b){
			id = 24;
			position = b.transform.position;
			euler = b.transform.eulerAngles;
			vertical = v;
			horizontal = h;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			position = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			euler = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write ((decimal)position.x);
			writer.Write ((decimal)position.y);
			writer.Write ((decimal)position.z);

			writer.Write ((decimal)euler.x);
			writer.Write ((decimal)euler.y);
			writer.Write ((decimal)euler.z);
			base.write (ref writer);
		}

	}

}