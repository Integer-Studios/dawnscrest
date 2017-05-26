using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.Player {

	public class AppearanceManager : MonoBehaviour {

		public Mesh[] maleBuilds;
		public Mesh[] maleBuildsEyes;

		public Mesh[] m0Hair;
		public Mesh[] m1Hair;
		public Mesh[] m2Hair;
		public Mesh[] m0Facial;
		public Mesh[] m1Facial;
		public Mesh[] m2Facial;
		public Mesh[] m0Eyebrows;
		public Mesh[] m1Eyebrows;
		public Mesh[] m2Eyebrows;

		public Material[] hairColors;
		public Material[] eyeColors;

		public static AppearanceManager manager;

		private void OnEnable() {
			manager = this;
		}

		public static Mesh GetBuild(bool sex, int build) {
			return manager.maleBuilds[build];
		}

		public static Mesh GetEyes(bool sex, int build) {
			return manager.maleBuildsEyes[build];
		}



		public static Mesh GetHair(bool sex, int build, int k) {
			switch (build) {
			case 0:
				return manager.m0Hair[k];
			case 1:
				return manager.m1Hair[k];
			case 2:
				return manager.m2Hair[k];
			default:
				return manager.m0Hair[k];
			}
		}

		public static Mesh GetFacial(bool sex, int build, int k) {
			switch (build) {
			case 0:
				return manager.m0Facial[k];
			case 1:
				return manager.m1Facial[k];
			case 2:
				return manager.m2Facial[k];
			default:
				return manager.m0Facial[k];
			}
		}

		public static Mesh GetEyebrows(bool sex, int build, int k) {
			switch (build) {
			case 0:
				return manager.m0Eyebrows[k];
			case 1:
				return manager.m1Eyebrows[k];
			case 2:
				return manager.m2Eyebrows[k];
			default:
				return manager.m0Eyebrows[k];
			}
		}

		public static Material GetHairColor(int k) {
			return manager.hairColors[k];
		}

		public static Material GetEyeColor(int k) {
			return manager.eyeColors[k];
		}
	}

}