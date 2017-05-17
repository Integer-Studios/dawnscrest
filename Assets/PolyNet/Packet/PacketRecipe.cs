using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PolyItem;

namespace PolyNet {

	public class PacketRecipe : PacketBehaviour {

		public Recipe recipe;

		public PacketRecipe() {
			id = 20;
		}

		public PacketRecipe(PolyNetBehaviour b, Recipe r) : base(b){
			id = 20;
			recipe = r;
		}

		public override void read(ref BinaryReader reader, PolyNetPlayer sender) {
			PacketHelper.read (ref reader, ref recipe);
			base.read (ref reader, sender);
		}

		public override void write(ref BinaryWriter writer) {
			PacketHelper.write (ref writer, recipe);
			base.write (ref writer);
		}

	}

}
