using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PolyItem;

namespace PolyNet {

	public class PacketSetCraftableRecipe : PacketBehaviour {

		public GameObject target;
		public Recipe recipe;

		public PacketSetCraftableRecipe() {
			id = 22;
		}

		public PacketSetCraftableRecipe(PolyNetBehaviour b, GameObject g, Recipe r) : base(b){
			id = 22;
			target = g;
			recipe = r;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			PacketHelper.read (ref reader, ref target);
			PacketHelper.read (ref reader, ref recipe);
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			PacketHelper.write (ref writer, target);
			PacketHelper.write (ref writer, recipe);
			base.write (ref writer);
		}

	}

}
