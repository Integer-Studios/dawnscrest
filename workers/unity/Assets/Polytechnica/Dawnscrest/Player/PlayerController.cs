﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Polytechnica.Dawnscrest.Core;

namespace Polytechnica.Dawnscrest.Player {

	/*
	 * This class is responsible for the client-authoritative player code
	 * Anything involving a player's affect on their character starts here
	 * It is only enabled on the authoritative client
	 */
	public class PlayerController : MonoBehaviour {

		[Require] private WorldTransform.Writer worldTransformWriter;
		[Require] private DynamicTransform.Writer dynamicTransformWriter;
		[Require] private PlayerAnim.Writer playerAnimWriter;

		public float mouseSensitivity = 8.0f;
		public float maxSpeed;
		public float maxRotation;
		public float jumpSpeed;
		public GameObject hairMesh;
		public GameObject bodyMesh;
		public Mesh fpsMesh;
		public GameObject hip;
		public GameObject pivot;

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

		//Control
		private GameObject playerCamera;
		private float speed;
		private float pitch, deltaPitch;

		/*
		 * Start Functions
		 */
		private void OnEnable () {
			if (Bootstrap.isServer)
				this.enabled = false;
			else {
				Bootstrap.menuManager.OnGameStarted ();
			}
			// Fix initialization of scale to 0 by authority latency
			transform.localScale = Vector3.one;

			// Initialize References
			anim = GetComponent<Animator> ();
			rigidBody = GetComponent<Rigidbody> ();
		}

		private void Start() {
			setUpFPS ();
		}

		/*
		 * This transforms the character on the local worker only
		 * to an FPS setup
		 */
		private void setUpFPS() {
			// Setup FPS Model
			Destroy (hairMesh);
			bodyMesh.GetComponent<SkinnedMeshRenderer> ().sharedMesh = fpsMesh;

			// Setup FPS Camera
			playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
			playerCamera.transform.parent = transform;
			playerCamera.transform.localPosition = new Vector3 (0f, 2.3f, 0f);
			playerCamera.transform.localRotation = Quaternion.Euler (new Vector3 (20f, 0f, 0f));

			// Set up FPS Cursor
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			// Set Up FPS Animator
			anim.SetLayerWeight(1, 1);
			anim.SetLayerWeight(0, 0);

			// Set up no-collide with self
			gameObject.layer = 2;
		}

		/*
		 * Update Functions
		 */
		private void Update () {
			UpdateInput ();
			UpdateLocomotionControls ();
			UpdateLocomotion ();
		}

		private void LateUpdate() {
			hip.transform.eulerAngles = new Vector3 (pitch, hip.transform.eulerAngles.y, hip.transform.eulerAngles.z);
		}

		void FixedUpdate() {
			UpdateTransform ();
		}

		/*
		 * Keypress Processing
		 */
		private void UpdateInput() {

			// Update Speed
			if (Input.GetKey(KeyCode.LeftShift)) {
				speed = maxSpeed;
			} else {
				speed = maxSpeed / 2f;
			}

			// Detect Jump
			if (Input.GetKeyDown (KeyCode.Space))
				Jump ();
		}

		/*
		 * Get Locomotion Input Inputs
		 */
		private void UpdateLocomotionControls() {

			// Velocity Input
			velocity = DownsampleVelocity(new Vector3 (Input.GetAxis ("Horizontal") * speed, 0f, Input.GetAxis ("Vertical") * speed));

			// Turn Input
			rotationalVelocity = mouseSensitivity * Input.GetAxis ("Mouse X");

			// Horizontal Mouse Input
			playerCamera.transform.RotateAround(pivot.transform.position, pivot.transform.TransformDirection(new Vector3(-1f,0f,0f)), deltaPitch);

			// Vertical Mouse Input
			deltaPitch = mouseSensitivity * Input.GetAxis ("Mouse Y");
			if (pitch - deltaPitch < 72f && pitch - deltaPitch > -72f) {
				pitch -= deltaPitch;
			} else {
				deltaPitch = 0f;
			}
		}

		/*
		 * Locomotion Enacting
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

			// Enact Movement
			rigidBody.velocity = transform.TransformDirection(velocity) + new Vector3(0f, rigidBody.velocity.y, 0f);

			// Enact Jump
			if (shouldJump) {
				rigidBody.velocity += new Vector3 (0f, jumpSpeed, 0f);
				shouldJump = false;
			}

			// Enact Turn
			transform.Rotate (Vector3.up * rotationalVelocity);

			// Set Anim Params
			anim.SetFloat ("Vertical", velocity.z / maxSpeed);
			anim.SetFloat ("Horizontal", rotationalVelocity / maxRotation);
		}

		/*
		 * Send a Transform update
		 */
		private void UpdateTransform() {

			// Send Transform Update
			worldTransformWriter.Send(new WorldTransform.Update()
				.SetPosition(transform.position.ToCoordinates())
				.SetRotation(MathHelper.toVector3d(transform.eulerAngles))
			);

			// Send Velocity Update
			dynamicTransformWriter.Send(new DynamicTransform.Update()
				.SetVelocity(MathHelper.toVector3d(velocity))
				.SetRotationalVelocity(rotationalVelocity)
			);

			// Send Pitch Update
			playerAnimWriter.Send (new PlayerAnim.Update ()
				.SetPitch(pitch)
			);
		}

		/*
		 * Helper Functions
		 */

		/*
		 * Used to normalize velocity for networking
		 */
		private Vector3 DownsampleVelocity(Vector3 v) {
			float x = Mathf.Round(v.x * 2f / maxSpeed);
			float z = Mathf.Round(v.z * 2f / maxSpeed);
			return new Vector3 (x * maxSpeed / 2f, 0f, z * maxSpeed / 2f);
		}

		/*
		 * Triggered by space bar
		 */
		private void Jump() {
			if (!grounded)
				return;

			// Send Jump Event
			playerAnimWriter.Send(new PlayerAnim.Update()
				.AddJump(new Nothing())
			);

			// Set Anim Param
			anim.SetTrigger ("Jump");
			shouldJump = true;
		}
	}

}