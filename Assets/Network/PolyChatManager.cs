using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PolyNetwork {

	public class PolyChatManager : PolyMessageHandler {

		public static void handleSend(int senderID, string message) {
			bool server = false;
			PolyClient sender = PolyDataManager.getPlayer (senderID);
			if (sender.loginID == -1)
				server = true;
			bool adminCase = (sender.admin || !PolyNetworkManager.getManager ().live || server);
			if ((message.StartsWith ("/"))|| server) {
				//is command
				if (!server)
					message = message.Substring(1);
				string[] args = message.Split (' ');
				string command = args [0];
				var argsList = new List<string>(args);
				argsList.RemoveAt(0);
				args = argsList.ToArray ();
				string fullCommand = string.Join (" ", args);
				switch (command) {
					case "save": 
						if (!adminCase)
							break;
						sendMessage (sender, "Server", "Received command " + message);
						switch (args [0]) {
							case "players":
								PolyDataManager.savePlayers ();
								break;
							case "world":
								PolyDataManager.save ();
								break;
							default:
								PolyDataManager.save ();
								PolyDataManager.savePlayers ();
								break;
						}
						break;
					case "say":
						if (!adminCase)
							break;
						broadcastGlobal (fullCommand);
						break;
					case "tp":
						if (!adminCase)
							break;
						handleTeleport (args, server, sender);
						break;
					case "w":
						broadcastLocal (sender, fullCommand, 6F);
						break;
					default: 
						if (!adminCase)
							break;
						sendMessage (sender, "Server", "Received command " + message);
						break;
				}

			} else {
				float range = calculateRange (message);
				
				broadcastLocal (sender, message, range);
			}
		}

		public static void handleTeleport(string[] args, bool server, PolyClient sender) {
			PolyClient teleportee;
			if (server) {
				teleportee = PolyDataManager.getPlayerForUsername (args [0]);
				var argsList = new List<string> (args);
				argsList.RemoveAt (0);
				args = argsList.ToArray ();
			} else
				teleportee = sender;
			if (args.Length == 3) {
				//vector
				Vector3 pos = new Vector3 (float.Parse (args [0]), float.Parse (args [1]), float.Parse (args [2]));
				teleportee.gameObject.transform.position = pos;
			} else if (args.Length == 1 && args [0] != "spawn") {
				//player
				PolyClient target = PolyDataManager.getPlayerForUsername (args [0]);
				if (target.loginID != -1) {
					Vector3 pos = target.gameObject.transform.position;
					pos = pos + target.gameObject.transform.forward;
					pos.y += 1;
					teleportee.gameObject.transform.position = pos;
				} else
					sendMessage (sender, "Server", "Could not find player: " + args [0]);

			} else if (args [0] == "spawn") {
				Vector3 pos = PolyNetworkManager.getManager ().spawn.transform.position;
				teleportee.gameObject.transform.position = pos;
			}

		}

		public static void broadcastLocal(PolyClient sender, string message, float range) {
			Collider[] colliders = Physics.OverlapSphere (sender.gameObject.transform.position, range);
			foreach (Collider c in colliders) {
				if (c.gameObject.tag == "Player") {
					float distance = Vector3.Distance (c.gameObject.transform.position, sender.gameObject.transform.position);
					distance = distance / range;
					int receiverID = c.GetComponent<PlayerSaveable> ().id;
					PolyClient receiver = PolyDataManager.getPlayer (receiverID);

					if (receiverID != -1 && receiver.loginID != -1) {

						if (receiver.loginID == sender.loginID || distance < 0.3f || range == 6F)
							distance = 0.0f;
						if (distance > 0.85f) {
							distance = 0.85f;
						}
						sendMessage (receiver, sender.identifier, message, distance);
					} else {
						Debug.Log ("Could not send message to player: " + receiverID);
					}
				}
			}
		}

		public static void broadcastGlobal(string message) {
			List<PolyClient> players = PolyDataManager.getActivePlayers ();
			foreach (PolyClient p in players) {
				sendMessage (p, null, message);
			}
		}

		public static void sendMessage(PolyClient receiver, string sender, string message, float distance = 0) {
			if (receiver.loginID == -1)
				return;
			
			PolyNetworkManager.sendNetMessage (receiver.connection, PlayerChat.MSG_TYPE, new PlayerChat () {
				senderIdentifier = sender,
				message = message,
				distance = distance
			});			
		}

		public void registerClientHandlers() {
			NetworkManager.singleton.client.connection.RegisterHandler (PlayerChat.MSG_TYPE, OnReceiveChat);
		}

		public void unregisterClientHandlers() {
			NetworkManager.singleton.client.connection.UnregisterHandler (PlayerChat.MSG_TYPE);
		}

		public void registerServerHandlers() {

		}

		public void unregisterServerHandlers() {

		}

		private static float calculateRange(string message) {
			int countUpper = 0;
			int countLower = 0;
			int countOther = 0;
			for (int i = 0; i < message.Length;i++ ) {
				if (char.IsUpper(message[i])) countUpper++;
				else if (char.IsLower(message[i])) countLower++;
				else countOther++;
			}
			int volume = 2;
			if (countLower + countUpper == 0)
				volume = 2;
			if (countUpper / (countLower + countUpper) > 0.7f) {
				//yelling
				volume = 4;
			} else if (countUpper != 0) {
				volume = 2;
			} else {
//				all lowercase
//				volume = 1; 1 = range 6, whisper
			}

			float range = 16F;
			if (volume == 1)
				range = 6F;
			else
				range = range * volume;
			return range;
		}
			
		public class PlayerChat : MessageBase {
			public static short MSG_TYPE = 3000;
			public string senderIdentifier = "";
			public string message = "";
			public float distance = 0.0f;
		}

		private void OnReceiveChat(NetworkMessage netMsg) {
			PlayerChat msg = netMsg.ReadMessage<PlayerChat>();
			if (PolyNetworkManager.getLocalPlayer() != null)
				PolyNetworkManager.getLocalPlayer().receiveChat (msg.senderIdentifier, msg.message, msg.distance);
		}

	}

}