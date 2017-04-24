using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PacketAnim2HandedTrigger : PacketBehaviour {

		public int triggerId;
		public bool rightHand;

		public PacketAnim2HandedTrigger() {
			id = 8;
		}

		public PacketAnim2HandedTrigger(PolyNetBehaviour b, int tid, bool rHand) : base(b){
			id = 8;
			triggerId = tid;
			rightHand = rHand;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			triggerId = reader.ReadInt32 ();
			rightHand = reader.ReadBoolean ();
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (triggerId);
			writer.Write (rightHand);
			base.write (ref writer);
		}

	}

}
