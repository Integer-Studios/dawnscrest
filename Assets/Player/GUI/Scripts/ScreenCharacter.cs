using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PolyPlayer {

	public class ScreenCharacter : Screen {

		public GUIBoundInventory mainInventoryGUI;
		public Slider healthSlider;
		public Slider hungerSlider;
		public Slider thirstSlider;

		/*
		* 
		* Public Interface
		* 
		*/

		public override void onPush() {
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

		/*
		* 
		* Private
		* 
		*/

		private void Update() {
			GUIManager.player.syncVital (0, healthSlider);
			GUIManager.player.syncVital (1, hungerSlider);
			GUIManager.player.syncVital (2, thirstSlider);
		}
	}

}