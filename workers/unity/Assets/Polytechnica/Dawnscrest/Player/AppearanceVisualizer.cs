using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.Player {

	public class AppearanceVisualizer : MonoBehaviour {

		public GameObject body;
		public GameObject hair;
		public GameObject facial;
		public GameObject brows;
		public GameObject eyes;

		private bool fpsMode = false;

		/*
		 * For use external to networking
		 */
		public void SetAppearance(AppearanceSet a) {
			if (a == null)
				return;
			
			body.GetComponent<SkinnedMeshRenderer>().sharedMesh = AppearanceManager.GetBuild(a.sex, a.build);
			eyes.GetComponent<SkinnedMeshRenderer>().sharedMesh = AppearanceManager.GetEyes(a.sex);

			hair.GetComponent<SkinnedMeshRenderer>().sharedMesh = AppearanceManager.GetHair(a.sex, a.hair);
			facial.GetComponent<SkinnedMeshRenderer>().sharedMesh = AppearanceManager.GetFacial(a.sex, a.facialHair);
			brows.GetComponent<SkinnedMeshRenderer>().sharedMesh = AppearanceManager.GetEyebrows(a.sex, a.eyebrows);

			if (fpsMode)
				return;
				
			Material[] mats;

			mats = hair.GetComponent<SkinnedMeshRenderer> ().materials;
			mats [0] = AppearanceManager.manager.hairColors [a.hairColor];
			hair.GetComponent<SkinnedMeshRenderer> ().materials = mats;
			brows.GetComponent<SkinnedMeshRenderer> ().materials = mats;
			facial.GetComponent<SkinnedMeshRenderer> ().materials = mats;

			mats = eyes.GetComponent<SkinnedMeshRenderer> ().materials;
			mats [0] = AppearanceManager.manager.eyeColors [a.eyeColor];
			eyes.GetComponent<SkinnedMeshRenderer> ().materials = mats;
		}

		public void SetFPSMode(bool b) {
			fpsMode = b;
		}
	}

	public class AppearanceSet {
		public bool sex;
		public int hairColor;
		public int eyeColor;
		public int build;
		public int hair;
		public int facialHair;
		public int eyebrows;
		// maybe we'll put clothes in here too later

		public AppearanceSet() {

		}

		public AppearanceSet(bool s, int hc, int ec, int b, int h, int f, int e) {
			sex = s;
			hairColor = hc;
			eyeColor = ec;
			build = b;
			hair = h;
			facialHair = f;
			eyebrows = e;
		}

		public override string ToString () {
			return string.Format ("[AppearanceSet]: sex={0}, hairColor={1}, eyeColor={2}, build={3}, hair={4}, facialHair={5}, eyebrows={6}", sex, hairColor, eyeColor, build, hair, facialHair, eyebrows);
		}

		public static AppearanceSet GetSetFromUpdate(CharacterAppearance.Update update) {
			return new AppearanceSet(
				update.sex.Value, 
				(int)update.hairColor.Value, 
				(int)update.eyeColor.Value, 
				(int)update.build.Value, 
				(int)update.hair.Value, 
				(int)update.facialHair.Value, 
				(int)update.eyebrow.Value
			);
		}

		public static AppearanceSet GetSetFromData(CharacterAppearanceData data) {
			return new AppearanceSet(
				data.sex, 
				(int)data.hairColor, 
				(int)data.eyeColor, 
				(int)data.build, 
				(int)data.hair, 
				(int)data.facialHair, 
				(int)data.eyebrow
			);
		}

		public static AppearanceSet GetGeneticVariation(AppearanceSet a) {
			AppearanceSet aNew = new AppearanceSet();
			aNew.build = Random.Range (0, 3);
			aNew.hair = Random.Range (0, AppearanceManager.GetHairOptions (true));
			aNew.facialHair = Random.Range (0, AppearanceManager.GetFacialOptions (true));
			aNew.eyebrows = Random.Range (0, AppearanceManager.GetEyebrowOptions (true));
			aNew.hairColor = a.hairColor;
			aNew.eyeColor = a.eyeColor;
			aNew.sex = true;
			return aNew;
		}
	}

}