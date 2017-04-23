using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketTransform : PacketBehaviour {

		public Vector3 position;
		public Vector3 scale;
		public Vector3 euler;

		public PacketTransform() {
			id = 2;
		}

		public PacketTransform(PolyNetBehaviour b) : base(b){
			id = 2;
			position = b.transform.position;
			scale = b.transform.localScale;
			euler = b.transform.eulerAngles;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			position = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			scale = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			euler = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write ((decimal)position.x);
			writer.Write ((decimal)position.y);
			writer.Write ((decimal)position.z);

			writer.Write ((decimal)scale.x);
			writer.Write ((decimal)scale.y);
			writer.Write ((decimal)scale.z);

			writer.Write ((decimal)euler.x);
			writer.Write ((decimal)euler.y);
			writer.Write ((decimal)euler.z);
			base.write (ref writer);
		}

	}

}