using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PolyItem;

namespace PolyNet {

	public class PacketPlayerSetSlot : PacketBehaviour {

		public int bindingId;
		public int slotId;
		public ItemStack stack;

		public PacketPlayerSetSlot() {
			id = 15;
		}

		public PacketPlayerSetSlot(PolyNetBehaviour b, int bid, int sid, ItemStack s) : base(b){
			id = 15;
			bindingId = bid;
			slotId = sid;
			stack = s;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			bindingId = reader.ReadInt32 ();
			slotId = reader.ReadInt32 ();
			PacketHelper.read (ref reader, ref stack);
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			writer.Write (bindingId);
			writer.Write (slotId);
			PacketHelper.write (ref writer, stack);
			base.write (ref writer);
		}

	}

}
