using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Improbable.Entity.Component;
using Polytechnica.Dawnscrest.Core;

namespace Polytechnica.Dawnscrest.Item {

	public class Interactable : MonoBehaviour {

		[Require] private InteractableComponent.Writer interactableWriter;

		public float maxStrength;
		protected float strength;

		protected virtual void OnEnable() {
			interactableWriter.CommandReceiver.OnInteract.RegisterResponse (OnInteractRequest);

			// set the component data to the configured data
			strength = maxStrength;
			interactableWriter.Send (new InteractableComponent.Update ()
				.SetMaxStrength(maxStrength)
				.SetStrength(strength)
			);
		}

		public virtual void Interact(float f) {
			strength -= f;
			if (strength <= 0f)
				OnComplete ();

			interactableWriter.Send (new InteractableComponent.Update ()
				.SetStrength(strength)
			);
		}

		protected virtual void OnComplete() {

		}

		private Nothing OnInteractRequest(InteractRequest r, ICommandCallerInfo callerInfo) {
			Interact (r.amount);
			return new Nothing ();
		}

	}

}