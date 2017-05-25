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
		public int hairColor;
		public int eyeColor;
		public int build;
		public int hair;
		public int facialHair;
		// maybe we'll put clothes in here too later

		public override string ToString ()
		{
			return string.Format ("[AppearanceSet]: sex={0}, hairColor={1}, eyeColor={2}, build={3}, hair={4}, facialHair={5}", sex, hairColor, eyeColor, build, hair, facialHair);
		}

		public static AppearanceSet GetSetFromUpdate(CharacterAppearance.Update update) {
			return null;
		}

		public static AppearanceSet GetSetFromData(CharacterAppearanceData data) {
			return null;
		}
	}

}