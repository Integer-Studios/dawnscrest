using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Polytechnica.Dawnscrest.Core;

namespace Polytechnica.Dawnscrest.Player {

	// Client Authoritative player functionality
	public class PlayerController : MonoBehaviour {

		[Require] private WorldTransform.Writer WorldTransformWriter;
		[Require] private DynamicTransform.Writer DynamicTransformWriter;
		[Require] private PlayerAnim.Writer PlayerAnimWriter;

		public float MouseSensitivity = 8.0f;
		public float MaxSpeed;
		public float MaxRotation;
		public float JumpSpeed;
		public GameObject HairMesh;
		public GameObject BodyMesh;
		public Mesh FpsMesh;
		public GameObject Hip;
		public GameObject Pivot;

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

		//Control
		private GameObject PlayerCamera;
		private float Speed;
		private float Pitch, DeltaPitch;

		/*
		 * 
		 * Start
		 * 
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
			Anim = GetComponent<Animator> ();
			RigidBody = GetComponent<Rigidbody> ();
		}

		private void Start() {
			setUpFPS ();
		}

		private void setUpFPS() {
			// Setup FPS Model
			Destroy (HairMesh);
			BodyMesh.GetComponent<SkinnedMeshRenderer> ().sharedMesh = FpsMesh;

			// Setup FPS Camera
			PlayerCamera = GameObject.FindGameObjectWithTag("MainCamera");
			PlayerCamera.transform.parent = transform;
			PlayerCamera.transform.localPosition = new Vector3 (0f, 2.3f, 0f);
			PlayerCamera.transform.localRotation = Quaternion.Euler (new Vector3 (20f, 0f, 0f));

			// Set up FPS Cursor
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			// Set Up FPS Animator
			Anim.SetLayerWeight(1, 1);
			Anim.SetLayerWeight(0, 0);

			// Set up no-collide with self
			gameObject.layer = 2;
		}

		/*
		 * 
		 * Update
		 * 
		 */

		private void Update () {
			UpdateInput ();
			UpdateLocomotionControls ();
			UpdateLocomotion ();
		}

		private void LateUpdate() {
			Hip.transform.eulerAngles = new Vector3 (Pitch, Hip.transform.eulerAngles.y, Hip.transform.eulerAngles.z);
		}

		void FixedUpdate() {
			UpdateTransform ();
		}

		// Keypress Processing
		private void UpdateInput() {

			// Update Speed
			if (Input.GetKey(KeyCode.LeftShift)) {
				Speed = MaxSpeed;
			} else {
				Speed = MaxSpeed / 2f;
			}

			// Detect Jump
			if (Input.GetKeyDown (KeyCode.Space))
				Jump ();
		}

		// Get Locomotion Input Inputs
		private void UpdateLocomotionControls() {

			// Velocity Input
			Velocity = DownsampleVelocity(new Vector3 (Input.GetAxis ("Horizontal") * Speed, 0f, Input.GetAxis ("Vertical") * Speed));

			// Turn Input
			RotationalVelocity = MouseSensitivity * Input.GetAxis ("Mouse X");

			// Horizontal Mouse Input
			PlayerCamera.transform.RotateAround(Pivot.transform.position, Pivot.transform.TransformDirection(new Vector3(-1f,0f,0f)), DeltaPitch);

			// Vertical Mouse Input
			DeltaPitch = MouseSensitivity * Input.GetAxis ("Mouse Y");
			if (Pitch - DeltaPitch < 72f && Pitch - DeltaPitch > -72f) {
				Pitch -= DeltaPitch;
			} else {
				DeltaPitch = 0f;
			}
		}

		// Locomotion Enacting
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

			// Enact Movement
			RigidBody.velocity = transform.TransformDirection(Velocity) + new Vector3(0f, RigidBody.velocity.y, 0f);

			// Enact Jump
			if (ShouldJump) {
				RigidBody.velocity += new Vector3 (0f, JumpSpeed, 0f);
				ShouldJump = false;
			}

			// Enact Turn
			transform.Rotate (Vector3.up * RotationalVelocity);

			// Set Anim Params
			Anim.SetFloat ("Vertical", Velocity.z / MaxSpeed);
			Anim.SetFloat ("Horizontal", RotationalVelocity / MaxRotation);
		}

		// Send a Transform update
		private void UpdateTransform() {

			// Send Transform Update
			WorldTransformWriter.Send(new WorldTransform.Update()
				.SetPosition(transform.position.ToCoordinates())
				.SetRotation(MathHelper.toVector3d(transform.eulerAngles))
			);

			// Send Velocity Update
			DynamicTransformWriter.Send(new DynamicTransform.Update()
				.SetVelocity(MathHelper.toVector3d(Velocity))
				.SetRotationalVelocity(RotationalVelocity)
			);

			// Send Pitch Update
			PlayerAnimWriter.Send (new PlayerAnim.Update ()
				.SetPitch(Pitch)
			);
		}

		/*
		 * 
		 * Helpers
		 * 
		 */

		// Used to normalize velocity for networking
		private Vector3 DownsampleVelocity(Vector3 v) {
			float x = Mathf.Round(v.x * 2f / MaxSpeed);
			float z = Mathf.Round(v.z * 2f / MaxSpeed);
			return new Vector3 (x * MaxSpeed / 2f, 0f, z * MaxSpeed / 2f);
		}

		private void Jump() {
			if (!Grounded)
				return;

			// Send Jump Event
			PlayerAnimWriter.Send(new PlayerAnim.Update()
				.AddJump(new Nothing())
			);

			// Set Anim Param
			Anim.SetTrigger ("Jump");
			ShouldJump = true;
		}
	}

}