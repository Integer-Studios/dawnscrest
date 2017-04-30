using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;
using System.IO;

namespace PolyItem {

	public class Gatherable : Workable, ISaveable{
		
		public Item[] drops;
		public int repeats = 3;
		public int refills = 5;
		public float refillTime = 60;
		public GameObject replacement;
		public int curRepeats = 0;
		public int curRefills = 0;


		private float runoutTime = -1f;

		/*
		* 
		* Public Interface
		* 
		*/

		public override bool isInteractable(Interactor i) {
			return !isOut() && base.isInteractable(i);
		}

		// networking 

		public override void writeBehaviourSpawnData(ref BinaryWriter writer) {
			base.writeBehaviourSpawnData (ref writer);
			writer.Write (curRepeats);
			writer.Write (curRefills);
		}

		public override void readBehaviourSpawnData(ref BinaryReader reader) {
			base.readBehaviourSpawnData (ref reader);
			curRepeats = reader.ReadInt32 ();
			curRefills = reader.ReadInt32 ();
		}

		public override void handleBehaviourPacket (PacketBehaviour p) {
			base.handleBehaviourPacket (p);
			if (p.id == 16) {
				PacketSyncInt o = (PacketSyncInt)p;
				if (o.syncId == 0)
					curRepeats = o.value;
				else if (o.syncId == 1)
					curRefills = o.value;
			}
		}

		/*
		* 
		* Private
		* 
		*/

		protected override void Start() {
			if (replacement != null)
				PolyNetWorld.registerPrefab(replacement);
			base.Start ();
		}

		protected IEnumerator refillUpdate() {
			while (isOut ()) {
				if (runoutTime + refillTime <= Time.time) {
					curRepeats = 0;
					identity.sendBehaviourPacket (new PacketSyncInt (this, 0, curRepeats));
				}
				yield return new WaitForSeconds (5f);
			}
		}

//		protected void Update() {
//			if (!PolyServer.isActive)
//				return;
//			if (isOut ()) {
//				if (runoutTime + refillTime <= Time.time) {
//					curRepeats = 0;
//					identity.sendBehaviourPacket (new PacketSyncInt (this, 0, curRepeats));
//				}
//			}
//		}

		protected override void onComplete(Interactor i) {
			if (isOut ())
				return;
			for (int j = 0; j < drops.GetLength (0); j++) {
				GameObject g = Instantiate (drops [j].gameObject);
				g.GetComponent<Rigidbody> ().velocity = i.interactor_getInteractionNormal () + new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f), Random.Range(-0.5f,0.5f));
				g.transform.position = i.interactor_getInteractionPosition() + new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f), Random.Range(-0.5f,0.5f));
				PolyNetWorld.spawnObject (g);
			}
			if (replacement != null) {
				GameObject g = Instantiate(replacement);
				g.transform.position = transform.position;
				g.transform.localScale = transform.localScale;
				g.transform.rotation = transform.rotation;
				PolyNetWorld.spawnObject (g);
				PolyNetWorld.destroy (gameObject);
			} else {
				curRepeats++;
				identity.sendBehaviourPacket (new PacketSyncInt (this, 0, curRepeats));
				if (curRepeats == repeats) {

					curRefills++;
					if (curRefills == refills)
						Destroy (this);
					else {
						identity.sendBehaviourPacket (new PacketSyncInt (this, 1, curRefills));
						StartCoroutine (refillUpdate ());
					}

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