using UnityEngine;
using System.Collections;

public class CrafterController : MonoBehaviour{
	private Animator animator;
	private GameObject axe;
	private GameObject hammer;
	private GameObject fishingpole;
	private GameObject shovel;
	private GameObject box;
	private GameObject food;
	private GameObject drink;
	private GameObject saw;
	private GameObject pickaxe;
	private GameObject sickle;
	private GameObject rake;
	private GameObject chair;
	private GameObject ladder;
	private GameObject lumber;
	private GameObject pushpull;
	private GameObject sphere;
	private GameObject cart;
	private GameObject paintbrush;
	float rotationSpeed = 5;
	Vector3 inputVec;
	bool isMoving;
	bool isPaused;

	public enum CharacterState{
		Idle,
		Item,
		Box,
		Fishing,
		Hammer,
		Digging,
		Chopping,
		Food,
		Drink,
		Axe,
		Shovel,
		FishingPole,
		Saw,
		Sawing,
		PickAxe,
		PickAxing,
		Sickle,
		Rake,
		Raking,
		Sit,
		Climb,
		PushPull,
		Lumber,
		Sphere,
		Pray,
		Cart,
		Kneel,
		Painting
	};
	
	public CharacterState charState;

	void Awake(){
		animator = this.GetComponent<Animator>();
		axe = GameObject.Find("Axe");
		hammer = GameObject.Find("Hammer");
		fishingpole = GameObject.Find("FishingPole");
		shovel = GameObject.Find("Shovel");
		box = GameObject.Find("Carry");
		food = GameObject.Find("Food");
		drink = GameObject.Find("Drink");
		saw = GameObject.Find("Saw");
		pickaxe = GameObject.Find("PickAxe");
		sickle = GameObject.Find("Sickle");
		rake = GameObject.Find("Rake");
		chair = GameObject.Find("Chair");
		ladder = GameObject.Find("Ladder");
		lumber = GameObject.Find("Lumber");
		pushpull = GameObject.Find("PushPull");
		sphere = GameObject.Find("Sphere");
		cart = GameObject.Find("Cart");
		paintbrush = GameObject.Find("Paintbrush");
	}

	void Start(){
		StartCoroutine(COShowItem("none", 0f));
		charState = CharacterState.Idle;
	}

	void Update(){
		//Get input from controls
		float z = Input.GetAxisRaw("Horizontal");
		float x = -(Input.GetAxisRaw("Vertical"));
		inputVec = new Vector3(x, 0, z);
		//if there is some input
		if(x != 0 || z != 0 ){  
			//set that character is moving
			animator.SetBool("Moving", true);
			isMoving = true;
			//if we are running, set the animator
			if(Input.GetButton("Jump")){
				animator.SetBool("Running", true);
			}
			else{
				animator.SetBool("Running", false);
			}
		}
		else{
			//character is not moving
			animator.SetBool("Moving", false);
			isMoving = false;
		} 
		if(Input.GetKey(KeyCode.R)){
			this.gameObject.transform.position = new Vector3(0,0,0);
		}
		//If not pushing cart
		if(charState != CharacterState.Cart){
			animator.SetFloat("VelocityX", -x);
			animator.SetFloat("VelocityY", z);
			//sent velocity to animator
			animator.SetFloat("Velocity", UpdateMovement());  
		} 
		else{
			animator.SetFloat("Velocity", -x);
		}
	}

