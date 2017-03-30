using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolyItem;

namespace PolyPlayer {

	public class GUIWeightedSlot : GUIItemSlot {
		
		public Sprite weight1,weight2,weight3;
		public int weight = 1;

		public void setWeight(int w) {
			weight = w;
			switch (weight) {
			case 1:
				GetComponent<Image> ().sprite = weight1;
				break;
			case 2:
				GetComponent<Image> ().sprite = weight2;
				break;
			case 3:
				GetComponent<Image> ().sprite = weight3;
				break;
			default:
				GetComponent<Image> ().sprite = weight1;
				break;
			}
		}

		public override bool canHold(GUIItemStack stack) {
			if (stack == null)
				return true;
			return ItemManager.getWeight(stack.getStack()) <= weight;
		}

	}

}