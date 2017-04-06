using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using PolyItem;
using PolyEntity;
using PolyWorld;
using PolyEffects;
using PolyNetwork;

namespace PolyPlayer {

	public class Player : NetworkBehaviour, Interactor, InventoryListener, ItemHolder, Living, IPolyPlayer {

		// Vars : public, protected, private, hide
		#region
		public float interpolationRate = 9f;
		public float ipPositionAllowance = 0.1f;
		public float ipPositionServerAuth = 1f;
		public float ipRotationAllowance = 1f;
		public float vitalsUpdateRate = 5f;
		public float mouseSensitivity = 8.0f;
		public GameObject hairMesh;
		public Mesh[] hairMeshes;
		public GameObject bodyMesh;
		public Mesh fpsMesh;
		public GameObject pivot;
		public float armLength;
		public float hitRange;
		public float maxSpeed;
		public float maxRotation;
		public float jumpSpeed;
		public float hungerMultiple;
		public float thirstMultiple;
		public float healthMultiple;
		public GameObject hip;
		public Recipe[] defaultRecipes;
		public Transform rightHandTransform;
		public Transform leftHandTransform;
		public Transform backTransform;
		public GameObject deadPrefab;
		public bool opMode;

		private GameObject playerCamera;
		private float pitch, deltaPitch;
		private float speed;
		private Crosshair crosshair;
		private Rigidbody rigidBody;
		private Animator anim;
		private bool grounded;
		private List<Recipe> recipes;
		private Inventory mainInventory;
		private HotBarInventory hotbarInventory;
		private Inventory openInventory;
		private GameObject lookingAtObject;
		private Vector3 lookingAtPoint;
		private Vector3 lookingAtNormal;
		private GameObject groundObject;
		private bool shouldJump;
		private float timeConsumeStart = -1f;
		private bool heated;
		private Vector3 velocity;
		private float rotationalVelocity;
		private bool rightHandActive = true;
		private SoundManager sounds;
		private EffectListener effects;

		// Syncvars
		[SyncVar]
		public float maxHealth;
		[SyncVar]
		public float maxHunger;
		[SyncVar]
		public float maxThirst;
		[SyncVar]
		private float health;
		[SyncVar]
		private float hunger;
		[SyncVar]
		private float thirst;
		[SyncVar]
		public int playerID;

		#endregion
		/*
		 * 
		 * Public Interface
		 * 
		 */
		#region
		// Living Interface

		[Server]
		public void living_hurt(Living l, float d) {
			health -= d * (maxHunger / hunger);
			if (health <= 0f)
				die ();
			RpcHurt ();
		}

		[Server]
		public void living_setHeated(bool h) {
			heated = h;
		}

		// Interactor Interface

		public ItemStack interactor_getItemInHand() {
			if (rightHandActive)
				return hotbarInventory.getSlotCopy(2);
			else
				return hotbarInventory.getSlotCopy(0);
		}

		[Server]
		public void interactor_giveItem(Item i) {
			if (!hotbarInventory.insert (i)) {
				if (!mainInventory.insert (i)) {
					return;
				}
			}
			sounds.rpcPlaySound (PlayerSound.ItemPickup);
			NetworkServer.Destroy (i.gameObject);
		}

		[Server]
		public Vector3 interactor_getInteractionPosition() {
			return lookingAtPoint;
		}

		[Server]
		public Vector3 interactor_getInteractionNormal() {
			return lookingAtNormal;
		}

		// Inventory Listener Interface
		[Server]
		public void inventoryListener_onSlotChange(Inventory inv, int i, ItemStack s) {
			if (isServer) {
				if (inv == hotbarInventory)
					updateHolding (s, i);
			}
		}

		[Server]
		public void polyPlayer_sendPlayerData(int playerID) {
			RpcPlayerData (playerID);
		}

		// Item Holder Interface

		public GameObject itemHolder_getGameObject() {
			return gameObject;
		}

		public Transform itemHolder_getHolderTransform (int hid) {
			switch (hid) {
			case 0:
				return leftHandTransform;
			case 1:
				return backTransform;
			case 2:
				return rightHandTransform;
			default:
				return rightHandTransform;
			}
		}

