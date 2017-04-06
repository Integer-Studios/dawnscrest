using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyItem {

	public class ReplaceCraftable : Craftable {

		/*
		* 
		* Private
		* 
		*/

		protected override void onComplete(Interactor i) {
			GameObject g = ItemManager.createItemForPlacing (recipe.output);
			g.GetComponent<Interactable> ().setTransform (transform);
			Destroy (gameObject);
			base.onComplete (i);
		}

	}

}