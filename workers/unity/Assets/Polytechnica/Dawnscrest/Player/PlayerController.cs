using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Core;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Polytechnica.Dawnscrest.Core;
using Polytechnica.Dawnscrest.GUI;

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
		[Require] private CharacterVitals.Reader characterVitalsReader;
		[Require] private CharacterAppearance.Reader appearanceReader;

		public float mouseSensitivity = 8.0f;
		public float maxSpeed;
		public float maxRotation;
		public float jumpSpeed;
		public Material cullingMaterial; 
		public GameObject[] cullingMeshes;
		public GameObject bodyMesh;
		public GameObject hip;
		public GameObject pivot;

		// Velocity
		private Vector3 velocity;
		private float rotationalVelocity;

		// Animation
		private bool rightHand;
		private bool isConsuming;
		private bool isInteracting;
		private bool shouldJump;
		private bool grounded = false;
		private GameObject groundObject;

		// Control
		private GameObject playerCamera;
		private float speed;
		private float pitch, deltaPitch;

		// References
		private AppearanceVisualizer appearanceVisualizer;
		private Animator anim;
		private Rigidbody rigidBody;

		/*
		 * Start Functions
		 */
		private void OnEnable () {
			// Disable if this is on the server
			if (Bootstrap.isServer) {
				this.enabled = false;
				return;
			} else {
				Bootstrap.OnPlayerSpawn ();
			}

			// Setup Vitals Reader
			characterVitalsReader.ComponentUpdated += OnVitalsUpdated;
			appearanceReader.ComponentUpdated += OnAppearanceUpdated;

			// Initialize References
			anim = GetComponent<Animator> ();
			rigidBody = GetComponent<Rigidbody> ();
			appearanceVisualizer = GetComponent<AppearanceVisualizer> ();
		}

		private void OnDisable() {
			characterVitalsReader.ComponentUpdated -= OnVitalsUpdated;
			appearanceReader.ComponentUpdated -= OnAppearanceUpdated;
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
			Material[] mats;
			foreach (GameObject g in cullingMeshes) {
				mats = g.GetComponent<SkinnedMeshRenderer> ().materials;
				mats [0] = cullingMaterial;
				g.GetComponent<SkinnedMeshRenderer> ().materials = mats;
			}
		    mats = bodyMesh.GetComponent<SkinnedMeshRenderer> ().materials;
			mats [0] = cullingMaterial;
			bodyMesh.GetComponent<SkinnedMeshRenderer> ().materials = mats;
			bodyMesh.GetComponent<SkinnedMeshRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

			// Setup FPS Camera
			playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
			playerCamera.transform.parent = transform;
			playerCamera.transform.localPosition = new Vector3 (0f, 3.3f, 0f);
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

		/*
		 * Update Function Which will overwrite physics calc's
		 * This is important because the character movement is done with rigidbody
		 * velocities so the controls have to overwrite the physics calculations *selectively*
		 */
		private void LateUpdate() {
			hip.transform.eulerAngles = new Vector3 (pitch, hip.transform.eulerAngles.y, hip.transform.eulerAngles.z);
			// Enact Movement
			rigidBody.velocity = transform.TransformDirection(velocity) + new Vector3(0f, rigidBody.velocity.y, 0f);
			// Entact Rotaton
			rigidBody.MoveRotation(Quaternion.Euler(rigidBody.rotation.eulerAngles + Vector3.up * rotationalVelocity));
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

			// Temporary Test Keys for Crosshair
			if (Input.GetKey (KeyCode.O)) {
				GUIManager.crosshair.SetInteractable ("Pickup\nRock");
			}
			if (Input.GetKey (KeyCode.P)) {
				GUIManager.crosshair.SetMinimized ();
			}
			if (Input.GetKey (KeyCode.L)) {
				GUIManager.crosshair.SetTooltip ("Pick\nRequired");
			}

			if (Input.GetKey (KeyCode.Escape)) {
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
//				Bootstrap.menuManager.LoadMenu (Polytechnica.Dawnscrest.Menu.MenuManager.MenuType.PAUSE);
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
			velocity = new Vector3 (Input.GetAxis ("Horizontal") * speed, 0f, Input.GetAxis ("Vertical") * speed);

			// Horizontal Mouse Input
			rotationalVelocity = mouseSensitivity * Input.GetAxis ("Mouse X");

			// Vertical Mouse Input
			deltaPitch = mouseSensitivity * Input.GetAxis ("Mouse Y");
			if (pitch - deltaPitch < 72f && pitch - deltaPitch > -72f) {
				pitch -= deltaPitch;
			} else {
				deltaPitch = 0f;
			}
		}

		/*
		 * Locomotion Enacting - Except Physics Velocities (See LateUpdate)
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
				rigidBody.velocity += new Vector3 (0f, jumpSpeed, 0f);
				shouldJump = false;
			}

			// Enact Pitch
			playerCamera.transform.RotateAround(pivot.transform.position, pivot.transform.TransformDirection(new Vector3(-1f,0f,0f)), deltaPitch);

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
				.SetVelocity(MathHelper.toVector3d(DownsampleVelocity(velocity)))
				.SetRotationalVelocity(rotationalVelocity)
			);

			// Send Pitch Update
			playerAnimWriter.Send (new PlayerAnim.Update ()
				.SetPitch(pitch)
			);
		}

		/*
		 * Triggered by component update for vitals, sets HUD sliders
		 */
		private void OnVitalsUpdated(CharacterVitals.Update update) {
			GUIManager.hud.SetThirst (update.thirst.Value, update.thirstMax.Value);
			GUIManager.hud.SetHunger (update.hunger.Value, update.hungerMax.Value);
			GUIManager.hud.SetHealth (update.health.Value, update.healthMax.Value);
		}

		/*
		 * Triggered by component update for appearance
		 */
		private void OnAppearanceUpdated(CharacterAppearance.Update update) {
			GUIManager.hud.SetPortraitAppearance (AppearanceSet.GetSetFromUpdate (update));
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