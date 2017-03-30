using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyItem;

namespace PolyPlayer {

	public class GUIRecipeInventory : GUIInventory {

		private Craftable binding;

		/*
		* 
		* Public Interface
		* 
		*/

		public void populate(Craftable b, List<Recipe> recipes) {
			binding = b;
			clear ();
			slots = new GUIRecipeSlot[recipes.Count];
			for (int i = 0; i < recipes.Count; i++) {
				GameObject g = Instantiate (slotPrefab.gameObject);
				g.transform.SetParent (transform, false);
				slots [i] = g.GetComponent<GUIRecipeSlot> ();
				((GUIRecipeSlot)slots [i]).setRecipe (binding, recipes [i]);
				slots [i].setParentInventory (this, i);
			}
		}

	}

}