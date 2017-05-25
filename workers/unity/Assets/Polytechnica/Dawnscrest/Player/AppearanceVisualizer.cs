using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.Player {

	public class AppearanceVisualizer : MonoBehaviour {
		/*
		 * For use external to networking
		 */
		public void SetAppearance(AppearanceSet a) {
			// actually set appearance TODO
		}

	}

	public class AppearanceSet {
		public bool sex;
		int hairColor;
		int eyeColor;
		int build;
		int hair;
		int facialHair;
		// maybe we'll put clothes in here too later

		public static AppearanceSet GetSetFromUpdate(CharacterAppearance.Update update) {
			return null;
		}

		public static AppearanceSet GetSetFromData(CharacterAppearanceData data) {
			return null;
		}
	}

}