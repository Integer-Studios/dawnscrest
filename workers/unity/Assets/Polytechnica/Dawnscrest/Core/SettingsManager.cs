using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Polytechnica.Dawnscrest.Core {

	public class SettingsManager {

		public static Polytechnica.Dawnscrest.Menu.MenuManager.HouseData house;
		public static Polytechnica.Dawnscrest.Player.AppearanceSet appearanceSet;
		public static Polytechnica.Dawnscrest.Player.AppearanceManager appearanceManager;

		private static bool initialized = false;

		public static void initialize() {
			if (!initialized) {
				initialized = true;
				SceneManager.LoadScene ("SettingsScene", LoadSceneMode.Additive);
				appearanceManager = Bootstrap.FindObjectOfType<Polytechnica.Dawnscrest.Player.AppearanceManager> ();
				if (appearanceManager == null) {
					Debug.Log ("Failed to find apperance manager");
				}
			}
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}

}
