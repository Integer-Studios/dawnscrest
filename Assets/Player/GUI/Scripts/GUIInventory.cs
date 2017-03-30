using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyItem;

namespace PolyPlayer {

	public class GUIInventory : MonoBehaviour {

		public GUIItemSlot slotPrefab;
		public GUIWeightedSlot weightedSlotPrefab;
		protected GUIItemSlot[] slots;

		/*
		 * 
		 * Public Interface
		 * 
		 */

		public void setSlot(int i, ItemStack stack) {
			slots [i].setStack (stack);
		}

		public virtual void onSlotUpdate(int i) {
			
		}

		public void clear() {
			foreach (Transform t in transform)
				Destroy (t.gameObject);
		}

	}

}
