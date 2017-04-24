using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketPlayerHit : PacketBehaviour {

		public GameObject hitObject;
		public Vector3 hitPoint;
		public Vector3 hitNormal;

		public PacketPlayerHit() {
			id = 10;
		}

		public PacketPlayerHit(PolyNetBehaviour b, GameObject hit, Vector3 p, Vector3 n) : base(b){
			id = 10;
			hitObject = hit;
			hitPoint = p;
			hitNormal = n;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			hitObject = Packet.unpackageObject(reader.ReadInt32 ());
			hitPoint = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			hitNormal = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (Packet.packageObject(hitObject));
		
			writer.Write ((decimal)hitPoint.x);
			writer.Write ((decimal)hitPoint.y);
			writer.Write ((decimal)hitPoint.z);

			writer.Write ((decimal)hitNormal.x);
			writer.Write ((decimal)hitNormal.y);
			writer.Write ((decimal)hitNormal.z);

			base.write (ref writer);
		}

	}

}
