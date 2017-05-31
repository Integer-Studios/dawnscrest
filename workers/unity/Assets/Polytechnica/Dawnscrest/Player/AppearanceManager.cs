using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.Player {

	public class AppearanceManager : MonoBehaviour {

		public Mesh maleEyes;

		public Mesh[] maleBuilds;
		public Mesh[] maleHair;
		public Mesh[] maleFacial;
		public Mesh[] maleEyebrows;

		public Material[] hairColors;
		public Material[] eyeColors;

		public static AppearanceManager manager;

		void Awake() {
			DontDestroyOnLoad (this);
		}

		private void OnEnable() {
			manager = this;
		}

		public static Mesh GetEyes(bool sex) {
			return manager.maleEyes;
		}

		public static Mesh GetBuild(bool sex, int k) {
			return manager.maleBuilds[k];
		}

		public static Mesh GetHair(bool sex, int k) {
			return manager.maleHair[k];
		}

		public static Mesh GetFacial(bool sex, int k) {
			return manager.maleFacial[k];
		}

		public static Mesh GetEyebrows(bool sex, int k) {
			return manager.maleEyebrows[k];
		}

		public static Material GetHairColor(int k) {
			return manager.hairColors[k];
		}

		public static Material GetEyeColor(int k) {
			return manager.eyeColors[k];
		}

		public static int GetHairOptions(bool sex) {
			return manager.maleHair.Length;
		}

		public static int GetEyebrowOptions(bool sex) {
			return manager.maleEyebrows.Length;
		}

		public static int GetFacialOptions(bool sex) {
			return manager.maleFacial.Length;
		}

		public static int GetHairColorOptions(bool sex) {
			return manager.hairColors.Length;
		}

		public static int GetEyeColorOptions(bool sex) {
			return manager.eyeColors.Length;
		}
	}

}