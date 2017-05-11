using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PolyItem;

namespace PolyNet {

	public class PacketItemHeld : PacketBehaviour {

		public GameObject holder;
		public int heldId;

		public PacketItemHeld() {
			id = 23;
		}

		public PacketItemHeld(PolyNetBehaviour b, GameObject g, int i) : base(b){
			id = 23;
			holder = g;
			heldId = i;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			PacketHelper.read (ref reader, ref holder);
			heldId = reader.ReadInt32 ();
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			PacketHelper.write (ref writer, holder);
			writer.Write (heldId);
			base.write (ref writer);
		}

	}

}
