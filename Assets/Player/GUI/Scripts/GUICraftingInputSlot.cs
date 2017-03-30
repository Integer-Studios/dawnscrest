using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolyItem;

namespace PolyPlayer {
	
	public class GUICraftingInputSlot : GUIItemSlot {

		public Image reqSprite;
		public Text reqSize;
		public Image reqWeight;

		private ItemStack requirement;

		/*
		* 
		* Public Interface
		* 
		*/

		public void setRequirement(ItemStack r) {
			requirement = r;

			reqSprite.sprite = ItemManager.getSprite(requirement);
			switch (ItemManager.getWeight(requirement)) {
			case 1:
				reqWeight.sprite = stackPrefab.weight1;
				break;
			case 2:
				reqWeight.sprite = stackPrefab.weight2;
				break;
			case 3:
				reqWeight.sprite = stackPrefab.weight3;
				break;
			}
			reqSize.text = "" + requirement.size;
		}

		public override void onStackUpdate() {
			base.onStackUpdate ();
			refresh ();
		}

		public bool isSatisfied() {
			GUIItemStack s = getStack ();
			if (s == null)
				return false;

			ItemStack stack = s.getStack ();
			if (stack.id != requirement.id)
				return false;
			if (stack.size < requirement.size)
				return false;

			return true;
		}

		public void useRequirement() {
			getStack ().setSize (getStack ().getStack ().size - requirement.size);
			refresh ();
		}

		public void refresh() {
			int size = requirement.size;

			GUIItemStack s = getStack ();
			if (s != null) {
				ItemStack stack = s.getStack ();
				if (stack.id == requirement.id) {
					size -= stack.size;
				}
			}
			if (size < 1)
				reqSize.text = "";
			else
				reqSize.text = "" + size;
		}
	}
}
