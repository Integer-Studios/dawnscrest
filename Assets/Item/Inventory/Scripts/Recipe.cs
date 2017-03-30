using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyItem {
		
	[System.Serializable]
	public class Recipe {

		public ItemStack output;
		public ItemStack[] input;
		public CraftingType type = CraftingType.Hand;

		/*
		* 
		* Public Interface
		* 
		*/

		public Recipe(ItemStack o, ItemStack[] i) {
			output = o;
			input = i;
		}

		public static Recipe unwrapRecipe(NetworkItemStack o, NetworkItemStackArray i) {
			if (o.id == -1)
				return null;
			Recipe r = new Recipe (ItemStack.unwrapNetworkStack (o), ItemStack.unwrapNetworkStackArray (i));
			return r;
		}

		public Recipe(Recipe r) {
			output = new ItemStack (r.output);
			input = new ItemStack[r.input.GetLength (0)];
			for (int i = 0; i < r.input.GetLength (0); i++) {
				input [i] = new ItemStack (r.input [i]);
			}
		}

	}

	public enum CraftingType {
		Hand,
		Fire,
	}
}