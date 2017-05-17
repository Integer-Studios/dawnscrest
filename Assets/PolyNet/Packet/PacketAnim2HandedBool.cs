using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketAnim2HandedBool : PacketBehaviour {

		public int boolId;
		public bool value;
		public bool rightHand;

		public PacketAnim2HandedBool() {
			id = 9;
		}

		public PacketAnim2HandedBool(PolyNetBehaviour b, int bid, bool val, bool rHand) : base(b){
			id = 9;
			boolId = bid;
			value = val;
			rightHand = rHand;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			boolId = reader.ReadInt32 ();
			value = reader.ReadBoolean ();
			rightHand = reader.ReadBoolean ();
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (boolId);
			writer.Write (value);
			writer.Write (rightHand);
			base.write (ref writer);
		}

	}

}
