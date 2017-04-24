using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;
using UnityEngine.Rendering;

namespace PolyItem {

	public class ItemManager : MonoBehaviour {

		// Vars : public, protected, private, hide
		public Item[] itemRegister;
		public AnimationSet[] animationSetRegister;
		public Item nullItemObj;

		private static Item[] items;
		private static AnimationSet[] animationSets;
		public static Item nullItem;
		/*
		* 
		* Public
		* 
		*/

		public static int getMaxStackSize(ItemStack s) {
			return getMaxStackSize (s.id);
		}

		public static int getWeight(ItemStack s) {
			return getWeight (s.id);
		}

		public static string getName(ItemStack s) {
			return getName (s.id);
		}

		public static Sprite getSprite(ItemStack s) {
			return getSprite (s.id);
		}

		public static bool isTool(ItemStack s) {
			return isTool (s.id);
		}

		public static ToolType getToolType(ItemStack s) {
			return getToolType (s.id);
		}

		public static bool isConsumable(ItemStack s) {
			return isConsumable (s.id);
		}

		public static ConsumableType getConsumableType(ItemStack s) {
			return getConsumableType (s.id);
		}

		public static GameObject createItem(ItemStack s) {
			if (s == null) {
				GameObject g = Instantiate (nullItem.gameObject);
				return g;
			} else {
				GameObject g = Instantiate(getItem (s.id).gameObject);
				g.GetComponent<Item> ().quality = s.quality;
				return g;
			}
		}

		public static GameObject createItemForPlacing(ItemStack s) {
			if (s == null) {
				GameObject g = Instantiate (nullItem.gameObject);
				return g;
			} else {
				Item i = getItem (s.id);
				GameObject g;
				if (i.onPlaced != null)
					g = Instantiate (i.onPlaced);
				else {
					g = Instantiate (i.gameObject);
					g.GetComponent<Item> ().quality = s.quality;
				}
				return g;
			}
		}

		public static float getConsumableNutrition(ItemStack s) {
			return ((ItemConsumable)getItem(s.id)).nutrition;
		}


		public static int getMaxStackSize(int id) {
			return getItem (id).maxStackSize;
		}

		public static int getWeight(int id) {
			Item i = getItem (id);
			if (i != null) {
				return getItem (id).weight;
			} else {
				Debug.Log ("Failed to get weight for id: " + id);
				return 1;
			}
		}

		public static string getName(int id) {
			return getItem (id).name;
		}

		public static Sprite getSprite(int id) {
			return getItem (id).sprite;
		}

		public static bool isTool(int id) {
			return getItem (id) is ItemTool;
		}

		public static ToolType getToolType(int id) {
			if (!isTool (id))
				return ToolType.Hand;

			return ((ItemTool)getItem (id)).toolType;
		}

		public static bool isConsumable(int id) {
			return getItem (id) is ItemConsumable;
		}

		public static ConsumableType getConsumableType(int id) {
			if (!isConsumable (id))
				return ConsumableType.None;

			return ((ItemConsumable)getItem (id)).consumableType;
		}

		/*
		* 
		* Public
		* 
		*/

		private void Start() {
			animationSets = animationSetRegister;
			nullItem = nullItemObj;
			PolyNetWorld.registerPrefab(nullItem.gameObject);
			setItems (itemRegister);
		}

		private static void setItems(Item[] it) {
			items = it;
			foreach (Item i in items) {
				PolyNetWorld.registerPrefab(i.gameObject);
				if (i.onPlaced != null)
					PolyNetWorld.registerPrefab(i.onPlaced);
			}
		}

		private static Item getItem(int id) {
			foreach (Item i in items) {
				if (i.id == id)
					return i;
			}
			return null;
		}
	}

	[System.Serializable]
	public class AnimationSet {
		public string name;
		public AnimationClip swing, interact;
	}

}
