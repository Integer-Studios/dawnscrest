using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PolyPlayer {
 
	public class MouseFollower : MonoBehaviour {

		/*
		* 
		* Private
		* 
		*/

		private void OnEnable() {
			transform.position = Input.mousePosition;
		}

		private void Update () {
			transform.position = Input.mousePosition;
		}

	}

}