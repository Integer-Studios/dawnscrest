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
		private static int playerId;

		/*
		 * 
		 * Public
		 * 
		 */

		public static void start (int sPort, string sAddress) {
			attemptConnection (sAddress, sPort);
			playerId = GameObject.FindObjectOfType<PolyNetManager> ().playerId;
			isActive = true;
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
				Debug.Log ("Connected to Server");
				clientSocket.EndAccept(result);
				socket = new PolySocket(clientSocket, handleMessage, onDisconnect);
				socket.start();
				PacketHandler.sendPacket(new PacketLogin(playerId, ""), null);
			} catch (Exception e) {
				Debug.LogError (e.Message);
			}
		}
	}

}