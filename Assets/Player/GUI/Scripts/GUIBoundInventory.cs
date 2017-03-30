using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyItem;

namespace PolyPlayer {

	public class GUIBoundInventory : GUIInventory, InventoryListener {

		private Inventory inventory;
		private int bindingID;

		/*
		* 
		* Public Interface
		* 
		*/

		public void bind(Inventory inv, int id) {
			inventory = inv;
			bindingID = id;
			initializeSlots ();
			inventory.startListening (this, true);
		}
			
		public void inventoryListener_onSlotChange (Inventory inv, int i, ItemStack s) {
			slots [i].setStack (s);
		}

		public override void onSlotUpdate(int i) {
			GUIItemStack s = slots [i].getStack ();
			if (s == null)
				GUIManager.player.onSlotUpdate(bindingID, i, null);
			else
				GUIManager.player.onSlotUpdate(bindingID, i, s.getStack());
		}

		/*
		* 
		* Private
		* 
		*/

		private void initializeSlots() {
			clear ();
			slots = new GUIItemSlot[inventory.size];
			if (inventory is WeightedInventory) {
				for (int i = 0; i < inventory.size; i++) {
					GameObject g = Instantiate (weightedSlotPrefab.gameObject);
					g.transform.SetParent (transform, false);
					slots [i] = g.GetComponent<GUIWeightedSlot> ();
					((GUIWeightedSlot)slots [i]).setWeight (((WeightedInventory)inventory).getSlotWeight(i));
					slots [i].setParentInventory (this, i);
				}
			} else {
				for (int i = 0; i < inventory.size; i++) {
					GameObject g = Instantiate (slotPrefab.gameObject);
					g.transform.SetParent (transform, false);
					slots [i] = g.GetComponent<GUIItemSlot> ();
					slots [i].setParentInventory (this, i);
				}
			}
		}
	
	}

}