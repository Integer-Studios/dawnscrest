using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace PolyNet {

	public class PolyClient {

		public static bool isActive = false;

		private static Socket clientSocket;
		private static PolySocket socket;
		public static int playerId;

		private static PolyNetManager.StartSequenceDelegate onConnectDelegate;
		private static int startSequenceId;
		private static PolyNetManager manager;
		/*
		 * 
		 * Public
		 * 
		 */

		public static void start (PolyNetManager man, PolyNetManager.StartSequenceDelegate del, int ssid, int sPort, string cAddress) {
			manager = man;
			onConnectDelegate = del;
			startSequenceId = ssid;
			attemptConnection (cAddress, sPort);
			playerId = GameObject.FindObjectOfType<PolyNetManager> ().playerId;
		}

		public static void stop () {
			socket.stop ();
		}

		// thread safe, queued
		public static void sendMessage (byte[] b) {
			socket.queueMessage (b);
		}

		/*
		 * 
		 * Private
		 * 
		 */

		// not main thread
		private static void handleMessage(byte[] b) {
			PacketHandler.receivePacket (b, null);
		}

		private static void onDisconnect() {
			Debug.Log ("Disconnected from server");
		}

		/*
		 * 
		 * Sockets
		 * 
		 */

		private static void attemptConnection(string ip, int port) {
			try {
				clientSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				clientSocket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), new AsyncCallback(onConnect), null);
			} catch (Exception e) {
				Debug.LogError (e.Message);
			}
		}

		private static void onConnect(IAsyncResult result) {
			try {
				clientSocket.EndAccept(result);
				socket = new PolySocket(clientSocket, handleMessage, onDisconnect);
				socket.start();
				Debug.Log ("Startup[" + startSequenceId + "]: Connected to Server");
				onConnectDelegate(startSequenceId);
			} catch (Exception e) {
				Debug.LogError (e.Message);
			}
		}
	}

}