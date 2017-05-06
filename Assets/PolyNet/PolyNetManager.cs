using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PolyNetManager : MonoBehaviour {

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

		[HideInInspector]
		public int worldID = -1;

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

			if (isClient)
				PolyClient.start (serverPort, externalAddress);
			else {
				PolyNodeHandler.initialize (this);
				PolyServer.start (serverPort, serverAddress);
			}
		}

		void Start() {
			PolyNetWorld.initialize (this);
		}

		void Update() {
			PacketHandler.update ();
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