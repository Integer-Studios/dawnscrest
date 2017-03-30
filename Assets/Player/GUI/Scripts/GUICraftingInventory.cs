using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyItem;

namespace PolyPlayer {

	public class GUICraftingInventory : GUIInventory {

		private Recipe recipe;
		private ScreenCrafting screen;

		/*
		* 
		* Public Interface
		* 
		*/

		public void populate(ScreenCrafting sc, Recipe r) {
			recipe = r;
			screen = sc;
			clear ();
			slots = new GUICraftingInputSlot[recipe.input.GetLength(0)];
			for (int i = 0; i < slots.GetLength(0); i++) {
				GameObject g = Instantiate (slotPrefab.gameObject);
				g.transform.SetParent (transform, false);
				slots[i] = g.GetComponentInChildren<GUICraftingInputSlot> ();
				((GUICraftingInputSlot)slots [i]).setRequirement (recipe.input [i]);
				slots [i].setParentInventory (this, i);
			}
		}

		public override void onSlotUpdate(int i) {
			screen.setSatisfied (getSatisfied());
		}

		public bool getSatisfied() {
			foreach (GUIItemSlot s in slots) {
				if (!((GUICraftingInputSlot)s).isSatisfied ())
					return false;
			}
			return true;
		}

		public void useRequirements() {
			foreach (GUIItemSlot s in slots) {
				((GUICraftingInputSlot)s).useRequirement ();
			}
		}

		public ItemStack[] getInput() {
			ItemStack[] input = new ItemStack[slots.GetLength (0)];
			for (int i = 0; i < slots.GetLength (0); i++) {
				if (slots [i].getStack () == null)
					input [i] = null;
				else
					input [i] = new ItemStack(slots [i].getStack ().getStack ());
			}
			return input;
		}

		public void loadInput(ItemStack[] inputs) {
			for (int i = 0; i < inputs.GetLength (0); i++) {
				slots [i].setStack (inputs [i]);
				((GUICraftingInputSlot)slots [i]).refresh ();
			}
		}
	}

}