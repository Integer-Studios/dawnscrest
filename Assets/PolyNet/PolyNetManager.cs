using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PolyNet {

	public class PolyNetManager : MonoBehaviour {

		public string serverAddress;
		public int serverPort;
		public int clientPort;
		public bool isClient;
		public int playerId;
		public float chunkSize;
		public int chunkLoadRadius;
		public int port = 0;
		public PolyNetIdentity playerPrefab;

		// Use this for initialization
		void Awake () {

			switch (port) {
			case 0:
				serverPort = 8888;
				clientPort = 8889;
				break;
			case 1:
				serverPort = 8890;
				clientPort = 8891;
				break;
			case 2:
				serverPort = 8892;
				clientPort = 8893;
				break;
			}

			if (isClient)
				PolyClient.start (clientPort, serverPort, serverAddress);
			else {
				PolyNodeHandler.initialize (this);
				PolyServer.start (serverPort);
			}

			StartCoroutine (socketListenerUpdate ());
			StartCoroutine (packetWriterUpdate ());
		}

		void Start() {
			PolyNetWorld.initialize (this);
		}

		private IEnumerator socketListenerUpdate() {
			while (true) {
				if (isClient)
					PolyClient.update ();
				else
					PolyServer.update ();
				yield return new WaitForSeconds(0f);
			}
		}

		private IEnumerator packetWriterUpdate() {
			while (true) {
				PacketHandler.update ();
				yield return new WaitForSeconds(0f);
			}
		}

	}


}