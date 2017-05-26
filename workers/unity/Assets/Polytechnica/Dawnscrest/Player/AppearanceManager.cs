using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytechnica.Dawnscrest.Player {

	public class AppearanceManager : MonoBehaviour {

		public Mesh[] maleBuilds;
		public Mesh[] maleHair;
		public Mesh[] maleFacial;
		public Mesh[] maleEyebrows;
		public Material[] hairColors;
		public Material[] eyeColors;

		public static AppearanceManager manager;

		private void OnEnable() {
			manager = this;
		}
	}

}