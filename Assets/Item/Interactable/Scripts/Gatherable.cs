using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PolyItem {

	public class Gatherable : Workable, ISaveable{
		
		public Item[] drops;
		public int repeats = 3;
		public int refills = 5;
		public float refillTime = 60;
		public GameObject replacement;

		[SyncVar]
		private int curRepeats = 0;
		[SyncVar]
		private int curRefills = 0;
		private float runoutTime = -1f;

		/*
		* 
		* Public Interface
		* 
		*/

		public override bool isInteractable(Interactor i) {
			return !isOut() && base.isInteractable(i);
		}

		/*
		* 
		* Private
		* 
		*/

		protected override void Start() {
			if (replacement != null)
				ClientScene.RegisterPrefab(replacement);
			base.Start ();
		}	

		protected void Update() {
			base.Update ();
			if (isOut ()) {
				if (runoutTime + refillTime <= Time.time) {
					curRepeats = 0;
				}
			}
		}

		protected override void onComplete(Interactor i) {
			for (int j = 0; j < drops.GetLength (0); j++) {
				GameObject g = Instantiate (drops [j].gameObject);
				g.transform.position = i.interactor_getInteractionPosition() + new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f), Random.Range(-0.5f,0.5f));
				g.GetComponent<Rigidbody> ().velocity = i.interactor_getInteractionNormal () + new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f), Random.Range(-0.5f,0.5f));
				NetworkServer.Spawn (g);
			}
			if (replacement != null) {
				GameObject g = Instantiate(replacement);
				g.transform.position = transform.position;
				g.transform.localScale = transform.localScale;
				g.transform.rotation = transform.rotation;
				NetworkServer.Spawn (g);
				Destroy (gameObject);
			} else {
				curRepeats++;
				if (curRepeats == repeats) {

					curRefills++;
					if (curRefills == refills)
						Destroy (this);

					runoutTime = Time.time;
				}
			}
			base.onComplete (i);
		}

		private bool isOut() {
			return curRefills == refills || curRepeats == repeats;
		}

		public JSONObject write () {
			JSONObject obj = new JSONObject (JSONObject.Type.OBJECT);
			obj.AddField ("strength", strength);
			obj.AddField ("repeats", curRepeats);
			obj.AddField ("refills", curRefills);
			obj.AddField ("runout", -1f);
			return obj;
		}

		public void read (JSONObject obj) {
			strength = obj.GetField ("strength").n;
			curRepeats = (int) obj.GetField ("repeats").n;
			curRefills = (int) obj.GetField ("refills").n;
			runoutTime = obj.GetField ("runout").n;
		}

		public string getType() {
			return "gatherable";
		}

	}

}