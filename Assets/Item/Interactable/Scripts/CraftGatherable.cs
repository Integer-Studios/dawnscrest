using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyItem {

	public class CraftGatherable : Craftable {

		/*
		* 
		* Private
		* 
		*/

		protected override void onComplete(Interactor i) {
			for (int j = 0; j < recipe.output.size; j++) {
				GameObject g = ItemManager.createItem (recipe.output);
				g.GetComponent<Item> ().setPosition (transform.position + transform.up * 2f);
			}
			base.onComplete (i);
		}

	}

}