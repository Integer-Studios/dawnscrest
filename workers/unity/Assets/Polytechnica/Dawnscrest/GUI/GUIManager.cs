using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.GUI {

	public class GUIManager : MonoBehaviour {

		public static HUD hud;

		private void OnEnable() {
			hud = GetComponentInChildren<HUD> ();
		}

	}

}