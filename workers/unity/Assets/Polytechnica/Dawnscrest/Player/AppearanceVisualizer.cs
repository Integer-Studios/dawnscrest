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
			mats [1] = AppearanceManager.manager.eyeColors [a.eyeColor];
			eyes.GetComponent<SkinnedMeshRenderer> ().materials = mats;
		}

		public void SetFPSMode(bool b) {
			fpsMode = b;
		}
	}

}