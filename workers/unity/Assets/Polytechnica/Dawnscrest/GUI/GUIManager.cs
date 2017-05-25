using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.GUI {

	public class GUIManager : MonoBehaviour {

		public static HUD hud;
		public static Crosshair crosshair;
		public static Photobooth photobooth;

		private void OnEnable() {
			hud = GetComponentInChildren<HUD> ();
			crosshair = GetComponentInChildren<Crosshair> ();
			photobooth = FindObjectOfType<Photobooth> ();
//			canvasGroup = GetComponent<CanvasGroup> ();
//			canvasGroup.alpha = 0F;
		}

//		public static void Show() {
//			GUIManager.canvasGroup.alpha = 1F;
//		}

	}

}