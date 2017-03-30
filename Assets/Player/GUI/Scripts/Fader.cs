using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PolyPlayer {

	public class Fader : MonoBehaviour {

		public float fadeTime = 10f;

		private float alpha = 1f;

		private void Update() {
			alpha -= (1f/fadeTime) * Time.deltaTime;
			GetComponent<Text> ().color = new Color (GetComponent<Text> ().color.r, GetComponent<Text> ().color.g, GetComponent<Text> ().color.b, alpha);
			if (alpha < 0f)
				Destroy (gameObject);
		}

	}

}