		public Vector3 itemHolder_getOffset(int hid) {
			if (hid == 1)
				return new Vector3 (0.2f, 0.2f, -0.2f);
			return Vector3.zero;
		}

		public Vector3 itemHolder_getRotation(int hid) {
			if (hid == 1)
				return new Vector3 (0f, 0f, -45f);
			return Vector3.zero;
		}

		public bool itemHolder_isLocalPlayer() {
			return isLocalPlayer;
		}
			
		// GUI Interface

		[Client]
		public void onSlotUpdate(int bindingID, int slotID, ItemStack s) {
			CmdSetSlot (bindingID, slotID, new NetworkItemStack (s));
		}

		[Client]
		public void setCraftableRecipe(Craftable c, Recipe r) {
			if (r == null)
				CmdSetCraftableRecipe (c.gameObject, new NetworkItemStack (null), new NetworkItemStackArray (null));
			else
				CmdSetCraftableRecipe (c.gameObject, new NetworkItemStack (r.output), new NetworkItemStackArray (r.input));
		}

		[Client]
		public void setCraftableInput(Craftable c, ItemStack[] i) {
			CmdSetCraftableInput (c.gameObject, new NetworkItemStackArray (i));
		}

		[Client]
		public void bindInventoryToGUI(int invID, GUIBoundInventory g) {
			switch (invID) {
			case 0:
				g.bind (hotbarInventory, 0);
				break;
			case 1:
				g.bind (mainInventory, 1);
				break;
			case 2:
				g.bind (openInventory, 2);
				break;
			}
		}

		[Client]
		public void syncVital(int vitalID, Slider s) {
			switch (vitalID) {
			case 0:
				s.maxValue = maxHealth;
				s.value = health;
				break;
			case 1:
				s.maxValue = maxHunger;
				s.value = hunger;
				break;
			case 2:
				s.maxValue = maxThirst;
				s.value = thirst;
				break;
			}
		}

		[Client]
		public void polyPlayer_receiveChat(string name, string message, float distance) {
			if (name != null && name.Length != 0)
				GUIManager.chat.displayMessage (name + ": " + message, distance);
			else
				GUIManager.chat.displayMessage (message, distance);
			
		}

		// Multi-Use

		public void loadVitals(float health, float hunger, float thirst) {
			this.health = health;
			this.hunger = hunger;
			this.thirst = thirst;
		}

		public float getHealth() {
			return health;
		}

		public float getHunger() {
			return hunger;
		}

		public float getThirst() {
			return thirst;
		}

		public List<Recipe> getRecipes(CraftingType t) {
			List<Recipe> r = new List<Recipe> ();
			foreach (Recipe rec in recipes) {
				if (rec.type == t)
					r.Add (rec);
			}
			return r;
		}
			
		public bool isMoving() {
			return velocity != Vector3.zero;
		}

		public void sendChat(string message) {
			if (message.Length > 0)
				CmdSendChat (message);
		}

		#endregion
		/*
		 * 
		 * Server->Client Networked Interface
		 * 
		 */
		#region
		// Server Auth

		[ClientRpc]
		private void RpcHurt() {
			anim.SetTrigger ("Hurt");
			sounds.playSound(PlayerSound.Hurt);
		}

		// Broadcasts

		[ClientRpc]
		private void RpcInteracting_Broadcast(bool i, bool rightHand) {
			if (!isLocalPlayer) {
				setRightHand (rightHand);
				anim.SetBool ("Interacting", i);
			}
		}

		[ClientRpc]
		private void RpcConsuming_Broadcast(bool e, bool rightHand) {
			if (!isLocalPlayer) {
				setRightHand (rightHand);
				anim.SetBool ("Consuming", e);
			}
		}

		[ClientRpc]
		private void RpcSwing_Broadcast(bool rightHand) {
			if (!isLocalPlayer) {
				setRightHand (rightHand);
				anim.SetTrigger ("Swing");
			}
		}

