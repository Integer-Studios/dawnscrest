using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.Player {

	public class AppearanceVisualizer : MonoBehaviour {

		/*
		 * Initialization from a reader data in networking
		 */
		public void InitializeFromData(CharacterAppearanceData data) {
			// unpack into an appearance set, call set appearance
		}

		/*
		 * Update related appearance setting
		 */
		public void setAppearanceFromUpdate(CharacterAppearance.Update update) {
			// unpack into an appearance set, call set appearance
		}

		/*
		 * For use external to networking
		 */
		public void setAppearance(AppearanceSet a) {
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
	}

}