using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Polytechnica.Dawnscrest.Core;

namespace Polytechnica.Dawnscrest.Player {

	/* Client Non-Authoritative player functionality
	 * This is a read only class that basically visualized the palyer
	 * parameters in the world
	 * it is enabled on non-auth clients as well as servers
	 * and is used to visualize NPCs as well as players
	 */
	public class CharacterVisualizer : MonoBehaviour {

		[Require] private WorldTransform.Reader worldTransformReader;
		[Require] private DynamicTransform.Reader dynamicTransformReader;
		[Require] private PlayerAnim.Reader playerAnimReader;
		[Require] private CharacterAppearance.Reader appearanceReader;

		public float ipPositionAllowance;
		public float ipRotationAllowance;

		// Velocity
		private Vector3 velocity;
		private float rotationalVelocity;

		// Animation
		private bool rightHand;
		private bool isConsuming;
		private bool isInteracting;
		private Animator anim;
		private Rigidbody rigidBody;
		private bool shouldJump;
		private bool grounded = false;
		private GameObject groundObject;
		private float pitch;

		//References
		private PlayerController controller;
		private AppearanceVisualizer appearanceVisualizer;

		/*
		 * 
		 * Start Functions
		 * 
		 */
		void OnEnable () {

			// Initialize References
			controller = GetComponent<PlayerController> ();
			anim = GetComponent<Animator> ();
			rigidBody = GetComponent<Rigidbody> ();
			rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
			appearanceVisualizer = GetComponent<AppearanceVisualizer> ();

			// Register Listeners
			worldTransformReader.ComponentUpdated += OnWorldTransformUpdated;
			dynamicTransformReader.ComponentUpdated += OnDynamicTransformUpdated;
			appearanceReader.ComponentUpdated += OnAppearanceUpdated;
			playerAnimReader.JumpTriggered += OnJump;
			playerAnimReader.PitchUpdated += OnPitchUpdated;

			// Initialize transform to Spatial
			transform.position = worldTransformReader.Data.position.ToVector3();
			transform.eulerAngles = MathHelper.toVector3(worldTransformReader.Data.rotation);
			transform.localScale = MathHelper.toVector3 (worldTransformReader.Data.scale);

			// Initialize Appearance
			appearanceVisualizer.InitializeFromData(appearanceReader.Data);
		}

		void OnDisable () {
			// UnRegister Listeners
			worldTransformReader.ComponentUpdated -= OnWorldTransformUpdated;
			dynamicTransformReader.ComponentUpdated -= OnDynamicTransformUpdated;
			playerAnimReader.JumpTriggered -= OnJump;
			playerAnimReader.PitchUpdated -= OnPitchUpdated;
		}

		/*
		 * 
		 * Update Function
		 * 
		 */
		private void Update() {
			if (worldTransformReader.HasAuthority)
				return;
			UpdateLocomotion ();
		}

		/*
		 * Physics part of locomotion update See PlayerController LateUpdate for details
		 */
		private void LateUpdate() {
			if (playerAnimReader.HasAuthority)
				return;

			// Update Pitch Visualization
			controller.hip.transform.eulerAngles = new Vector3 (pitch, controller.transform.eulerAngles.y, controller.hip.transform.eulerAngles.z);
			// Enact Movement
			rigidBody.velocity = transform.TransformDirection(velocity) + new Vector3(0f, rigidBody.velocity.y, 0f);
			// Entact Rotaton
			rigidBody.MoveRotation(Quaternion.Euler(rigidBody.rotation.eulerAngles + Vector3.up * rotationalVelocity));
		}

		/*
		 * Visualize Locomotion - Except Physics Velocities
		 */
		private void UpdateLocomotion() {

			// Update Grounded
			RaycastHit hit;
			if (Physics.Raycast (transform.position+transform.up, -transform.up, out hit, 2f)) {
				groundObject = hit.collider.gameObject;
				anim.SetBool ("Grounded", true);
				grounded = true;
			} else {
				anim.SetBool ("Grounded", false);
				grounded = false;
			}

			// Enact Jump
			if (shouldJump) {
				rigidBody.velocity += new Vector3 (0f, controller.jumpSpeed, 0f);
				shouldJump = false;
			}

			// Set Anim Params
			anim.SetFloat ("Vertical", velocity.z / controller.maxSpeed);
			anim.SetFloat ("Horizontal", rotationalVelocity / controller.maxRotation);
		}


		/*
		 * Networking
		 */

		/*
		 * Visualize Transform Update
		 */
		private void OnWorldTransformUpdated(WorldTransform.Update update) {
			if (worldTransformReader.HasAuthority)
				return;

			// Update Position within ipAllowance
			if (update.position.HasValue) {
				Vector3 pos = update.position.Value.ToVector3 ();
				if (Vector3.Distance (pos, transform.position) > ipPositionAllowance && grounded)
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

		/* 
		 * Visualize Velocity Update
		 */
		private void OnDynamicTransformUpdated(DynamicTransform.Update update) {
			if (dynamicTransformReader.HasAuthority)
				return;

			// Update Velocity
			if (update.velocity.HasValue)
				velocity = MathHelper.toVector3 (update.velocity.Value);

			// Update Rotational Velocity
			if (update.rotationalVelocity.HasValue)
				rotationalVelocity = update.rotationalVelocity.Value;
		}

		/* 
		 * Pitch Update Trigger
		 */
		private void OnPitchUpdated(float p) {
			pitch = p;
		}

		private void OnAppearanceUpdated(CharacterAppearance.Update update) {
			appearanceVisualizer.setAppearanceFromUpdate (update);
		}

		/*
		 * Jump Event Trigger
		 */
		private void OnJump(Nothing n) {
			if (playerAnimReader.HasAuthority)
				return;
			anim.SetTrigger ("Jump");
			shouldJump = true;
		}

		/*
		 * Helpers
		 */

		/* 
		 * Jump Land Anim Event Trigger
	     */
		private void OnLand() {
		
		}

	}

}