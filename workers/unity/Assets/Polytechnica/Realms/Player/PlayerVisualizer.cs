using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Polytechnica.Realms.Core;

namespace Polytechnica.Realms.Player {

	// Client Non-Authoritative player functionality
	public class PlayerVisualizer : MonoBehaviour {

		[Require] private WorldTransform.Reader WorldTransformReader;
		[Require] private DynamicTransform.Reader DynamicTransformReader;
		[Require] private PlayerAnim.Reader PlayerAnimReader;

		public float ipPositionAllowance;
		public float ipRotationAllowance;

		// Velocity
		private Vector3 Velocity;
		private float RotationalVelocity;

		// Animation
		private bool RightHand;
		private bool IsConsuming;
		private bool IsInteracting;
		private Animator Anim;
		private Rigidbody RigidBody;
		private bool ShouldJump;
		private bool Grounded = false;
		private GameObject GroundObject;
		private float Pitch;

		//References
		private PlayerController Controller;

		/*
		 * 
		 * Start
		 * 
		 */

		void OnEnable () {

			// Register Listeners
			WorldTransformReader.ComponentUpdated += OnWorldTransformUpdated;
			DynamicTransformReader.ComponentUpdated += OnDynamicTransformUpdated;
			PlayerAnimReader.JumpTriggered += OnJump;
			PlayerAnimReader.PitchUpdated += OnPitchUpdated;

			// Initialize transform to Spatial
			transform.position = WorldTransformReader.Data.position.ToVector3();
			transform.eulerAngles = MathHelper.toVector3(WorldTransformReader.Data.rotation);
			transform.localScale = MathHelper.toVector3 (WorldTransformReader.Data.scale);

			// Initialize References
			Controller = GetComponent<PlayerController> ();
			Anim = GetComponent<Animator> ();
			RigidBody = GetComponent<Rigidbody> ();
			RigidBody.interpolation = RigidbodyInterpolation.Interpolate;
		}

		void OnDisable () {
			// UnRegister Listeners
			WorldTransformReader.ComponentUpdated -= OnWorldTransformUpdated;
			DynamicTransformReader.ComponentUpdated -= OnDynamicTransformUpdated;
			PlayerAnimReader.JumpTriggered -= OnJump;
			PlayerAnimReader.PitchUpdated -= OnPitchUpdated;
		}

		/*
		 * 
		 * Update
		 * 
		 */

		// Update
		private void Update() {
			if (WorldTransformReader.HasAuthority)
				return;
			UpdateLocomotion ();
		}

		private void LateUpdate() {
			if (PlayerAnimReader.HasAuthority)
				return;

			// Update Pitch Visualization
			Controller.Hip.transform.eulerAngles = new Vector3 (Pitch, Controller.transform.eulerAngles.y, Controller.Hip.transform.eulerAngles.z);
		}

		// Visualize Locomotion
		private void UpdateLocomotion() {

			// Update Grounded
			RaycastHit hit;
			if (Physics.Raycast (transform.position+transform.up, -transform.up, out hit, 2f)) {
				GroundObject = hit.collider.gameObject;
				Anim.SetBool ("Grounded", true);
				Grounded = true;
			} else {
				Anim.SetBool ("Grounded", false);
				Grounded = false;
			}

			// Enact Move
			RigidBody.velocity = transform.TransformDirection(Velocity) + new Vector3(0f, RigidBody.velocity.y, 0f);

			// Enact Jump
			if (ShouldJump) {
				RigidBody.velocity += new Vector3 (0f, Controller.JumpSpeed, 0f);
				ShouldJump = false;
			}

			// Enact Turn
			transform.Rotate (Vector3.up * RotationalVelocity);

			// Set Anim Params
			Anim.SetFloat ("Vertical", Velocity.z / Controller.MaxSpeed);
			Anim.SetFloat ("Horizontal", RotationalVelocity / Controller.MaxRotation);
		}


		/*
		 * 
		 * Networking
		 * 
		 */

		// Visualize Transform Update
		private void OnWorldTransformUpdated(WorldTransform.Update update) {
			if (WorldTransformReader.HasAuthority)
				return;

			// Update Position within ipAllowance
			if (update.position.HasValue) {
				Vector3 pos = update.position.Value.ToVector3 ();
				if (Vector3.Distance (pos, transform.position) > ipPositionAllowance && Grounded)
					transform.position = Vector3.Lerp(transform.position, pos, 0.5f);
			}

			// Update Rotation Within ipAllowance
			if (update.rotation.HasValue) {
				Vector3 rot = MathHelper.toVector3 (update.rotation.Value);
				if (Mathf.Abs (transform.eulerAngles.y - rot.y) > ipRotationAllowance)
					transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler(rot), 0.5f);
			}

			// Update Scale in Necessary (Usually not)
			if (update.scale.HasValue)
				transform.localScale = MathHelper.toVector3(update.scale.Value);
		}

		// Visualize Velocity Update
		private void OnDynamicTransformUpdated(DynamicTransform.Update update) {
			if (DynamicTransformReader.HasAuthority)
				return;

			// Update Velocity
			if (update.velocity.HasValue)
				Velocity = MathHelper.toVector3 (update.velocity.Value);

			// Update Rotational Velocity
			if (update.rotationalVelocity.HasValue)
				RotationalVelocity = update.rotationalVelocity.Value;
		}

		// Pitch Update Trigger
		private void OnPitchUpdated(float p) {
			Pitch = p;
		}

		// Jump Event Trigger
		private void OnJump(Nothing n) {
			if (PlayerAnimReader.HasAuthority)
				return;
			Anim.SetTrigger ("Jump");
			ShouldJump = true;
		}

		/*
		 * 
		 * Helpers
		 * 
		 */

		// Jump Land Anim Event Trigger
		private void OnLand() {
		
		}

	}

}