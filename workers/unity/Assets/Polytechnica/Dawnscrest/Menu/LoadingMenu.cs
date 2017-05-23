using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Polytechnica.Dawnscrest.Menu {

	public class LoadingMenu : Menu {

		public Text stageText;
		public AsyncOperation async = null; // When assigned, load is in progress.
		private float sceneProgress = 0f;

		[HideInInspector]
		public static int stage = 0;

		private float barDisplay; //current progress
		private Vector2 pos = new Vector2(100,400);
		private Vector2 size = new Vector2(600,40);
		public Texture2D emptyTex;
		public Texture2D fullTex;

		public override void Start() {
			base.Start ();

		}

		void OnGUI() {
			
			if (async != null) {

				if (async.progress != 1 && sceneProgress != async.progress) {
					sceneProgress = async.progress;
				
					Debug.Log (async.progress * 100 + "%");
				} else if (async.progress == 1 &&  sceneProgress != async.progress) {
					sceneProgress = async.progress;
					LoadingMenu.stage = 2;

				}
			}

//			UnityEngine.GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
//			UnityEngine.GUI.Box(new Rect(0,0, size.x, size.y), emptyTex);
//
//			//draw the filled-in part:
//			UnityEngine.GUI.BeginGroup(new Rect(0,0, size.x * barDisplay, size.y));
//			UnityEngine.GUI.Box(new Rect(0,0, size.x, size.y), fullTex);
//			UnityEngine.GUI.EndGroup();
//			UnityEngine.GUI.EndGroup();

			switch (stage) {
			case 0:
				stageText.text = "beginning load...";
				break;
			case 1:
				stageText.text = "initializing scene...";
				break;
			case 2:
				stageText.text = "connecting to spatial...";
				break;
			case 3:
				stageText.text = "creating family...";
				break;
			case 4:
				stageText.text = "embodying character...";
				break;
				
			}
//			barDisplay = (stage / 4);

		}

		public override void Initialize() {
		}

		public override void Update() {
			base.Update ();

		}
			
		protected override void OnShow() {
			LoadingMenu.stage = 1;
			StartCoroutine(manager.StartGame (this));
		}

	}

}