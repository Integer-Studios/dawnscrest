using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketHotbarSwitch : PacketBehaviour {

		public bool rightHand;

		public PacketHotbarSwitch() {
			id = 17;
		}

		public PacketHotbarSwitch(PolyNetBehaviour b, bool rHand) : base(b){
			id = 17;
			rightHand = rHand;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			rightHand = reader.ReadBoolean ();
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (rightHand);
			base.write (ref writer);
		}

	}

}