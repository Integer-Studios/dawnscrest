using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.Item {

	public class ItemSlot {

		// Guarunteed to exist
		public ItemStack stack;

		public ItemSlot() {
			stack = new ItemStack();
		}

		public ItemSlot(ItemStack s) {
			stack = s;
		}

		public virtual bool CanHold(ItemStack stack) {
			return true;
		}

		public bool IsEmpty() {
			return stack.IsNull ();
		}

		/*
		 * When an inventory update is recieved, compare
		 * the new stack and avoid calling an update
		 * if the stack is unchanged
		 */
		public bool CompareAndUpdate(ItemStackData data) {
			if (stack.id != data.itemId || stack.quality != data.quality || stack.size != data.size) {
				stack = new ItemStack (data);
				return true;
			}
			return false;
		}


	}

}