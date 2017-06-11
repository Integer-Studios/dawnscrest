using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.Item {

	public class ItemStack {

		public int id;
		public int quality;
		public int size;

		public ItemStack(ItemStack s) {
			id = s.id;
			quality = s.quality;
			size = s.size;
		}

		public ItemStack(int i, int q, int s) {
			id = i;
			quality = q;
			size = s;
		}

		public ItemStack(ItemStackData d) {
			id = d.itemId;
			quality = d.quality;
			size = d.size;
		}

		public ItemStack() {
			Nullify ();
		}

		public ItemStackData GetData() {
			return new ItemStackData(id,quality,size);
		}

		public bool Insert(ItemStack stack) {
			if (stack.id != id)
				return false;

			int newQuality = (int)(((float)((quality * size) + stack.quality)) / ((float)(size + 1)));
			size++;
			quality = newQuality;
			return true;
		}

		public void Decrease() {
			size--;
			if (size <= 0)
				Nullify ();
		}

		public bool IsNull() {
			return id == -1;
		}

		private void Nullify() {
			id = -1;
			quality = -1;
			size = -1;
		}

	}

}