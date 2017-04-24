using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketPlaceItem : PacketBehaviour {

		public Vector3 position;
		public bool rightHand;

		public PacketPlaceItem() {
			id = 12;
		}

		public PacketPlaceItem(PolyNetBehaviour b, Vector3 p, bool rh) : base(b){
			id = 12;
			position = p;
			rightHand = rh;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			position = new Vector3 ((float)reader.ReadDecimal (), (float)reader.ReadDecimal (), (float)reader.ReadDecimal ());
			rightHand = reader.ReadBoolean ();
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write ((decimal)position.x);
			writer.Write ((decimal)position.y);
			writer.Write ((decimal)position.z);

			writer.Write (rightHand);

			base.write (ref writer);
		}

	}

}