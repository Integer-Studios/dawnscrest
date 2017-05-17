using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;

namespace PolyItem {
	
	public class FueledCraftable : Craftable {

		//TOD fuel is a syncvar
		public float fuel = 0f;
		public float fuelConumptionRate = 0.1f;
		public Item[] fuelItems;

		protected float cookTime = 0f;
		protected bool isFueled = true;

		/*
		* 
		* Public Interface
		* 
		*/

		public override void setRecipe(Recipe r) {
			base.setRecipe (r);
			setMaxStrength (0f);
		}

		public override bool isInteractable(Interactor i) {
			return isFueled && base.isInteractable(i);
		}
			
		/*
		* 
		* Private
		* 
		*/

		protected virtual void OnCollisionEnter(Collision collision) {

			if (!PolyServer.isActive)
				return;
			
			Item i = collision.gameObject.GetComponent<Item> ();
			if (i == null)
				return;

			foreach (Item item in fuelItems) {
				if (i.id == item.id) {
					addFuel (5f);
					PolyNetWorld.destroy (i.gameObject);
					break;
				}
			}
		}

		protected override void Start() {
			if (fuel > 0)
				setFuled (true);
		}

		protected virtual void Update() {
			if (fuel <= 0f && isFueled)
				setFuled (false);
			else if (fuel > 0f && !isFueled)
				setFuled (true);
			
			if (!PolyServer.isActive)
				return;

			if (isFueled)
				fuel -= Time.deltaTime * fuelConumptionRate;
			
			if (isSatisfied ())
				cook();
		}

		protected virtual void cook() {
			cookTime += Time.deltaTime;
		}

		protected virtual void setFuled(bool f) {
			// turn particles/lights on and off
			isFueled = f;
		}

		protected virtual void addFuel(float f) {
			// increase fire size etc
			fuel += f;
			if (fuel > 0f && !isFueled)
				setFuled (true);
		}

		protected override void onComplete(Interactor i) {
			if (!isSatisfied ())
				return;
			ItemStack drop = new ItemStack (recipe.output);
			// make this more complex later
			drop.quality = (int)cookTime;

			for (int j = 0; j < recipe.output.size; j++) {
				GameObject g = ItemManager.createItem (drop);
				g.transform.position = transform.position + transform.up * 2f;
//				PolyServer.spawnObject (g);
			}
			base.onComplete (i);
		}

		public override JSONObject write() {
			JSONObject obj = base.write ();
			JSONObject metadata = new JSONObject (JSONObject.Type.OBJECT);
			metadata.AddField ("fuel", fuel);
			metadata.AddField ("cookTime", cookTime);

			obj.AddField ("metadata", metadata);
			return obj;
		}

		public override void read (JSONObject obj) {
			base.read (obj);
			JSONObject metadata = obj.GetField ("metadata");
			fuel = metadata.GetField ("fuel").n;
			cookTime = metadata.GetField ("cookTime").n;

		}

	}

}