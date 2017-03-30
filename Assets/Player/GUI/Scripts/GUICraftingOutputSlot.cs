using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PolyItem;

namespace PolyPlayer {
	
	public class GUICraftingOutputSlot : GUIItemSlot {

		private Recipe recipe;
		private bool isSatisfied;
		private ScreenCrafting screen;

		/*
		* 
		* Public Interface
		* 
		*/

		public void setRecipe(ScreenCrafting sc, Recipe r) {
			screen = sc;
			recipe = r;
			setStack (recipe.output);
			isSatisfied = false;
		}

		public override bool OnStackPointerDown(PointerEventData data) {
			if (data.button == PointerEventData.InputButton.Right)
				return false;
			if (GUIManager.mouseFollower.activeSelf)
				return false;
			
			return isSatisfied && !screen.isBound();
		}

		public override bool OnStackPointerUp(PointerEventData data) {
			return isSatisfied && !screen.isBound();
		}

		public override bool OnStackPointerEnter(PointerEventData eventData) {
			return isSatisfied && !screen.isBound();
		}

		public override bool OnStackPointerExit(PointerEventData eventData) {
			return isSatisfied && !screen.isBound();
		}

		public void setSatisfied(bool s) {
			isSatisfied = s;
		}

		public override void onStackUpdate() {
			if (getStack() == null || getStack().getStack().id != recipe.output.id || getStack().getStack().size < recipe.output.size)
				screen.useRequirements ();
			
			base.onStackUpdate ();
		}

	}
}
