using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyItem {

	public class ItemSlot {
		
		public ItemStack stack;

		public ItemSlot() {
			stack = null;
		}

		public ItemSlot(ItemStack s) {
			stack = s;
		}

		public virtual bool canHold(ItemStack stack) {
			return true;
		}
			
	}

	public class WeightedSlot : ItemSlot {
		
		public int weight;

		public WeightedSlot(int w) : base() {
			weight = w;
		}

		public WeightedSlot(ItemStack s, int w) : base(s) {
			weight = w;
		}

		public override bool canHold(ItemStack stack) {
			return (ItemManager.getWeight(stack) <= weight);
		}
	}

}
