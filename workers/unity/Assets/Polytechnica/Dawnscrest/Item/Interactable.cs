using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;

namespace Polytechnica.Dawnscrest.Item {

	public class Interactable : MonoBehaviour {

		[Require] private InteractableComponent.Writer interactableWriter;

		public float maxStrength;
		protected float strength;

		// Use this for initialization
		void OnEnable () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		protected virtual void OnComplete() {

		}

	}

}