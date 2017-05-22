using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.GUI {

	public class GUIManager : MonoBehaviour {

		public static HUD hud;
		public static Crosshair crosshair;

		private void OnEnable() {
			hud = GetComponentInChildren<HUD> ();
			crosshair = GetComponentInChildren<Crosshair> ();
		}

	}

}