using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyItem {

	public class WeightedInventory : Inventory {

		public int ones = 0;
		public int twos = 0;
		public int threes = 0;

		/*
		* 
		* Private
		* 
		*/

		public override void Start() {
			slots = new WeightedSlot[size];
			int weight = 3;
			for (int i = 0; i < size; i++) {
				if (i == threes)
					weight = 2;
				if (i == threes + twos)
					weight = 1;
				slots [i] = new WeightedSlot (weight);
			}
			if (data != null)
				this.read (data);
		}


		public int getSlotWeight(int i) {
			return ((WeightedSlot)slots [i]).weight;
		}

	}

}