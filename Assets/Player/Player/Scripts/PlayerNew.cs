using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using PolyItem;
using PolyEntity;
using PolyWorld;
using PolyEffects;
using PolyNet;

namespace PolyPlayer {

	public class PlayerNew : PolyNetBehaviour, Interactor, InventoryListener, ItemHolder, Living {

		/*
		 * Spawn Data Ideas
		 * 
		 * Player ID
		 * 
		 */

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
		public Material[] hairColors;
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
		//TODO
		public float maxHealth;
		public float maxHunger;
		public float maxThirst;
		private float health;
		private float hunger;
		private float thirst;
		public int playerID;

		#endregion

		/*
		 * 
		 * Public Interface
		 * 
		 */

		#region

		// Living Interface

		public void living_hurt(Living l, float d) {
			health -= d * (maxHunger / hunger);
			if (health <= 0f)
				die ();

			anim.SetTrigger ("Hurt");
			sounds.playSound(PlayerSound.Hurt);
			identity.sendBehaviourPacket (new PacketAnimTrigger (this, 1));
		}

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

		public void interactor_giveItem(Item i) {
			if (!hotbarInventory.insert (i)) {
				if (!mainInventory.insert (i)) {
					return;
				}
			}
			sounds.rpcPlaySound (PlayerSound.ItemPickup);
			PolyNetWorld.destroy (i.gameObject);
		}

		public Vector3 interactor_getInteractionPosition() {
			return lookingAtPoint;
		}

		public Vector3 interactor_getInteractionNormal() {
			return lookingAtNormal;
		}

		// Inventory Listener Interface
		public void inventoryListener_onSlotChange(Inventory inv, int i, ItemStack s) {
			if (inv == hotbarInventory)
				updateHolding (s, i);
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
			return identity.isLocalPlayer;
		}

		// GUI Interface

		public void setFancyGraphics(bool b) {
			Camera.main.GetComponent<FlareLayer> ().enabled = b;
			Camera.main.GetComponent<Bloom> ().enabled = b;
			Camera.main.GetComponent<SunShafts> ().enabled = b;
			Camera.main.GetComponent<ScreenSpaceAmbientOcclusion> ().enabled = b;
			Camera.main.GetComponent<DepthOfField> ().enabled = b;
			Camera.main.GetComponent<Antialiasing> ().enabled = b;
		}

		//TODO - after inventory and craftables are in

		public void onSlotUpdate(int bindingID, int slotID, ItemStack s) {
//			CmdSetSlot (bindingID, slotID, new NetworkItemStack (s));
		}

		public void setCraftableRecipe(Craftable c, Recipe r) {
//			if (r == null)
//				CmdSetCraftableRecipe (c.gameObject, new NetworkItemStack (null), new NetworkItemStackArray (null));
//			else
//				CmdSetCraftableRecipe (c.gameObject, new NetworkItemStack (r.output), new NetworkItemStackArray (r.input));
		}

		public void setCraftableInput(Craftable c, ItemStack[] i) {
//			CmdSetCraftableInput (c.gameObject, new NetworkItemStackArray (i));
		}

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

		public void polyPlayer_receiveChat(string name, string message, float distance) {
			if (name != null && name.Length != 0)
				GUIManager.chat.displayMessage (name + ": " + message, distance);
			else
				GUIManager.chat.displayMessage (message, distance);

		}

