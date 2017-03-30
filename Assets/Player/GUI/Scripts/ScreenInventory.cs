using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolyItem;

namespace PolyPlayer {

	public class ScreenInventory : Screen {

		public GUIBoundInventory mainInventoryGUI;
		public GUIBoundInventory otherInventoryGUI;

		/*
		* 
		* Public Interface
		* 
		*/

		public override void onPush() {
			GUIManager.player.bindInventoryToGUI (2, otherInventoryGUI);
			GUIManager.player.bindInventoryToGUI (1, mainInventoryGUI);
		}

		public override void onPop() {

		}

		public override void onRePush() {

		}

		public override void onClose() {

		}

		public override void processInput() {
			if (Input.GetKeyDown (KeyCode.Z)) {
				GUIManager.closeGUI ();
			}
		}

	}

}