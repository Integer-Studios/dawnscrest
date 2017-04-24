using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;

namespace PolyItem {

	public class Workable : Interactable {

		public ToolType type = ToolType.Hand;
		public bool destroyOnInteract;

		/*
		* 
		* Public Interface
		* 
		*/

		public override void interact(Interactor i, float f) {
			if (isCompatable (i))
				base.interact (i, f);
		}

		public override bool isInteractable(Interactor i) {
			return isCompatable (i);
		}

		/*
		* 
		* Private
		* 
		*/

		protected override void onComplete(Interactor i) {
			if (destroyOnInteract)
				PolyNetWorld.destroy (gameObject);
			else
				strength = maxStrength;
		}

		private bool isCompatable(Interactor i) {
			if (type == ToolType.Hand)
				return true;

			ItemStack s = i.interactor_getItemInHand ();

			if (s == null)
				return false;

			if (!ItemManager.isTool(s))
				return false;

			if (ItemManager.getToolType(s) == type)
				return true;
			else
				return false;
		}
	}

}