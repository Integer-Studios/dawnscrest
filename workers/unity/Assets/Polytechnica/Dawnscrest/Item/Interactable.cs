using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Improbable.Entity.Component;
using Improbable.Unity.Core;
using Improbable.Worker;
using Improbable;
using Polytechnica.Dawnscrest.Core;

namespace Polytechnica.Dawnscrest.Item {

	public class Interactable : MonoBehaviour {

		[Require] protected InteractableComponent.Writer interactableWriter;

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

		protected virtual void OnDisable() {
			interactableWriter.CommandReceiver.OnInteract.DeregisterResponse ();
		}

		public virtual void Interact(EntityId i, float f) {
			strength -= f;
			if (strength <= 0f)
				OnComplete (i);

			interactableWriter.Send (new InteractableComponent.Update ()
				.SetStrength(strength)
			);
		}

		protected virtual void OnComplete(EntityId i) {

		}

		private Nothing OnInteractRequest(InteractRequest r, ICommandCallerInfo callerInfo) {
			Interact (r.entityId, r.amount);
			return new Nothing ();
		}

	}

}