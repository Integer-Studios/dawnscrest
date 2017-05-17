using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PolyItem;

namespace PolyNet {

	public class PacketSetCraftableInput : PacketBehaviour {

		public GameObject target;
		public ItemStack[] stacks;

		public PacketSetCraftableInput() {
			id = 21;
		}

		public PacketSetCraftableInput(PolyNetBehaviour b, GameObject g, ItemStack[] s) : base(b){
			id = 21;
			target = g;
			stacks = s;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			PacketHelper.read (ref reader, ref target);
			PacketHelper.read (ref reader, ref stacks);
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			PacketHelper.write (ref writer, target);
			PacketHelper.write (ref writer, stacks);
			base.write (ref writer);
		}

	}

}