	//face character along input direction
	void RotateTowardsMovementDir(){
		if(inputVec != Vector3.zero){
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVec), Time.deltaTime * rotationSpeed);
		}
	}

	//movement of character
	float UpdateMovement(){
		//get movement input from controls
		Vector3 motion = inputVec;
		//reduce input for diagonal movement
		motion *= (Mathf.Abs(inputVec.x) == 1 && Mathf.Abs(inputVec.z) == 1)?0.7f:1;
		//if not paused, face character along input direction
		if(!isPaused && charState != CharacterState.Cart){
			RotateTowardsMovementDir();
		}
		return inputVec.magnitude;
	}

	void OnGUI(){
		if(charState == CharacterState.Idle && !isMoving){
			isPaused = false;
			if(GUI.Button(new Rect (25, 25, 150, 30), "Get Hammer")){
				animator.SetTrigger("ItemBeltTrigger");
				StartCoroutine(COMovePause(1f));
				StartCoroutine(COShowItem("hammer", 0.5f));
				charState = CharacterState.Hammer;
			}
			if(GUI.Button(new Rect (195, 25, 150, 30), "Get Paintbrush")){
				animator.SetTrigger("ItemBeltTrigger");
				StartCoroutine(COMovePause(1f));
				StartCoroutine(COShowItem("paintbrush", 0.5f));
				charState = CharacterState.Painting;
			}
			if(GUI.Button(new Rect (25, 65, 150, 30), "Get Axe")){
				animator.SetTrigger("ItemBackTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("axe", 0.5f));
				charState = CharacterState.Axe;
			}
			if(GUI.Button(new Rect (25, 105, 150, 30), "Get PickAxe")){
				animator.SetTrigger("ItemBackTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("pickaxe", 0.5f));
				charState = CharacterState.PickAxe;
			}
			if(GUI.Button(new Rect (25, 145, 150, 30), "Pickup Shovel")){
				animator.SetTrigger("ItemPickupTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("shovel", 0.3f));
				charState = CharacterState.Shovel;
			}
			if(GUI.Button(new Rect (25, 185, 150, 30), "PullUp Fishing Pole")){
				animator.SetTrigger("ItemPullUpTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("fishingpole", 0.5f));
				charState = CharacterState.FishingPole;
			}
			if(GUI.Button(new Rect (25, 225, 150, 30), "Take Food")){
				animator.SetTrigger("ItemTakeTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("food", 0.3f));
				charState = CharacterState.Food;
			}
			if(GUI.Button(new Rect (25, 265, 150, 30), "Recieve Drink")){
				animator.SetTrigger("ItemRecieveTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("drink", 0.5f));
				charState = CharacterState.Drink;
			}
			if(GUI.Button(new Rect (25, 305, 150, 30), "Pickup Box")){
				animator.SetTrigger("CarryPickupTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("box", 0.5f));
				charState = CharacterState.Box;
			}
			if(GUI.Button(new Rect (195, 305, 150, 30), "Pickup Lumber")){
				animator.SetTrigger("LumberPickupTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("lumber", 0.5f));
				charState = CharacterState.Lumber;
			}
			if(GUI.Button(new Rect (370, 305, 150, 30), "Pickup Overhead")){
				animator.SetTrigger("CarryOverheadPickupTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("sphere", 0.5f));
				charState = CharacterState.Sphere;
			}
			if(GUI.Button(new Rect (25, 345, 150, 30), "Recieve Box")){
				animator.SetTrigger("CarryRecieveTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("box", 0.5f));
				charState = CharacterState.Box;
			}
			if(GUI.Button(new Rect (25, 385, 150, 30), "Get Saw")){
				animator.SetTrigger("ItemBeltTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("saw", 0.5f));
				charState = CharacterState.Saw;
			}
			if(GUI.Button(new Rect (25, 425, 150, 30), "Get Sickle")){
				animator.SetTrigger("ItemBeltTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("sickle", 0.5f));
				charState = CharacterState.Sickle;
			}
			if(GUI.Button(new Rect (25, 465, 150, 30), "Get Rake")){
				animator.SetTrigger("ItemBackTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("rake", 0.5f));
				charState = CharacterState.Rake;
			}
			if(GUI.Button(new Rect (25, 505, 150, 30), "Sit")){
				animator.SetTrigger("ChairSitTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("chair", 0.3f));
				charState = CharacterState.Sit;
			}
			if(GUI.Button(new Rect (200, 505, 150, 30), "Push Cart")){
				animator.SetTrigger("CartPullGrabTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("cart", 0.25f));
				charState = CharacterState.Cart;
			}
			if(GUI.Button(new Rect (25, 545, 150, 30), "Gather")){
				animator.SetTrigger("GatherTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect (200, 545, 150, 30), "Gather Praying")){
				animator.SetTrigger("GatherPrayingTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
	      if(GUI.Button(new Rect (200, 585, 150, 30), "Wave1")){
	        animator.SetTrigger("Wave1Trigger");
	        StartCoroutine(COMovePause(2.2f));
	      }
	      if(GUI.Button(new Rect (375, 545, 150, 30), "Cheer1")){
	        animator.SetTrigger("Cheer1Trigger");
	        StartCoroutine(COMovePause(2.2f));
	      }
			if(GUI.Button(new Rect (25, 585, 150, 30), "Scratch Head")){
				animator.SetTrigger("Bored1Trigger");
				StartCoroutine(COMovePause(1.2f));
			}
	      if(GUI.Button(new Rect (375, 585, 150, 30), "Cheer2")){
	        animator.SetTrigger("Cheer2Trigger");
	        StartCoroutine(COMovePause(2.2f));
	      }
	      if(GUI.Button(new Rect (375, 630, 150, 30), "Cheer3")){
	        animator.SetTrigger("Cheer3Trigger");
	        StartCoroutine(COMovePause(2.2f));
	      }
			if(GUI.Button(new Rect (375, 670, 150, 30), "Fear")){
				animator.SetTrigger("FearTrigger");
				StartCoroutine(COMovePause(4f));
			}
			if(GUI.Button(new Rect (25, 625, 150, 30), "Climb")){
				animator.SetTrigger("ClimbStartTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("ladder", 0.3f));
				charState = CharacterState.Climb;
			}
			if(GUI.Button(new Rect (200, 625, 150, 30), "Climb Top")){
				this.gameObject.transform.position += new Vector3(0,3,0);
				animator.SetTrigger("ClimbOnTopTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("ladder", 0.3f));
				charState = CharacterState.Climb;
			}
			if(GUI.Button(new Rect (200, 665, 150, 30), "Pray")){
				animator.SetTrigger("Pray-DownTrigger");
				charState = CharacterState.Pray;
				isPaused = true;
			}
			if(GUI.Button(new Rect (25, 665, 150, 30), "Push Pull")){
				animator.SetTrigger("PushPullStartTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("pushpull", 0.3f));
				charState = CharacterState.PushPull;
			}
		}
		if(charState == CharacterState.Cart){
			if(GUI.Button(new Rect(200, 505, 150, 30), "Release Cart")){
				animator.SetTrigger("CartPullReleaseTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.75f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Pray){
			if(GUI.Button(new Rect (200, 665, 150, 30), "Stand")){
				animator.SetTrigger("Pray-StandTrigger");
				StartCoroutine(COMovePause(1.1f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Hammer && !isMoving){
			if(GUI.Button(new Rect (25, 25, 150, 30), "Hammer Wall")){
				animator.SetTrigger("HammerWallTrigger");
				StartCoroutine(COMovePause(1.9f));
			}
			if(GUI.Button(new Rect (25, 65, 150, 30), "Hammer Table")){
				animator.SetTrigger("HammerTableTrigger");
				StartCoroutine(COMovePause(1.9f));
			}
			if(GUI.Button(new Rect (25, 105, 150, 30), "Give Hammer")){
				animator.SetTrigger("ItemHandoffTrigger");
				StartCoroutine(COMovePause(1f));
				StartCoroutine(COShowItem("none", 0.4f));
				StartCoroutine(COChangeCharacterState(0.4f, CharacterState.Idle));
			}
			if(GUI.Button(new Rect (25, 145, 150, 30), "Put Away Hammer")){
				animator.SetTrigger("ItemBeltAwayTrigger");
				StartCoroutine(COMovePause(1f));
				StartCoroutine(COShowItem("none", 0.5f));
				StartCoroutine(COChangeCharacterState(0.4f, CharacterState.Idle));
			}
			if(GUI.Button(new Rect (25, 185, 150, 30), "Put Down Hammer")){
				animator.SetTrigger("ItemPutdownTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.7f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect (25, 225, 150, 30), "Drop Hammer")){
				animator.SetTrigger("ItemDropTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.4f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 265, 150, 30), "Kneel")){
				animator.SetTrigger("ItemKneelDownTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.Kneel;
			}
			if(GUI.Button(new Rect(25, 305, 150, 30), "Chisel")){
				animator.SetTrigger("ItemChiselTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
		}
		if(charState == CharacterState.Painting && !isMoving){
			if(GUI.Button(new Rect (25, 25, 150, 30), "Paint Wall")){
				animator.SetTrigger("ItemPaintTrigger");
				StartCoroutine(COMovePause(1.9f));
			}
			if(GUI.Button(new Rect (25, 65, 150, 30), "Fill Brush")){
				animator.SetTrigger("ItemPaintRefillTrigger");
				StartCoroutine(COMovePause(1.9f));
			}
			if(GUI.Button(new Rect (25, 105, 150, 30), "Give Paintbrush")){
				animator.SetTrigger("ItemHandoffTrigger");
				StartCoroutine(COMovePause(1f));
				StartCoroutine(COShowItem("none", 0.4f));
				StartCoroutine(COChangeCharacterState(0.4f, CharacterState.Idle));
			}
			if(GUI.Button(new Rect (25, 145, 150, 30), "Put Away Paintbrush")){
				animator.SetTrigger("ItemBeltAwayTrigger");
				StartCoroutine(COMovePause(1f));
				StartCoroutine(COShowItem("none", 0.5f));
				StartCoroutine(COChangeCharacterState(0.4f, CharacterState.Idle));
			}
			if(GUI.Button(new Rect (25, 185, 150, 30), "Put Down Paintbrush")){
				animator.SetTrigger("ItemPutdownTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.7f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect (25, 225, 150, 30), "Drop Paintbrush")){
				animator.SetTrigger("ItemDropTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.4f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Kneel){
			if(GUI.Button(new Rect(25, 30, 150, 30), "Hammer")){
				animator.SetTrigger("ItemKneelHammerTrigger");
				StartCoroutine(COMovePause(1.1f));
			}
			if(GUI.Button(new Rect(25, 265, 150, 30), "Stand")){
				animator.SetTrigger("ItemKneelStandTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.Hammer;
			}
		}
	   if(charState == CharacterState.Drink && isMoving){
	      if(GUI.Button(new Rect (25, 25, 150, 30), "Drink")){
	        animator.SetTrigger("DrinkUpperTrigger");
	      }
	   }
		if(charState == CharacterState.Drink && !isMoving){
			if(GUI.Button(new Rect (25, 25, 150, 30), "Drink")){
				animator.SetTrigger("ItemDrinkTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect (25, 65, 150, 30), "Water")){
				animator.SetTrigger("ItemWaterTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect (25, 105, 150, 30), "Give Drink")){
				animator.SetTrigger("ItemHandoffTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect (25, 145, 150, 30), "Put Drink Away")){
				animator.SetTrigger("ItemBeltAwayTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect (25, 185, 150, 30), "Put Drink Down")){
				animator.SetTrigger("ItemPutdownTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect (25, 225, 150, 30), "Drop Drink")){
				animator.SetTrigger("ItemDropTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.4f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Food && isMoving){
			if(GUI.Button(new Rect(25, 25, 150, 30), "Eat Food")){
				animator.SetTrigger("EatUpperTrigger");
			}
		}
		if(charState == CharacterState.Food && !isMoving){
			if(GUI.Button(new Rect(25, 25, 150, 30), "Eat Food")){
				animator.SetTrigger("ItemEatTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Give Food")){
				animator.SetTrigger("ItemHandoffTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 105, 150, 30), "Put Food Down")){
				animator.SetTrigger("ItemPutdownTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.7f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 145, 150, 30), "Put Food Away")){
				animator.SetTrigger("ItemBeltAwayTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 185, 150, 30), "Drop Food")){
				animator.SetTrigger("ItemDropTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.4f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 225, 150, 30), "Plant Food")){
				animator.SetTrigger("ItemPlantTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.6f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Sickle && !isMoving){
			if(GUI.Button(new Rect(25, 25, 150, 30), "Use Sickle")){
				animator.SetTrigger("ItemSickleUse");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Give Sickle")){
				animator.SetTrigger("ItemHandoffTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 105, 150, 30), "Put Sickle Down")){
				animator.SetTrigger("ItemPutdownTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.7f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 145, 150, 30), "Put Sickle Away")){
				animator.SetTrigger("ItemBeltAwayTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 185, 150, 30), "Drop Sickle")){
				animator.SetTrigger("ItemDropTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.4f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Axe && isMoving){
			if(GUI.Button(new Rect(25, 25, 150, 30), "Chop Upper Horizontal")){
				animator.SetTrigger("ChopHorizontalUpperTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Chop Upper Vertical")){
				animator.SetTrigger("ChopVerticalUpperTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
		}
		if(charState == CharacterState.Axe && !isMoving){
			isPaused = false;
			if(GUI.Button(new Rect(25, 25, 150, 30), "Start Chopping")){
				animator.SetTrigger("ChoppingStartTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.Chopping;
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Put Axe Away")){
				animator.SetTrigger("ItemBackAwayTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 105, 150, 30), "Give Axe")){
				animator.SetTrigger("ItemHandoffTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 145, 150, 30), "Put Axe Down")){
				animator.SetTrigger("ItemPutdownTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.7f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 185, 150, 30), "Drop Axe")){
				animator.SetTrigger("ItemDropTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.4f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.PickAxe && isMoving){
			if(GUI.Button(new Rect(25, 25, 150, 30), "Chop Upper Horizontal")){
				animator.SetTrigger("ChopHorizontalUpperTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Chop Upper Vertical")){
				animator.SetTrigger("ChopVerticalUpperTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
		}
		if(charState == CharacterState.PickAxe && !isMoving){
			isPaused = false;
			if(GUI.Button(new Rect(25, 25, 150, 30), "Start PickAxing")){
				animator.SetTrigger("ChoppingStartTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.PickAxing;
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Put PickAxe Away")){
				animator.SetTrigger("ItemBackAwayTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 105, 150, 30), "Give PickAxe")){
				animator.SetTrigger("ItemHandoffTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 145, 150, 30), "Put PickAxe Down")){
				animator.SetTrigger("ItemPutdownTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.7f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 185, 150, 30), "Drop PickAxe")){
				animator.SetTrigger("ItemDropTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.4f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Saw && !isMoving){
			isPaused = false;
			if(GUI.Button(new Rect(25, 25, 150, 30), "Start Sawing")){
				animator.SetTrigger("SawStartTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.Sawing;
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Put Saw Away")){
				animator.SetTrigger("ItemBeltAwayTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 105, 150, 30), "Give Saw")){
				animator.SetTrigger("ItemHandoffTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 145, 150, 30), "Drop Saw")){
				animator.SetTrigger("ItemDropTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.4f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Sawing && !isMoving){
			isPaused = true;
			if(GUI.Button(new Rect(25, 25, 150, 30), "Finish Sawing")){
				animator.SetTrigger("SawFinishTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.Saw;
			}
		}
		if(charState == CharacterState.Chopping && !isMoving){
			isPaused = true;
			if(GUI.Button(new Rect(25, 25, 150, 30), "Chop Vertical")){
				animator.SetTrigger("ChopVerticalTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Chop Horizontal")){
				animator.SetTrigger("ChopHorizontalTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 105, 150, 30), "Chop Diagonal")){
				animator.SetTrigger("ChopDiagonalTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 145, 150, 30), "Chop Ground")){
				animator.SetTrigger("ChopGroundTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 185, 150, 30), "Finish Chopping")){
				animator.SetTrigger("ChopFinishTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.Axe;
			}
		}
		if(charState == CharacterState.PickAxing && !isMoving){
			isPaused = true;
			if(GUI.Button(new Rect(25, 25, 150, 30), "Swing Vertical")){
				animator.SetTrigger("ChopVerticalTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Swing Horizontal")){
				animator.SetTrigger("ChopHorizontalTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 105, 150, 30), "Swing Ground")){
				animator.SetTrigger("ChopGroundTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 145, 150, 30), "Swing Ceiling")){
				animator.SetTrigger("ChopCeilingTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 185, 150, 30), "Swing Diagonal")){
				animator.SetTrigger("ChopDiagonalTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 225, 150, 30), "Finish PickAxing")){
				animator.SetTrigger("ChopFinishTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.PickAxe;
			}
		}
		if(charState == CharacterState.Shovel && !isMoving){
			isPaused = false;
			if(GUI.Button(new Rect(25, 25, 150, 30), "Start Digging")){
				animator.SetTrigger("DiggingStartTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.Digging;
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Put Shovel Away")){
				animator.SetTrigger("ItemBackAwayTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 105, 150, 30), "Give Shovel")){
				animator.SetTrigger("ItemHandoffTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 145, 150, 30), "Put Shovel Down")){
				animator.SetTrigger("ItemPutdownTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.7f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 185, 150, 30), "Drop Shovel")){
				animator.SetTrigger("ItemDropTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.4f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Rake && !isMoving){
			isPaused = false;
			if(GUI.Button(new Rect(25, 25, 150, 30), "Start Raking")){
				animator.SetTrigger("DiggingStartTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.Raking;
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Put Rake Away")){
				animator.SetTrigger("ItemBackAwayTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 105, 150, 30), "Give Rake")){
				animator.SetTrigger("ItemHandoffTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 145, 150, 30), "Put Rake Down")){
				animator.SetTrigger("ItemPutdownTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.7f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 185, 150, 30), "Drop Rake")){
				animator.SetTrigger("ItemDropTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.4f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Raking && !isMoving){
			isPaused = true;
			if(GUI.Button(new Rect(25, 25, 150, 30), "Rake")){
				animator.SetTrigger("ItemRakeUse");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Finish Raking")){
				animator.SetTrigger("DiggingFinishTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.Rake;
			}
		}
		if(charState == CharacterState.Digging && !isMoving){
			isPaused = true;
			if(GUI.Button(new Rect(25, 25, 150, 30), "Dig")){
				animator.SetTrigger("DiggingScoopTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Finish Digging")){
				animator.SetTrigger("DiggingFinishTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.Shovel;
			}
		}
		if(charState == CharacterState.FishingPole && !isMoving){
			isPaused = false; 
			if(GUI.Button(new Rect(25, 25, 150, 30), "Cast Reel")){
				animator.SetTrigger("FishingCastTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.Fishing;
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Put Fishing Pole Away")){
				animator.SetTrigger("ItemBackAwayTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 105, 150, 30), "Give Fishing Pole")){
				animator.SetTrigger("ItemHandoffTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 145, 150, 30), "Put Fishing Pole Down")){
				animator.SetTrigger("ItemPutdownTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.7f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 185, 150, 30), "Drop FishingPole")){
				animator.SetTrigger("ItemDropTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.4f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Sawing && !isMoving){
			isPaused = true;
			if(GUI.Button(new Rect(25, 25, 150, 30), "Finish Sawing")){
				animator.SetTrigger("SawFinishTrigger");
				StartCoroutine(COMovePause(1.2f));
				charState = CharacterState.Saw;
			}
		}
		if(charState == CharacterState.Sit && !isMoving){
			isPaused = true;
			if(GUI.Button(new Rect(25, 25, 150, 30), "Talk1")){
				animator.SetTrigger("ChairTalk1Trigger");
			}

			if(GUI.Button(new Rect(25, 65, 150, 30), "Eat")){
				animator.SetTrigger("ChairEatTrigger");
				StartCoroutine(COShowItem("chaireat", 0.2f));
				StartCoroutine(COShowItem("chair", 1.1f));
			}

			if(GUI.Button(new Rect(25, 105, 150, 30), "Drink")){
				animator.SetTrigger("ChairDrinkTrigger");
				StartCoroutine(COShowItem("chairdrink", 0.2f));
				StartCoroutine(COShowItem("chair", 1.1f));
			}
			
			if(GUI.Button(new Rect(25, 145, 150, 30), "Stand")){
				animator.SetTrigger("ChairStandTrigger");
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Fishing && !isMoving){
			isPaused = true;
			if(GUI.Button(new Rect(25, 25, 150, 30), "Reel In")){
				animator.SetTrigger("FishingReelTrigger");
				StartCoroutine(COMovePause(1.2f));
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Finish Fishing")){
				animator.SetTrigger("ItemBackAwayTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Box && !isMoving){
			if(GUI.Button(new Rect(25, 25, 150, 30), "Put Down Box")){
				animator.SetTrigger("CarryPutdownTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.7f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 65, 150, 30), "Throw Box")){
				animator.SetTrigger("CarryThrowTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
			if(GUI.Button(new Rect(25, 104, 150, 30), "Give Box")){
				animator.SetTrigger("CarryHandoffTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.6f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Lumber && !isMoving){
			if(GUI.Button(new Rect (25, 25, 150, 30), "Put Down Lumber")){
				animator.SetTrigger("CarryPutdownTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 1f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Sphere && !isMoving){
			if(GUI.Button(new Rect (25, 25, 150, 30), "Throw Sphere")){
				animator.SetTrigger("CarryOverheadThrowTrigger");
				StartCoroutine(COMovePause(1.2f));
				StartCoroutine(COShowItem("none", 0.5f));
				charState = CharacterState.Idle;
			}
		}
		if(charState == CharacterState.Climb){
			isPaused = true;
			if(GUI.Button(new Rect (25, 25, 150, 30), "Climb Off Bottom")){
				animator.SetTrigger("ClimbOffBottomTrigger");
				StartCoroutine(COShowItem("none", 0.9f));
				StartCoroutine(COChangeCharacterState(0.9f, CharacterState.Idle));
			}
			if(GUI.Button(new Rect (25, 65, 150, 30), "Climb Up")){
				animator.SetTrigger("ClimbUpTrigger");
			}
			if(GUI.Button(new Rect (25, 105, 150, 30), "Climb Down")){
				animator.SetTrigger("ClimbDownTrigger");
			}
			if(GUI.Button(new Rect (25, 145, 150, 30), "Climb Off Top")){
				Vector3 posPivot = animator.pivotPosition;
				Debug.Log("Pivot POS " + posPivot);
				animator.SetTrigger("ClimbOffTopTrigger");
				StartCoroutine(COShowItem("none", 2f));
				StartCoroutine(COChangeCharacterState(2f, CharacterState.Idle));
				animator.stabilizeFeet = true;
			}
		}
		if(charState == CharacterState.PushPull){
			isPaused = true;
			if(GUI.Button(new Rect (25, 25, 150, 30), "Release")){
				animator.SetTrigger("PushPullReleaseTrigger");
				StartCoroutine(COShowItem("none", 0.5f));
				StartCoroutine(COChangeCharacterState(0.5f, CharacterState.Idle));
			}
		}
	}

	public IEnumerator COMovePause(float pauseTime){
		isPaused = true;
		yield return new WaitForSeconds(pauseTime);
		isPaused = false;
	}

	public IEnumerator COChangeCharacterState(float waitTime, CharacterState state){
		yield return new WaitForSeconds(waitTime);
		charState = state;
	}
	
	public IEnumerator COShowItem(string item, float waittime){
		yield return new WaitForSeconds (waittime);
		if(item == "none"){
			axe.SetActive(false);
			hammer.SetActive(false);
			fishingpole.SetActive(false);
			shovel.SetActive(false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "axe"){
			axe.SetActive(true);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "hammer"){
			axe.SetActive(false);
			hammer.SetActive (true);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "fishingpole"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (true);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "shovel"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (true);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "box"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(true);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "food"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(true);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "drink"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(true);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "saw"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(true);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "pickaxe"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(true);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "sickle"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(true);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "rake"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(true);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "chair"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(true);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "chaireat"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(true);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(true);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "chairdrink"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(true);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(true);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "ladder"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(true);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "pushpull"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(true);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "lumber"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(true);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "sphere"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(true);
			cart.SetActive(false);
			paintbrush.SetActive(false);
		}
		else if(item == "cart"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(true);
			paintbrush.SetActive(false);
		}
		else if(item == "paintbrush"){
			axe.SetActive(false);
			hammer.SetActive (false);
			fishingpole.SetActive (false);
			shovel.SetActive (false);
			box.SetActive(false);
			food.SetActive(false);
			drink.SetActive(false);
			saw.SetActive(false);
			pickaxe.SetActive(false);
			sickle.SetActive(false);
			rake.SetActive(false);
			chair.SetActive(false);
			ladder.SetActive(false);
			pushpull.SetActive(false);
			lumber.SetActive(false);
			sphere.SetActive(false);
			cart.SetActive(false);
			paintbrush.SetActive(true);
		}
		yield return null;
	}
}