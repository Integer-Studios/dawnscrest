using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyItem {

	public class HotBarInventory : WeightedInventory, ISaveable {

		public override void Start() {
			slots = new WeightedSlot[3];
			slots [0] = new WeightedSlot (2);
			slots [1] = new WeightedSlot (3);
			slots [2] = new WeightedSlot (2);
			if (data != null)
				this.read (data);
		}

		public override bool insert(Item item) {
			ItemStack stack = new ItemStack (item);
			for (int i = 0; i < slots.GetLength(0); i++) {
				if (insertSlot (i, stack))
					return true;
			}

			if (slots [0].stack == null && slots [0].canHold (stack)) {
				setSlot (0, stack);
				return true;
			}

			if (slots [2].stack == null && slots [2].canHold(stack)) {
				setSlot (2, stack);
				return true;
			}

			if (slots [1].stack == null && slots [1].canHold(stack)) {
				setSlot (1, stack);
				return true;
			}

			return false;
		}

		public void switchBack(bool rightHand) {
			int hand = 0;
			if (rightHand)
				hand = 2;
			
			ItemStack h = getSlotCopy (hand);
			ItemStack b = getSlotCopy (1);
			setSlot (hand, b);
			setSlot (1, h);
		}

	}

}