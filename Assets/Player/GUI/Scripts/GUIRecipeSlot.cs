using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyItem;

namespace PolyPlayer {
	public class GUIRecipeSlot : GUIItemSlot {

		private Recipe recipe;
		private Craftable binding;

		/*
		* 
		* Public Interface
		* 
		*/

		public void setRecipe(Craftable b, Recipe r) {
			binding = b;
			recipe = r;
			setStack (recipe.output);
		}

		public override bool OnStackPointerDown(PointerEventData data) {
			return false;
		}

		public override bool OnStackPointerUp(PointerEventData data) {
			GUIManager.craftingScreen.openWithRecipe (binding, recipe);
			return false;
		}

		public override bool OnStackPointerEnter(PointerEventData eventData) {
			return false;
		}

		public override bool OnStackPointerExit(PointerEventData eventData) {
			return false;
		}

	}
}
