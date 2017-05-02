using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Threading;
using UnityEngine;

namespace PolyNet {

	public class PolySocket {

		public delegate void MessageHandler(byte[] b);
		public delegate void DisconnectHandler();

		private Socket socket;
		private byte[] buffer;
		private int bufferSize;
		private byte[] toSend;
		private MessageHandler handler;
		private DisconnectHandler onDisconnect;
		private Queue<byte[]> messages;
		private bool busy = false;
		private bool isActive = false;

		/*
		 * 
		 * Public Interface
		 * 
		 */

		public PolySocket(Socket s, MessageHandler h, DisconnectHandler d) {
			socket = s;
			handler = h;
			onDisconnect = d;
			messages = new Queue<byte[]> ();
			buffer = new byte[4];
		}

		public void start() {
			isActive = true;
			socket.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback (onReceiveBufferSize), null);
		}

		public void stop () {
			isActive = false;
			socket.Close ();
			onDisconnect ();
		}

		public void queueMessage(byte[] b) {
			if (!busy) {
				busy = true;
				send (b);
			} else {
				busy = true;
				lock (messages) {
					messages.Enqueue (b);
				}
			}
		}

		/*
		 * 
		 * Private
		 * 
		 */

		// Abstract

		private void handleData() {
			byte[] data = new byte[buffer.Length];
			Array.Copy (buffer, data, buffer.Length);
			handler(data);
		}

		private void onSendComplete() {
			Thread.Sleep (10);
			if (messages.Count > 0) {
				lock (messages) {
					send (messages.Dequeue ());
				}
			} else {
				busy = false;
			}
		}

		// Low Level

		private void send(byte[] b) {
			if (!isActive)
				return;
			try {
				
				toSend = b;
				byte[] sizeBuf = BitConverter.GetBytes(toSend.Length);
				socket.BeginSend (sizeBuf, 0, 4, SocketFlags.None, new AsyncCallback (onBufferSizeSent), null);
			} catch (SocketException e) {
				if (isActive)
					stop ();
			} catch (Exception e) {
					Debug.LogError (e.Message);
			}
		}

		private void setBufferSize(int i) {
			bufferSize = i;
			ressetBufferSize ();
		}

		private void ressetBufferSize() {
			buffer = new byte[bufferSize];
		}


		// Low Level Callbacks

		private void onReceiveData(IAsyncResult result) {
			if (!isActive)
				return;
			try {
				if (buffer.Length == 0) {
					socket.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback (onReceiveData), null);
					ressetBufferSize();
				} else {
					socket.EndReceive(result);
					handleData();
					setBufferSize(4);
					socket.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback (onReceiveBufferSize), null);
				}
			} catch (SocketException e) {
				if (isActive)
					stop ();
			} catch (Exception e) {
					Debug.LogError (e.Message);
			}
		}

		private void onReceiveBufferSize(IAsyncResult result) {
			
			if (!isActive)
				return;
			try {
				if (buffer.Length == 0) {
					ressetBufferSize();
					socket.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback (onReceiveBufferSize), null);
				} else {
					socket.EndReceive(result);
					setBufferSize(BitConverter.ToInt32(buffer, 0));
					socket.BeginReceive (buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback (onReceiveData), null);
				}
			} catch (SocketException e) {
				if (isActive)
					stop ();
			}
			catch (Exception e) {
					Debug.LogError (e.Message);
			}
		}

		private void onBufferSizeSent(IAsyncResult result) {
			if (!isActive)
				return;
			try {
				socket.EndSend(result);
				socket.BeginSend (toSend, 0, toSend.Length, SocketFlags.None, new AsyncCallback (onDataSent), null);
			} catch (SocketException e) {
				if (isActive)
					stop ();
			} catch (Exception e) {
					Debug.LogError (e.Message);
			}
		}

		private void onDataSent(IAsyncResult result) {
			if (!isActive)
				return;
			try {
				socket.EndSend(result);
				onSendComplete();
			} catch (SocketException e) {
				if (isActive)
					stop ();
			} catch (Exception e) {
					Debug.LogError (e.Message);
			}
		}
	}

}