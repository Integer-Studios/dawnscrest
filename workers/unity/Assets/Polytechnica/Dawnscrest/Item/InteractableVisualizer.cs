using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;

namespace Polytechnica.Dawnscrest.Item {

	public class InteractableVisualizer : MonoBehaviour {

		[Require] private InteractableComponent.Reader interactableReader;

		public float maxStrength;
		protected float strength;

		// Use this for initialization
		void OnEnable () {
			interactableReader.ComponentUpdated += OnInteractableUpdated;
			maxStrength = interactableReader.Data.maxStrength;
			strength = interactableReader.Data.strength;

		}

		void OnDisable () {
			interactableReader.ComponentUpdated -= OnInteractableUpdated;
		}


		public virtual string GetTooltip() {
			return "Interact";
		}

		public virtual float GetPercent() {
			return strength/maxStrength;
		}

		public virtual bool IsInteractable() {
			return true;
		}

		private void OnInteractableUpdated(InteractableComponent.Update update) {
			if (update.maxStrength.HasValue)
				maxStrength = update.maxStrength.Value;
			if (update.strength.HasValue)
				strength = update.strength.Value;
		}

	}

}