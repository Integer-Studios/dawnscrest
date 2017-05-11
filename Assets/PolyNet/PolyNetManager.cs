using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PolyNetManager : MonoBehaviour {

		public delegate void StartSequenceDelegate(int stage);

		public string serverAddress;
		public string externalAddress;
		public int serverPort;
		public bool isClient;
		public int playerId;
		public float chunkSize;
		public int chunkLoadRadius;
		public int port = 0;
		public string rawFile;
		public PolyNetIdentity playerPrefab;
		public int worldID = -1;

		public bool limit = false;

		[HideInInspector]
		public int xMax = 10;
		[HideInInspector]
		public int xMin = 0;
		[HideInInspector]
		public int zMax = 10;
		[HideInInspector]
		public int zMin = 0;

		private bool clientThreadFixer = false;
		private int clientThreadFixStage = -1;

		// Use this for initialization
		void Awake () {

			switch (port) {
			case 0:
				serverPort = 8888;
				break;
			case 1:
				serverPort = 8890;
				break;
			case 2:
				serverPort = 8892;
				break;
			}

			if (isClient) {
				PolyClient.isActive = true;
				clientStartSequence (-1);
			} else {
				PolyServer.isActive = true;
				serverStartSequence (-1);
			}
		}

		private void serverStartSequence(int stage) {
			stage++;
			switch (stage) {
			case 0:
				PolyNodeHandler.initialize (this, serverStartSequence, stage);
				break;
			case 1:
				PolySaveManager.initialize (this, serverStartSequence, stage);
				break;
			case 2:
				PolyServer.initialize (this, serverStartSequence, stage);
				break;
			case 3:
				PolyNetWorld.initialize (this, serverStartSequence, stage);
				break;
			default:
				Debug.Log ("Realms Server Started Successfully!");
				break;
			}
		}

		private void clientStartSequence(int stage) {
			stage++;
			switch (stage) {
			case 0:
				PolyClient.start (this, clientStartSequence, stage, serverPort, externalAddress);
				break;
			case 1:
				// get back on main thread
				clientThreadFixer = true;
				clientThreadFixStage = stage;
				break;
			case 2:
				PolyNetWorld.initialize (this, clientStartSequence, stage);
				break;
			case 3:
				PacketHandler.sendPacket (new PacketLogin (playerId, ""), null);
				clientStartSequence (stage);
				break;
			default:
				Debug.Log ("Realms Client Started Successfully!");
				break;
			}
		}

		private IEnumerator clientStartSequenceThreadFix(int stage) {
			yield return null;
			clientStartSequence (stage);
		}

		int pollCount = 0;

		void Update() {
			PacketHandler.update ();
			PolyNetWorld.update ();
			pollCount++;
			if (pollCount == 10 && PolyServer.isActive) {
				pollCount = 0;
				PolyServer.poll ();
			}
			if (clientThreadFixer) {
				clientThreadFixer = false;
				clientStartSequence (clientThreadFixStage);
			}
		}

		void OnApplicationQuit() {
			stop ();
		}

		private void stop() {
			if (isClient)
				PolyClient.stop ();
			else 
				PolyServer.stop ();
		}

	}

}