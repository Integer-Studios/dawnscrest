using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyItem;

namespace PolyPlayer {

	public class ScreenRecipes : Screen {

		public GUIBoundInventory mainInventoryGUI;
		public GUIRecipeInventory RecipeInventory;

		private Craftable binding;

		/*
		* 
		* Public Interface
		* 
		*/

		public void openWithRecipes(Craftable c, List<Recipe> recipes) {
			binding = c;
			RecipeInventory.populate (binding, recipes);
			GUIManager.pushScreen (this);
			if (binding && binding.isLoaded ())
				GUIManager.craftingScreen.openWithRecipe (binding, binding.getRecipe ());
		}

		public void openWithRecipes(List<Recipe> recipes) {
			openWithRecipes (null, recipes);
		}

		public override void onPush() {
			GUIManager.player.bindInventoryToGUI (1, mainInventoryGUI);
		}

		public override void onPop() {

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
			
	}

}