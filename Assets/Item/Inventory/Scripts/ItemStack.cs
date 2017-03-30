using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyItem {

	[System.Serializable]
	public class ItemStack {
		
		public int id;
		public int quality;
		public int size;

		/*
		* 
		* Public Interface
		* 
		*/

		public static ItemStack unwrapNetworkStack(NetworkItemStack s) {
			ItemStack stack = new ItemStack (s.id, s.quality, s.size);
			if (stack.id == -1)
				stack = null;
			return stack;
		}

		public static ItemStack[] unwrapNetworkStackArray(NetworkItemStackArray s) {
			if (s.id.GetLength (0) == 0)
				return null;
			
			ItemStack[] stacks = new ItemStack[s.id.GetLength (0)];
			for (int i = 0; i < stacks.GetLength (0); i++) {
				ItemStack stack = new ItemStack (s.id[i], s.quality[i], s.size[i]);
				if (stack.id == -1)
					stack = null;
				stacks [i] = stack;
			}
			return stacks;
		}

		public ItemStack(ItemStack s) {
			id = s.id;
			quality = s.quality;
			size = s.size;
		}

		public ItemStack(ItemStack s, int si) {
			id = s.id;
			quality = s.quality;
			size = si;
		}

		public ItemStack(Item i) {
			id = i.id;
			quality = i.getQuality();
			size = 1;
		}

		public ItemStack(int i, int q, int s) {
			id = i;
			quality = q;
			size = s;
		}
			
		public ItemStack() {
			nullify ();
		}

		public JSONObject serializeJSON() {
			JSONObject itemJSON = new JSONObject (JSONObject.Type.OBJECT);
			itemJSON.AddField ("id", id);
			itemJSON.AddField ("quality", quality);
			itemJSON.AddField ("size", size);
			return itemJSON;
		}

		public ItemStack(JSONObject obj) {
			this.id = (int)obj.GetField ("item").n;
			this.size = (int)obj.GetField ("size").n;
			this.quality = (int)obj.GetField ("quality").n;
		}

		public bool insert(ItemStack stack) {
			if (stack.id != id || size >= ItemManager.getMaxStackSize(stack))
				return false;

			int newQuality = (int)(((float)((quality * size) + stack.quality)) / ((float)(size + 1)));

			size++;
			quality = newQuality;

			return true;
		}

		public void decrease() {
			size--;
			if (size <= 0)
				nullify ();
		}

		/*
		* 
		* Private
		* 
		*/

		private void nullify() {
			id = -1;
			quality = -1;
			size = -1;
		}

	}

	public struct NetworkItemStack {
		
		public int id;
		public int quality;
		public int size;

		public NetworkItemStack(ItemStack s) {
			if (s == null) {
				id = -1;
				quality = -1;
				size = -1;
			} else {
				id = s.id;
				quality = s.quality;
				size = s.size;
			}
		}
			
	}
		
	public struct NetworkItemStackArray {
		public int[] id;
		public int[] quality;
		public int[] size;

		public NetworkItemStackArray(ItemStack[] stacks) {
			
			if (stacks == null) {
				id = new int[0];
				quality = new int[0];
				size = new int[0];
				return;
			}

			id = new int[stacks.GetLength (0)];
			quality = new int[stacks.GetLength (0)];
			size = new int[stacks.GetLength (0)];
			for (int i = 0; i < stacks.GetLength (0); i++) {
				if (stacks[i] == null) {
					id[i] = -1;
					quality[i] = -1;
					size[i] = -1;
				} else {
					id[i] = stacks[i].id;
					quality[i] = stacks[i].quality;
					size[i] = stacks[i].size;
				}
			}
		}
	}

}