		[ClientRpc]
		private void RpcJump_Broadcast() {
			if (!isLocalPlayer) {
				anim.SetTrigger ("Jump");
				shouldJump = true;
			}
		}

		[ClientRpc]
		private void RpcVelocity_Broadcast(Vector3 v) {
			if (!isLocalPlayer)
				velocity = v;
		}

		[ClientRpc]
		private void RpcRotation_Broadcast(float f) {
			if (!isLocalPlayer)
				rotationalVelocity = f;
		}

		[ClientRpc]
		private void RpcPitch_Broadcast(float f) {
			if (!isLocalPlayer)
				pitch = f;
		}

		[ClientRpc]
		private void RpcUpdateTransform_Broadcast(Vector3 v, float rv, Vector3 p, Quaternion r, float pi) {
			if (isLocalPlayer)
				return;
			velocity = v;
			rotationalVelocity = rv;
			pitch = Mathf.Lerp(pitch, pi, 0.5f);
			if (Vector3.Distance(transform.position, p) > ipPositionAllowance)
				transform.position = Vector3.Lerp(transform.position, p, 0.5f);
			if (Mathf.Abs (transform.rotation.y - r.y) > ipRotationAllowance)
				transform.rotation = Quaternion.Lerp (transform.rotation, r, 0.5f);
			transform.rotation = r;
		}

		[ClientRpc]
		private void RpcTransformDenied(Vector3 pos) {
			if (isLocalPlayer)
				transform.position = pos;
		}

		[ClientRpc]
		private void RpcPlayerData(int playerID) {
			this.playerID = playerID;
			setUpHair ();
		}

		#endregion
		/*
		* 
		* Client->Server Networked Interface
		* 
		*/
		#region
		// Animation Commands

		[Command]
		private void CmdInteracting(bool i, bool rightHand) {
			if (!isLocalPlayer && !isClient) {
				setRightHand (rightHand);
				anim.SetBool ("Interacting", i);
			}
			RpcInteracting_Broadcast (i, rightHand);
		}

		[Command]
		private void CmdConsuming(bool e, bool rightHand) {
			if (!isLocalPlayer && !isClient) {
				setRightHand (rightHand);
				anim.SetBool ("Consuming", e);
			}
			RpcConsuming_Broadcast (e, rightHand);
		}

		[Command]
		private void CmdSwing(bool rightHand) {
			if (!isLocalPlayer && !isClient) {
				setRightHand (rightHand);
				anim.SetTrigger ("Swing");
			}
			RpcSwing_Broadcast (rightHand);
		}

		[Command]
		private void CmdJump() {
			if (!isLocalPlayer && !isClient) {
				anim.SetTrigger ("Jump");
				shouldJump = true;
			}
			RpcJump_Broadcast ();
		}

		// Locomotion Commands

		[Command]
		private void CmdUpdateTransform (Vector3 v, float rv, Vector3 p, Quaternion r, float pi) {
			if (Vector3.Distance (transform.position, p) > ipPositionServerAuth) {
				p = transform.position;
				RpcTransformDenied (p);
			}

			if (!isClient) {
				velocity = v;
				rotationalVelocity = rv;
				transform.position = p;
				transform.rotation = r;
				pitch = pi;
			}
			RpcUpdateTransform_Broadcast (v, rv, p, r, pi);
		}

		// Interaction Commands

		[Command]
		private void CmdInteract(GameObject g, Vector3 point, Vector3 norms) {
			if (g == null)
				return;
			
			lookingAtPoint = point;
			lookingAtNormal = norms;
			g.GetComponent<Interactable> ().interact (this, Time.deltaTime);
		}

		// Inventory Commands

		[Command]
		private void CmdSetSlot(int inventoryID, int slotID, NetworkItemStack stack) {
			ItemStack s = ItemStack.unwrapNetworkStack(stack);
			switch (inventoryID) {
			case 0:
				hotbarInventory.setSlot (slotID, s);
				break;
			case 1:
				mainInventory.setSlot (slotID, s);
				break;
			case 2:
				openInventory.setSlot (slotID, s);
				break;
			default:
				break;
			}
		}

