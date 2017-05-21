using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Polytechnica.Realms.Core;

namespace Polytechnica.Realms.Player {

	// Server authoritative player functionality
	public class PlayerOnline : MonoBehaviour {

		[Require] private WorldTransform.Reader WorldTransformReader;

		private float timeout;
		private CharacterController character;

		// Use this for initialization
		void OnEnable () {
			WorldTransformReader.ComponentUpdated += Heatbeat;
			character = GetComponent<CharacterController> ();
			timeout = 0f;
		}

		// Update is called once per frame
		void OnDisable () {
			WorldTransformReader.ComponentUpdated -= Heatbeat;
		}

		private void Update() {
			timeout += Time.deltaTime;
			if (timeout > 5f)
				Logout ();
		}

		private void Heatbeat(WorldTransform.Update update) {
			timeout = 0f;
		}

		private void Logout() {
			character.setToNPC ();
		}
	}

}