using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;

namespace Polytechnica.Dawnscrest.Item {

	public class InteractableVisualizer : MonoBehaviour {

		[Require] private InteractableComponent.Reader interactableReader;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}


		public virtual string GetTooltip() {
			return "Test!";
		}


		public virtual bool IsInteractable() {
			return true;
		}

	}

}