		[Command]
		private void CmdOpenInventory(GameObject g) {
			openInventory = g.GetComponent<Inventory> ();
		}

		[Command]
		private void CmdSetCraftableRecipe(GameObject g, NetworkItemStack o, NetworkItemStackArray i) {
			Recipe r = Recipe.unwrapRecipe(o,i);
			g.GetComponent<Craftable> ().setRecipe (r);
		}

		[Command]
		private void CmdSetCraftableInput(GameObject g, NetworkItemStackArray i) {
			g.GetComponent<Craftable> ().setInput (ItemStack.unwrapNetworkStackArray (i));
		}

		[Command]
		private void CmdHotbarSwitch(bool rightHand) {
			hotbarInventory.switchBack (rightHand);
		}

		// Combat Commands

		[Command]
		private void CmdOnHit(GameObject g) {
			
			Living l = g.GetComponent<Living> ();
			if (l != null) {
				
				l.living_hurt (this, 5f);
			}
		}

		// Eating Commands

		[Command]
		private void CmdCompleteConsuming() {
			int slot = 0;
			if (rightHandActive)
				slot = 2;
			ItemStack s = hotbarInventory.getSlotCopy (slot);
			if (ItemManager.getConsumableType (s) == ConsumableType.Food) {
				hunger += ItemManager.getConsumableNutrition (s);
			}
			if (ItemManager.getConsumableType (s) == ConsumableType.Water) {
				thirst += ItemManager.getConsumableNutrition (s);
			}
			hotbarInventory.decreaseSlot(slot);
		}

		// Item Placing Commands

		[Command]
		private void CmdPlaceItem(Vector3 p, bool rightHand) {
			
			int slot = 0;
			if (rightHand)
				slot = 2;
			
			GameObject g = ItemManager.createItemForPlacing (hotbarInventory.getSlotCopy (slot));
			g.GetComponent<Item> ().setPosition (p);
			hotbarInventory.decreaseSlot (slot);
		}

		// Chat Commands

		[Command]
		private void CmdSendChat(string message) {
			PolyChatManager.handleSend (playerID, message);
		}

		// Networking / Saving Commands

		[Command]
		private void CmdOverwriteSave() {

			PolyDataManager.overwriteSave (playerID);
		}

		[Command]
		private void CmdOnPlayerLoaded(int id) {
			if (id > 0) 
				StartCoroutine(PolyDataManager.ReadPlayerData (id));
		}


		#endregion
		/* 
		 * 
		 * Private
		 * 
		 */
		#region
		// Start

		private void Start() {
			health = maxHealth;
			hunger = maxHunger;
			thirst = maxThirst;

			ClientScene.RegisterPrefab (deadPrefab);
			sounds = GetComponent <SoundManager> ();
			effects = GetComponent<EffectListener> ();

			if (isServer)
				StartCoroutine (updateVitals ());

			rigidBody = GetComponent<Rigidbody> ();
			anim = GetComponent<Animator> ();
			recipes = new List<Recipe> ();
			foreach (Recipe r in defaultRecipes) {
				recipes.Add (r);
			}

			foreach (Inventory i in GetComponents<Inventory> ()) {
				if (i is HotBarInventory)
					hotbarInventory = (HotBarInventory)i;
				else
					mainInventory = i;

			}
			StartCoroutine(lateStart ());

			if (!isLocalPlayer)
				return;
			PolyNetworkManager.setLocalPlayer (this);
			CmdOnPlayerLoaded (playerID);

			rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
			setUpCamera ();
			setUpLocalAnimations ();
			crosshair = FindObjectOfType<Crosshair> ();
			GUIManager.closeGUI ();
			StartCoroutine(networkTransformUpdate ());
			StartCoroutine(footstepSoundPlay ());
		}

		private void setUpHair() {
			Debug.Log (playerID);
			hairMesh.GetComponent<SkinnedMeshRenderer> ().sharedMesh = hairMeshes[playerID-1];
//			hairMesh.GetComponent<SkinnedMeshRenderer> ().material = 
		}

