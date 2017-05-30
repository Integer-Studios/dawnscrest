using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Polytechnica.Dawnscrest.Menu {

	public class Parallax : MonoBehaviour {

		private ScrollRect scroller;
		private float position = 0f;
		// Use this for initialization
		void OnEnable () {
			scroller = GetComponent<ScrollRect> ();
		}
		
		// Update is called once per frame
		void Update () {
			position += Time.deltaTime/100f;
			if (position > 1)
				position = 1;
			scroller.horizontalNormalizedPosition = position;
		}
	}

}