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
			g.transform.position = transform.position;
			g.transform.localScale = transform.localScale;
			g.transform.rotation = transform.rotation;
//			PolyServer.destroy (gameObject);
//			PolyServer.spawnObject (g);
			base.onComplete (i);
		}

	}

}