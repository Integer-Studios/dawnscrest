using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolyWorld;

namespace PolyPlayer {

	public class ScreenSettings : Screen {

		public Slider renderSlider;
		public Slider sensitivitySlider;
		public Toggle fancyToggle;
		public float maxRender, minRender;
		public float maxSensitivity, minSensitivity;

		/*
		* 
		* Public Interface
		* 
		*/

		public override void onPush() {
			
		}

		public override void onPop() {

		}

		public override void onRePush() {

		}

		public override void onClose() {

		}

		public void onRenderChanged() {
			WorldTerrain.setRenderDistance (renderSlider.value + minRender);
		}

		public void onSensitivityChanged() {
			GUIManager.player.mouseSensitivity = sensitivitySlider.value + minSensitivity;
		}

		public void onFancyChanged() {
			GUIManager.player.setFancyGraphics (fancyToggle.isOn);
		}

		public override void processInput() {
			if (Input.GetKeyDown (KeyCode.Tab)) {
				GUIManager.closeGUI ();
			}
		}

		private void Start() {
			renderSlider.maxValue = maxRender - minRender;
			renderSlider.value = 100f;
			sensitivitySlider.maxValue = maxSensitivity - minSensitivity;
			sensitivitySlider.value = 8f;
		}

	}

}
