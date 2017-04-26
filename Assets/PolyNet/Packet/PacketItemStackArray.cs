using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PolyItem;

namespace PolyNet {

	public class PacketItemStackArray : PacketBehaviour {

		public ItemStack[] stacks;

		public PacketItemStackArray() {
			id = 19;
		}

		public PacketItemStackArray(PolyNetBehaviour b, ItemStack[] s) : base(b){
			id = 19;
			stacks = s;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			PacketHelper.read (ref reader, ref stacks);
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			PacketHelper.write (ref writer, stacks);
			base.write (ref writer);
		}

	}

}
