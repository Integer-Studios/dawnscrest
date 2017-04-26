using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PolyItem;

namespace PolyNet {

	public class PacketSlotUpdate : PacketBehaviour {

		public int slotId;
		public ItemStack stack;

		public PacketSlotUpdate() {
			id = 14;
		}

		public PacketSlotUpdate(PolyNetBehaviour b, int sid, ItemStack s) : base(b){
			id = 14;
			slotId = sid;
			stack = s;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			slotId = reader.ReadInt32 ();
			PacketHelper.read (ref reader, ref stack);
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (slotId);
			PacketHelper.write (ref writer, stack);
			base.write (ref writer);
		}

	}

}
