using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyItem;

namespace PolyPlayer {

	public class ScreenCrafting : Screen {

		public GUIBoundInventory mainInventoryGUI;
		public GUICraftingInventory craftingInventory;
		public GUICraftingOutputSlot output;

		private Recipe recipe;
		private Craftable binding;

		/*
		* 
		* Public Interface
		* 
		*/

		public void openWithRecipe(Craftable b, Recipe r) {
			binding = b;
			recipe = r;
			craftingInventory.populate (this, recipe);
			output.setRecipe (this, recipe);
			output.setSatisfied (craftingInventory.getSatisfied ());

			if (binding && binding.isLoaded ())
				craftingInventory.loadInput (binding.getInput ());
			else if (binding && !binding.isLoaded ())
				GUIManager.player.setCraftableRecipe (binding, recipe);

			GUIManager.pushScreen (this);
		}

		public override void onPush() {
			GUIManager.player.bindInventoryToGUI (1, mainInventoryGUI);
		}

		public override void onPop() {
			if (binding)
				GUIManager.player.setCraftableRecipe (binding, null);
		}

		public override void onRePush() {

		}

		public override void onClose() {

		}

		public override void processInput() {
			if (Input.GetKeyDown (KeyCode.C)) {
				GUIManager.closeGUI ();
			}
		}

		public void setSatisfied(bool s) {
			output.setSatisfied (s);
			if (binding)
				GUIManager.player.setCraftableInput (binding, craftingInventory.getInput ());
		}

		public void useRequirements() {
			craftingInventory.useRequirements ();
			output.setRecipe (this, recipe);
			output.setSatisfied (craftingInventory.getSatisfied ());
			if (binding)
				GUIManager.player.setCraftableInput (binding, craftingInventory.getInput ());
		}

		public bool isBound() {
			return binding != null;
		}

		public void onCancelBtn() {
			GUIManager.popScreen ();
		}
	}

}