using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PolyPlayer {

	public class GUIManager : MonoBehaviour {

		public GameObject crosshairObj;
		public GameObject blackoutObj;
		public GUIBoundInventory hotbar;
		public GameObject mouseFollowerObj;
		public Slider healthSlider;
		public Slider hungerSlider;
		public Slider thirstSlider;
		public Chat chatObj;
		public ScreenCharacter characterScreenObj;
		public ScreenRecipes recipesScreenObj;
		public ScreenCrafting craftingScreenObj;
		public ScreenInventory inventoryScreenObj;
		public ScreenSettings settingsScreenObj;

		public static GameObject crosshair;
		public static GameObject blackout;
		public static GUIBoundInventory hotbarGUI;
		public static GameObject mouseFollower;
		public static Chat chat;
		public static ScreenCharacter characterScreen;
		public static ScreenRecipes recipesScreen;
		public static ScreenCrafting craftingScreen;
		public static ScreenInventory inventoryScreen;
		public static ScreenSettings settingsScreen;
		public static Player player;

		private static List<Screen> screenStack;
		private static bool uiOpen;
		private static bool chatOpen;

		/*
		* 
		* Public Interface
		* 
		*/

		public static void setPlayer(Player p) {
			player = p;
			player.bindInventoryToGUI (0, hotbarGUI);
		}

		public static void setChatOpen(bool c) {
			chatOpen = c;
			chat.setInputEnabled (c);
		}

		public static bool processInput() {
			if (!uiOpen && !chatOpen)
				return false;

			if (chatOpen)
				return true;

			screenStack [0].processInput ();
			return true;
		}

		public static void pushScreen(Screen screen) {
			if (!uiOpen)
				openGUI ();
			if (screenStack.Count > 0) {
				screenStack[0].gameObject.SetActive (false);
			}
			screenStack.Insert (0, screen);
			screenStack[0].gameObject.SetActive (true);
			screenStack [0].onPush ();
		}

		public static void popScreen() {
			screenStack [0].onPop ();
			screenStack[0].gameObject.SetActive (false);
			screenStack.RemoveAt (0);
			if (screenStack.Count > 0) {
				screenStack [0].gameObject.SetActive (true);
				screenStack [0].onRePush ();
			} else {
				closeGUI ();
			}
		}


		public static void closeGUI() {
			for (int i = 0; i < screenStack.Count; i++) {
				screenStack [i].onClose ();
				screenStack [i].gameObject.SetActive (false);
			}
			screenStack.Clear ();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			blackout.SetActive (false);
			crosshair.SetActive (true);
			uiOpen = false;
		}

		public static void openGUI() {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			blackout.SetActive (true);
			crosshair.SetActive (false);
			uiOpen = true;
		}

		public static bool isGUIOpen() {
			return uiOpen;
		}

		/*
		* 
		* Private
		* 
		*/

		private void Start() {
			crosshair = crosshairObj;
			blackout = blackoutObj;
			mouseFollower = mouseFollowerObj;
			mouseFollower.SetActive (false);
			hotbarGUI = hotbar;
			chat = chatObj;
			characterScreen = characterScreenObj;
			recipesScreen = recipesScreenObj;
			craftingScreen = craftingScreenObj;
			inventoryScreen = inventoryScreenObj;
			settingsScreen = settingsScreenObj;
			screenStack = new List<Screen> ();
		}

		private void Update() {
			if (player != null) {
				player.syncVital (0, healthSlider);
				player.syncVital (1, hungerSlider);
				player.syncVital (2, thirstSlider);
			}
		}

	}

}