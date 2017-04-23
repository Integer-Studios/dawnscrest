using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PolyNet {

	public class PolyClient {

		public static bool isActive = false;

		private static int port;
		private static int reliableChannelId, socketId, connectionId;

		public static void start (int cPort, int sPort, string sAddress) {
			port = cPort;
			NetworkTransport.Init();
			ConnectionConfig config = new ConnectionConfig();
			reliableChannelId  = config.AddChannel(QosType.Reliable);
			HostTopology topology = new HostTopology(config, 1);
			socketId = NetworkTransport.AddHost(topology, port);
			Debug.Log ("PolyNet Client Started on Port: "+ port +", socketId: " + socketId);
			byte error; connectionId = NetworkTransport.Connect(socketId, sAddress, sPort, 0, out error);
			isActive = true;
		}

		public static void update() {
			int recHostId, recConnectionId, recChannelId;
			byte[] recBuffer = new byte[1024];
			int bufferSize = 1024;
			int dataSize;
			byte error;
			NetworkEventType recNetworkEvent = NetworkTransport.Receive (out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);
			switch (recNetworkEvent) {
			case NetworkEventType.Nothing:
				break;
			case NetworkEventType.ConnectEvent:
				if (recConnectionId == connectionId)
					onConnect ();
				break;
			case NetworkEventType.DataEvent:
				onRecieveMessage (recBuffer);
				break;
			case NetworkEventType.DisconnectEvent:
				onDisconnect ();
				break;
			}
		}

		public static void sendMessage(byte[] buffer) {
			byte error; NetworkTransport.Send(socketId, connectionId, reliableChannelId, buffer, buffer.GetLength(0), out error);
		}

		private static void onRecieveMessage(byte[] buffer) {
			PacketHandler.handlePacket (buffer, null);
		}

		private static void onConnect() {
			Debug.Log ("Connected to Server");
			PacketHandler.queuePacket (new PacketLogin (GameObject.FindObjectOfType<PolyNetManager>().playerId), null);
		}

		private static void onDisconnect() {
			Debug.Log ("Client Disconnected");
		}
	}

}