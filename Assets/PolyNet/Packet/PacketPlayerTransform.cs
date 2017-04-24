using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketPlayerTransform : PacketBehaviour {

		public Vector3 velocity;
		public Vector3 position;
		public Vector3 euler;
		public float rotationalVelocity;
		public float pitch;

		public PacketPlayerTransform() {
			id = 4;
		}

		public PacketPlayerTransform(PolyNetBehaviour b, Vector3 vel, float rotVel, Vector3 pos, Vector3 rot, float p) : base(b){
			id = 4;
			velocity = vel;
			rotationalVelocity = rotVel;
			position = pos;
			euler = rot;
			pitch = p;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			velocity = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			position = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			euler = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			rotationalVelocity = (float)reader.ReadDecimal ();
			pitch = (float)reader.ReadDecimal ();
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write ((decimal)velocity.x);
			writer.Write ((decimal)velocity.y);
			writer.Write ((decimal)velocity.z);

			writer.Write ((decimal)position.x);
			writer.Write ((decimal)position.y);
			writer.Write ((decimal)position.z);

			writer.Write ((decimal)euler.x);
			writer.Write ((decimal)euler.y);
			writer.Write ((decimal)euler.z);

			writer.Write ((decimal)rotationalVelocity);
			writer.Write ((decimal)pitch);

			base.write (ref writer);
		}

	}

}