		private void setUpCamera() {
			Destroy (hairMesh);
			bodyMesh.GetComponent<SkinnedMeshRenderer> ().sharedMesh = fpsMesh;
			playerCamera = GameObject.FindGameObjectWithTag("MainCamera");

			playerCamera.transform.parent = transform;
			playerCamera.transform.localPosition = new Vector3 (0f, 2.3f, 0f);
			playerCamera.transform.localRotation = Quaternion.Euler (new Vector3 (20f, 0f, 0f));

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		private void setUpLocalAnimations() {
			anim.SetLayerWeight(1, 1);
			anim.SetLayerWeight(0, 0);
			gameObject.layer = 2;
		}

		private IEnumerator lateStart() {
			yield return new WaitForSeconds (1f);

			if (isLocalPlayer)
				GUIManager.setPlayer (this);
			
			if (isServer)
				hotbarInventory.startListening (this, true);

			// TODO delete this when we have a network manager doing on login notifications
			//
			if (isServer) {
				Item[] items = FindObjectsOfType<Item> ();
				foreach (Item i in items) {
					i.OnPlayerConnected ();
				}
			}
			//
			//
		}

		// Update

		private void Update() {

			updateLocomotion ();

			if (!isLocalPlayer)
				return;

			updateLookingAt ();
			if (!GUIManager.processInput()) {
				updateCrosshair ();
				updateLocomotionControls ();
				updateInput ();
				updateMouse ();
			}
		}

		private void LateUpdate() {
			hip.transform.eulerAngles = new Vector3 (pitch, hip.transform.eulerAngles.y, hip.transform.eulerAngles.z);
		}

		private void updateCrosshair () {
			if (isLookingAtInteractable ()) {
				crosshair.expand ();
				crosshair.setFill (lookingAtObject.GetComponent<Interactable> ().getPercent ());
			} else {
				crosshair.minimize ();
			}
		}

		private void updateLocomotion() {
			RaycastHit hit;
			if (Physics.Raycast (transform.position+transform.up, -transform.up, out hit, 2f)) {
				groundObject = hit.collider.gameObject;
				anim.SetBool ("Grounded", true);
				grounded = true;
			} else {
				anim.SetBool ("Grounded", false);
				grounded = false;
			}

			rigidBody.velocity = transform.TransformDirection(velocity) + new Vector3(0f, rigidBody.velocity.y, 0f);
			if (shouldJump) {
				rigidBody.velocity += new Vector3 (0f, jumpSpeed, 0f);
				shouldJump = false;
			}
			transform.Rotate (Vector3.up * rotationalVelocity);
			anim.SetFloat ("Vertical", velocity.z / maxSpeed);
			anim.SetFloat ("Horizontal", rotationalVelocity / maxRotation);
		}

		private void updateLocomotionControls() {
			velocity = new Vector3 (Input.GetAxis ("Horizontal") * speed, 0f, Input.GetAxis ("Vertical") * speed);
			rotationalVelocity = mouseSensitivity * Input.GetAxis ("Mouse X");
			playerCamera.transform.RotateAround(pivot.transform.position, pivot.transform.TransformDirection(new Vector3(-1f,0f,0f)), deltaPitch);
		}

		private void updateMouse() {
			if(Input.GetMouseButtonDown (0))
				onMouseDown (false);

			if(Input.GetMouseButtonUp (0))
				cancelActions ();

			if(Input.GetMouseButton (0))
				mouseDown ();

			if(Input.GetMouseButtonDown (1))
				onMouseDown (true);

			if(Input.GetMouseButtonUp (1))
				cancelActions ();

			if(Input.GetMouseButton (1))
				mouseDown ();

		}

		private void updateLookingAt() {
			RaycastHit hit; 
			Ray ray = Camera.main.ScreenPointToRay (crosshair.transform.position); 

			if (Physics.Raycast (ray, out hit, armLength)) {
				lookingAtObject = hit.collider.gameObject;
				lookingAtPoint = hit.point;
				lookingAtNormal = hit.normal;
			} else {
				lookingAtObject = null;
				lookingAtPoint = transform.position+ transform.up  + transform.forward;
				ray = new Ray (transform.position + transform.up  + transform.forward, -transform.up);
				if (Physics.Raycast (ray, out hit)) {
					lookingAtPoint = hit.point;
					lookingAtNormal = Vector3.zero;
				}
				lookingAtNormal = hit.normal;
			}
		}

		private void updateInput() {
			if (Input.GetKey(KeyCode.LeftShift)) {
				speed = maxSpeed;
			} else {
				speed = maxSpeed / 2f;
			}

			if (Input.GetKeyDown (KeyCode.Space) && !(opMode && Input.GetKey(KeyCode.LeftAlt)))
				jump ();

			if (Input.GetKeyDown (KeyCode.Escape)) {
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			if (Input.GetKeyDown (KeyCode.Z)) {
				GUIManager.pushScreen (GUIManager.characterScreen);
			}
			if (Input.GetKeyDown (KeyCode.C)) {
				GUIManager.recipesScreen.openWithRecipes(getRecipes(CraftingType.Hand));
			}
			if (Input.GetKeyDown(KeyCode.Q)) {
				CmdHotbarSwitch (false);
			}
			if (Input.GetKeyDown(KeyCode.E)) {
				CmdHotbarSwitch (true);
			}

			if (Input.GetKeyDown(KeyCode.Alpha1)) {
				attemptPlaceItem (false);
			}
			if (Input.GetKeyDown(KeyCode.Alpha3)) {
				attemptPlaceItem (true);
			}

			if (Input.GetKeyDown(KeyCode.T)) {
				GUIManager.setChatOpen (true);
			}

			deltaPitch = mouseSensitivity * Input.GetAxis ("Mouse Y");
			if (pitch - deltaPitch < 72f && pitch - deltaPitch > -72f) {
				pitch -= deltaPitch;
			} else {
				deltaPitch = 0f;
			}

			if (opMode && Input.GetKey(KeyCode.LeftAlt))
				processOPInput ();
		}

		private void processOPInput () {
			if (Input.GetKey (KeyCode.LeftShift)) {
				maxSpeed += Time.deltaTime * 10f;
			} else {
				maxSpeed = 8f;
			}

			if (Input.GetKey (KeyCode.Space)) {
				jumpSpeed += Time.deltaTime * 10f;
			} else{
				jumpSpeed = 7f;
			}

			if (Input.GetKeyDown(KeyCode.RightBracket)) {
				CmdOverwriteSave ();
			}
		}

		private IEnumerator networkTransformUpdate() {
			while (true) {
				CmdUpdateTransform (downsampleVelocity(velocity), rotationalVelocity, transform.position, transform.rotation, pitch);
				yield return new WaitForSeconds (1/interpolationRate);
			}
		}

		private IEnumerator updateVitals() {
			while (true) {
				
				float temp = WorldTerrain.getTerrainTempurature (transform.position);
				if (!heated)
					hunger -= (hungerMultiple / temp) * vitalsUpdateRate;

				if (heated) 
					health += healthMultiple * vitalsUpdateRate;
				else if (temp < 0.3)
					health -= healthMultiple * vitalsUpdateRate;
				
				if (hunger < 0.3 * maxHunger) 
					health -= healthMultiple * vitalsUpdateRate;
				 else if (hunger > 0.8 * maxHunger) 
					health += healthMultiple * vitalsUpdateRate;
				
				if (thirst < 0.3 * maxThirst)
					health -= healthMultiple * vitalsUpdateRate;
				
				thirst -= thirstMultiple * vitalsUpdateRate;
				health = Mathf.Max (0f, health);
				health = Mathf.Min (100f, health);
				hunger = Mathf.Max (0f, hunger);
				hunger = Mathf.Min (100f, hunger);
				thirst = Mathf.Max (0f, thirst);
				thirst = Mathf.Min (100f, thirst);

				if (health <= 0f)
					die ();
				
				heated = false;

				yield return new WaitForSeconds (vitalsUpdateRate);
			}
		}

		// Mouse

		private void onMouseDown(bool rightHand) {

			if (Cursor.visible) {
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				return;
			}

			setRightHand (rightHand);

			if (Input.GetKey (KeyCode.LeftControl))
				secondaryAction ();
			else 
				primaryAction ();
		}

		private void mouseDown() {
			if (Input.GetKey (KeyCode.LeftControl))
				secondaryActionUpdate ();
			else 
				primaryActionUpdate ();
		}

		private void primaryAction() {
			if (!isLookingAtInteractable ()) {
				swing ();
			} else {
				setInteracting (true);
			}
		}

		private void primaryActionUpdate() {
			if (isLookingAtInteractable ()) {
				CmdInteract (lookingAtObject, lookingAtPoint, lookingAtNormal);
			}
		}

		private void secondaryAction() {
			if (attemptCraft ())
				return;

			if (attemptOpenInventory ())
				return;

			if (attemptConsume ())
				return;
		}

		private void secondaryActionUpdate() {
			if (isConsuming ())
				updateConsuming ();
		}

		private void cancelActions() {
			setInteracting (false);
			if (isConsuming ())
				stopConsuming ();
		}
			
		// Animation

		private void setInteracting(bool i) {
			anim.SetBool ("Interacting", i);
			CmdInteracting (i, rightHandActive);
		}

		private void setRightHand(bool b) {
			rightHandActive = b;
			anim.SetBool ("Right", b);
		}
			
		public void OverrideAnimationClip(string name, AnimationClip clip) {
			AnimatorOverrideController overrideController = new AnimatorOverrideController();
			overrideController.runtimeAnimatorController = GetEffectiveController(anim);
			overrideController[name] = clip;
			anim.runtimeAnimatorController = overrideController;
		}

		public RuntimeAnimatorController GetEffectiveController(Animator animator) {
			RuntimeAnimatorController controller = animator.runtimeAnimatorController;
			AnimatorOverrideController overrideController = controller as AnimatorOverrideController;
			while (overrideController != null)
			{
				controller = overrideController.runtimeAnimatorController;
				overrideController = controller as AnimatorOverrideController;
			}
			return controller;
		}

		// Helpers

		private bool attemptCraft() {
			if (!isLookingAtInteractable (false))
				return false;
			Craftable c = lookingAtObject.GetComponent<Craftable> ();
			if (c == null)
				return false;
			
			GUIManager.recipesScreen.openWithRecipes (c, getRecipes (c.cratingType));
			return true;
		}

		private bool attemptOpenInventory() {
			if (lookingAtObject == null)
				return false;
			
			Inventory inv = lookingAtObject.GetComponent<Inventory> ();

			if (inv == null)
				return false;

			openInventory = inv;
			CmdOpenInventory (lookingAtObject);
			GUIManager.pushScreen (GUIManager.inventoryScreen);
			return true;
		}

		private bool attemptConsume() {
			int slot = 0;
			if (rightHandActive)
				slot = 2;
			ItemStack s = hotbarInventory.getSlotCopy (slot);
			if (s == null)
				return false;
			if (ItemManager.isConsumable (s)) {
				startConsuming ();
				return true;
			} else
				return false;
		}

		private void startConsuming() {
			anim.SetBool ("Consuming", true);
			timeConsumeStart = Time.time;
			CmdConsuming (true, rightHandActive);
		}

		private void updateConsuming() {
			if (Time.time > 3f + timeConsumeStart)
				completeConsuming ();
		}

		private void stopConsuming() {
			anim.SetBool ("Consuming", false);
			CmdConsuming (false, rightHandActive);
			timeConsumeStart = -1f;
		}

		private void completeConsuming() {
			//play burp sound
			sounds.playSound(PlayerSound.ConsumeFinish);
			anim.SetBool ("Consuming", false);
			CmdCompleteConsuming ();
			CmdConsuming (false, rightHandActive);
			timeConsumeStart = -1f;
		}

		private bool attemptPlaceItem(bool rightHand) {
			if (!rightHand && hotbarInventory.getSlotCopy (0) == null)
				return false;
			
			if (rightHand && hotbarInventory.getSlotCopy (2) == null)
				return false;

			//TODO finsih this - send it throguh the command
			CmdPlaceItem (lookingAtPoint + lookingAtNormal*0.01f, rightHand);
			return true;
		}

		private void updateHolding (ItemStack stack, int hid) {
			foreach (Transform t in itemHolder_getHolderTransform(hid)) {
				if (t.GetComponent<Item> () != null) {
					NetworkServer.Destroy (t.gameObject);
				}
			}
			
			GameObject g = ItemManager.createItem (stack);
			g.GetComponent<Item> ().setHolder (this, hid);
		}

		private bool isLookingAtInteractable(bool inter) {
			if (lookingAtObject == null)
				return false;

			Interactable i = lookingAtObject.GetComponent<Interactable> ();
			if (inter)
				return (i && i.isInteractable(this));
			else return i;
		}

		private bool isLookingAtInteractable() {
			return isLookingAtInteractable (true);
		}

		private Vector3 downsampleVelocity(Vector3 v) {
			float x = Mathf.Round(v.x * 2f / maxSpeed);
			float z = Mathf.Round(v.z * 2f / maxSpeed);
			return new Vector3 (x * maxSpeed / 2f, 0f, z * maxSpeed / 2f);
		}

		private void jump() {
			if (!grounded)
				return;

			CmdJump ();
			anim.SetTrigger ("Jump");
			shouldJump = true;
		}

		private void onLand() {
			if (groundObject.layer == 8)
				effects.playEffect (WorldTerrain.getMaterialEffects(transform.position).stepEffect, transform.position, Vector3.up, 50f);
			else if (groundObject.GetComponent<FXMaterial> ())
				effects.playEffect (groundObject.GetComponent<FXMaterial> ().effects.stepEffect, transform.position, Vector3.up, 50f);
		}
			
		private void swing() {
			CmdSwing (rightHandActive);
			anim.SetTrigger ("Swing");
		}

		private void onSwingHit() {
			if (lookingAtObject == null)
				return;
			if (lookingAtObject.layer == 8)
				effects.playEffect (WorldTerrain.getMaterialEffects(transform.position).hitEffect, lookingAtPoint, lookingAtNormal, 50f);
			else if (lookingAtObject.GetComponent<FXMaterial> ())
				effects.playEffect (lookingAtObject.GetComponent<FXMaterial> ().effects.hitEffect, lookingAtPoint, lookingAtNormal, 50f);
			CmdOnHit (lookingAtObject);
		}

		private bool isConsuming() {
			return timeConsumeStart > 0f;
		}
			
		private IEnumerator footstepSoundPlay() {
			while (true) {
				if (grounded && isMoving()) {
					//can pass clothing items here when they exist
					if (groundObject.layer == 8) {
						effects.playEffect (WorldTerrain.getMaterialEffects (transform.position).stepEffect, transform.position, Vector3.up, 50f);
					} else if (groundObject.GetComponent<FXMaterial> ())
						effects.playEffect (groundObject.GetComponent<FXMaterial> ().effects.stepEffect, transform.position, Vector3.up, 50f);
				}
				if (speed == 0f)
					yield return new WaitForSeconds (1f);
				else
					yield return new WaitForSeconds ((maxSpeed/speed)/2);
			}
		}

		[Server]
		private void die() {
			// copy out inventory into a dead body
			GameObject g = Instantiate(deadPrefab);
			g.transform.position = transform.position;
			NetworkServer.Spawn (g);
			StartCoroutine (deathTransferInventory (g));

			// drop things in hands
			hotbarInventory.dropAll(transform.position + transform.up *2f);

			// do other things that fuck them
			health = maxHealth;
			hunger = maxHunger;
			thirst = maxThirst;

			// move to spawn point
			transform.position = WorldTerrain.getSpawnPoint().position;
		}

		private IEnumerator deathTransferInventory(GameObject g) {
			yield return new WaitForSeconds (1f);
			g.GetComponent<Inventory> ().transfer (mainInventory);
			mainInventory.clearAll ();
		}

		#endregion

	}
		
}