		public void polyPlayer_setPlayerID(int playerID) {
			this.playerID = playerID;
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

		// Packet Handling

		public override void handleBehaviourPacket (PacketBehaviour p) {
			base.handleBehaviourPacket (p);
			if (p.id == 4) {
				PacketPlayerTransform o = (PacketPlayerTransform)p;
				if (PolyServer.isActive)
					cmd_updateTransform (o.velocity, o.rotationalVelocity, o.position, o.euler, o.pitch);
				else
					rpc_updateTransform_Broadcast (o.velocity, o.rotationalVelocity, o.position, o.euler, o.pitch);
			} else if (p.id == 5) {
				PacketPlayerTransformDenied o = (PacketPlayerTransformDenied)p;
				rpc_transformDenied (o.position);
			} else if (p.id == 6) {
				PacketAnimTrigger o = (PacketAnimTrigger)p;
				switch (o.triggerId) {
				case 0:
					if (PolyServer.isActive)
						cmd_jump ();
					else
						rpc_jump_Broadcast ();
					break;
				case 1:
					rpc_hurt ();
					break;
				default:
					break;
				}
			} else if (p.id == 8) {
				PacketAnim2HandedTrigger o = (PacketAnim2HandedTrigger)p;
				switch (o.triggerId) {
				case 0:
					if (PolyServer.isActive)
						cmd_swing (o.rightHand);
					else
						rpc_swing_Broadcast (o.rightHand);
					break;
				default:
					break;
				}
			} else if (p.id == 9) {
				PacketAnim2HandedBool o = (PacketAnim2HandedBool)p;
				switch (o.boolId) {
				case 0:
					if (PolyServer.isActive)
						cmd_interacting (o.value, o.rightHand);
					else
						rpc_interacting_Broadcast (o.value, o.rightHand);
					break;
				case 1:
					if (PolyServer.isActive)
						cmd_consuming (o.value, o.rightHand);
					else
						rpc_consuming_Broadcast (o.value, o.rightHand);
					break;
				default:
					break;
				}
			} else if (p.id == 10) {
				PacketPlayerHit o = (PacketPlayerHit)p;
				cmd_onHit (o.hitObject, o.hitPoint, o.hitNormal);
			} else if (p.id == 11) {
				PacketMetadata o = (PacketMetadata)p;
				switch (o.metadata) {
				case 0:
					cmd_completeConsuming ();
					break;
				default:
					break;
				} 
			} else if (p.id == 12) {
				PacketPlaceItem o = (PacketPlaceItem)p;
				cmd_placeItem (o.position, o.rightHand);
			}
		}

		#endregion

		/*
		 * 
		 * Client->Server Networked Interface
		 * 
		 */

		#region

		// Animation Commands

		private void cmd_interacting(bool i, bool rightHand) {
			setRightHand (rightHand);
			anim.SetBool ("Interacting", i);
			identity.sendBehaviourPacket (new PacketAnim2HandedBool (this, 0, i, rightHand));
		}

		private void cmd_consuming(bool e, bool rightHand) {
			setRightHand (rightHand);
			anim.SetBool ("Consuming", e);
			identity.sendBehaviourPacket (new PacketAnim2HandedBool (this, 1, e, rightHand));
		}

		private void cmd_swing(bool rightHand) {
			setRightHand (rightHand);
			anim.SetTrigger ("Swing");
			identity.sendBehaviourPacket (new PacketAnim2HandedTrigger (this, 0, rightHand));
		}

		private void cmd_jump() {
			anim.SetTrigger ("Jump");
			shouldJump = true;
			identity.sendBehaviourPacket (new PacketAnimTrigger (this, 0));
		}

		// Locomotion Commands

		private void cmd_updateTransform (Vector3 v, float rv, Vector3 p, Vector3 e, float pi) {
			if (Vector3.Distance (transform.position, p) > ipPositionServerAuth) {
				p = transform.position;
				identity.sendBehaviourPacket (new PacketPlayerTransformDenied (this, p));
			}

			velocity = v;
			rotationalVelocity = rv;
			transform.position = p;
			transform.eulerAngles = e;
			pitch = pi;

			identity.sendBehaviourPacket (new PacketPlayerTransform (this, v, rv, p, e, pi));
		}

		// Inventory Commands

		private void cmd_setSlot(int inventoryID, int slotID, NetworkItemStack stack) {
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

		private void cmd_openInventory(GameObject g) {
			openInventory = g.GetComponent<Inventory> ();
		}

		private void cmd_setCraftableRecipe(GameObject g, NetworkItemStack o, NetworkItemStackArray i) {
			Recipe r = Recipe.unwrapRecipe(o,i);
			g.GetComponent<Craftable> ().setRecipe (r);
		}

		private void cmd_setCraftableInput(GameObject g, NetworkItemStackArray i) {
			g.GetComponent<Craftable> ().setInput (ItemStack.unwrapNetworkStackArray (i));
		}

		private void cmd_hotbarSwitch(bool rightHand) {
			hotbarInventory.switchBack (rightHand);
		}

		// Combat Commands

		private void cmd_onHit(GameObject g, Vector3 point, Vector3 norms) {
			if (g == null)
				return;
			Living l = g.GetComponent<Living> ();
			if (l != null) {
				l.living_hurt (this, 5f);
				return;
			}
			Interactable i = g.GetComponent<Interactable> ();
			if (i != null && i.isInteractable(this)) {
				lookingAtPoint = point;
				lookingAtNormal = norms;
				g.GetComponent<Interactable> ().interact (this, 1f);
			}
		}

		// Eating Commands

		private void cmd_completeConsuming() {
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

		private void cmd_placeItem(Vector3 p, bool rightHand) {

			int slot = 0;
			if (rightHand)
				slot = 2;

			GameObject g = ItemManager.createItemForPlacing (hotbarInventory.getSlotCopy (slot));
			g.GetComponent<Interactable> ().setPosition (p);
			hotbarInventory.decreaseSlot (slot);
		}

		#endregion

		/*
		 * 
		 * Server->Client Networked Interface
		 * 
		 */

		#region

		// Server Auth

		private void rpc_hurt() {
			anim.SetTrigger ("Hurt");
			sounds.playSound(PlayerSound.Hurt);
		}

		// Broadcasts

		private void rpc_interacting_Broadcast(bool i, bool rightHand) {
			if (!identity.isLocalPlayer) {
				setRightHand (rightHand);
				anim.SetBool ("Interacting", i);
			}
		}

		private void rpc_consuming_Broadcast(bool e, bool rightHand) {
			if (!identity.isLocalPlayer) {
				setRightHand (rightHand);
				anim.SetBool ("Consuming", e);
			}
		}

		private void rpc_swing_Broadcast(bool rightHand) {
			if (!identity.isLocalPlayer) {
				setRightHand (rightHand);
				anim.SetTrigger ("Swing");
			}
		}

		private void rpc_updateTransform_Broadcast(Vector3 v, float rv, Vector3 p, Vector3 e, float pi) {
			if (identity.isLocalPlayer)
				return;
			
			velocity = v;
			rotationalVelocity = rv;
			pitch = Mathf.Lerp(pitch, pi, 0.5f);
			if (Vector3.Distance(transform.position, p) > ipPositionAllowance)
				transform.position = Vector3.Lerp(transform.position, p, 0.5f);
			if (Mathf.Abs (transform.eulerAngles.y - e.y) > ipRotationAllowance)
				transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler(e), 0.5f);
			transform.eulerAngles = e;
		}

		private void rpc_transformDenied(Vector3 pos) {
			if (identity.isLocalPlayer)
				transform.position = pos;
		}

		private void rpc_jump_Broadcast() {
			if (!identity.isLocalPlayer) {
				anim.SetTrigger ("Jump");
				shouldJump = true;
			}
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
			PolyNetWorld.registerPrefab (deadPrefab);

//			sounds = GetComponent <SoundManager> ();
//			effects = GetComponent<EffectListener> ();
			//TODO - vitals
//			if (NetworkServer.active)
//				StartCoroutine (updateVitals ());

			rigidBody = GetComponent<Rigidbody> ();
			anim = GetComponent<Animator> ();
			recipes = new List<Recipe> ();
			foreach (Recipe r in defaultRecipes) {
				recipes.Add (r);
			}
			//TODO - after inventory
//			foreach (Inventory i in GetComponents<Inventory> ()) {
//				if (i is HotBarInventory)
//					hotbarInventory = (HotBarInventory)i;
//				else
//					mainInventory = i;
//
//			}

			StartCoroutine(lateStart ());

			if (!identity.isLocalPlayer) {
				//TODO - spawn data player id
//				if (playerID != 0)
//					setUpHair ();
				return;
			}

			rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
			setUpCamera ();
			setUpLocalAnimations ();
			crosshair = FindObjectOfType<Crosshair> ();
			GUIManager.closeGUI ();
			StartCoroutine(networkTransformUpdate ());
			//TODO - connect sound manager
//			StartCoroutine(footstepSoundPlay ());
		}

		private void setUpHair() {
			if (playerID > 10)
				return;

			hairMesh.GetComponent<SkinnedMeshRenderer> ().sharedMesh = hairMeshes[playerID-1];
			hairMesh.GetComponent<SkinnedMeshRenderer> ().material = hairColors [playerID - 1];
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

			//TODO - connect gui, get inventory working
//			if (identity.isLocalPlayer)
//				GUIManager.setPlayer (this);
//			if (PolyServer.isActive)
//				hotbarInventory.startListening (this, true);

			// TODO delete this when we have a network manager doing on login notifications
			//
			if (PolyServer.isActive) {
				Item[] items = FindObjectsOfType<Item> ();
				foreach (Item i in items) {
					i.OnPlayerConnected ();
				}
			}
			//
			//
		}
		
		// Update

		private void Update () {
			updateLocomotion ();

			if (!identity.isLocalPlayer)
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
			//TODO - connect gui
//			if (Input.GetKeyDown (KeyCode.Z)) {
//				GUIManager.pushScreen (GUIManager.characterScreen);
//			}
//			if (Input.GetKeyDown (KeyCode.C)) {
//				GUIManager.recipesScreen.openWithRecipes(getRecipes(CraftingType.Hand));
//			}
//			if (Input.GetKeyDown(KeyCode.Q)) {
//				CmdHotbarSwitch (false);
//			}
//			if (Input.GetKeyDown(KeyCode.E)) {
//				CmdHotbarSwitch (true);
//			}
//
//			if (Input.GetKeyDown(KeyCode.Alpha1)) {
//				attemptPlaceItem (false);
//			}
//			if (Input.GetKeyDown(KeyCode.Alpha3)) {
//				attemptPlaceItem (true);
//			}

//			if (Input.GetKeyDown(KeyCode.T)) {
//				GUIManager.setChatOpen (true);
//			}
//
//			if (Input.GetKeyDown(KeyCode.Tab)) {
//				GUIManager.pushScreen (GUIManager.settingsScreen);
//			}

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

		}

		private IEnumerator networkTransformUpdate() {
			while (true) {
				identity.sendBehaviourPacket (new PacketPlayerTransform (this, downsampleVelocity (velocity), rotationalVelocity, transform.position, transform.eulerAngles, pitch));
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
				if (lookingAtObject.GetComponent<Interactable> ().maxStrength == 0f)
					identity.sendBehaviourPacket (new PacketPlayerHit (this, lookingAtObject, lookingAtPoint, lookingAtNormal));
				else
					setInteracting (true);
			}
		}

		private void primaryActionUpdate() {
			if (!isLookingAtInteractable (true)) {
				cancelActions ();
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
			if (identity.isLocalPlayer)
				crosshair.setHighlighted (i);
			identity.sendBehaviourPacket(new PacketAnim2HandedBool(this, 0, i, rightHandActive));
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
			//TODO - after inventory
//			CmdOpenInventory (lookingAtObject);
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
			identity.sendBehaviourPacket(new PacketAnim2HandedBool(this, 1, true, rightHandActive));
		}

		private void updateConsuming() {
			if (Time.time > 3f + timeConsumeStart)
				completeConsuming ();
		}

		private void stopConsuming() {
			anim.SetBool ("Consuming", false);
			identity.sendBehaviourPacket(new PacketAnim2HandedBool(this, 1, false, rightHandActive));
			timeConsumeStart = -1f;
		}

		private void completeConsuming() {
			//play burp sound
			sounds.playSound(PlayerSound.ConsumeFinish);
			anim.SetBool ("Consuming", false);
			identity.sendBehaviourPacket (new PacketMetadata (this, 1));
			identity.sendBehaviourPacket(new PacketAnim2HandedBool(this, 1, true, rightHandActive));
			timeConsumeStart = -1f;
		}

		private bool attemptPlaceItem(bool rightHand) {
			if (!rightHand && hotbarInventory.getSlotCopy (0) == null)
				return false;

			if (rightHand && hotbarInventory.getSlotCopy (2) == null)
				return false;

			identity.sendBehaviourPacket(new PacketPlaceItem(this, lookingAtPoint + lookingAtNormal*0.01f, rightHand));
			return true;
		}

		private void updateHolding (ItemStack stack, int hid) {
			foreach (Transform t in itemHolder_getHolderTransform(hid)) {
				if (t.GetComponent<Item> () != null) {
					PolyNetWorld.destroy (t.gameObject);
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

			identity.sendBehaviourPacket (new PacketAnimTrigger(this, 0));
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
			identity.sendBehaviourPacket(new PacketAnim2HandedTrigger(this, 0, rightHandActive));
			anim.SetTrigger ("Swing");
		}

		private void onSwingHit() {
			if (lookingAtObject == null)
				return;
			if (lookingAtObject.layer == 8)
				effects.playEffect (WorldTerrain.getMaterialEffects(transform.position).hitEffect, lookingAtPoint, lookingAtNormal, 50f);
			else if (lookingAtObject.GetComponent<FXMaterial> ())
				effects.playEffect (lookingAtObject.GetComponent<FXMaterial> ().effects.hitEffect, lookingAtPoint, lookingAtNormal, 50f);
			identity.sendBehaviourPacket (new PacketPlayerHit (this, lookingAtObject, lookingAtPoint, lookingAtNormal));
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

		private void die() {
			// copy out inventory into a dead body
			GameObject g = PolyNetWorld.instantiate(deadPrefab);
			g.transform.position = transform.position;
//			NetworkServer.Spawn (g);
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