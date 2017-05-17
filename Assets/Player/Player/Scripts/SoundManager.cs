using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;
using PolyItem;

namespace PolyPlayer {
	public class SoundManager : PolyNetBehaviour {

		public AudioClip itemPickupSound;
		public AudioClip hurtSound;
		public AudioClip burpSound;	

		private AudioSource source;
		private Dictionary<PlayerSound, int> playerSoundsEncode = new Dictionary<PlayerSound, int>();
		private Dictionary<int, PlayerSound> playerSoundsDecode = new Dictionary<int, PlayerSound>();

		public void rpcPlaySound(PlayerSound i) {
			int s = -1;
			playerSoundsEncode.TryGetValue (i, out s);
			identity.sendBehaviourPacket (new PacketMetadata (this, s));
		}

		public void playSound(AudioClip c) {
			source.clip = c;
			source.Play ();
		}

		public void playSound(PlayerSound i) {
			AudioClip c;
			switch (i) {
			case PlayerSound.ItemPickup:
				c = itemPickupSound;
				break;
			case PlayerSound.ConsumeFinish:
				c = burpSound;
				break;
			case PlayerSound.Hurt:
				c = hurtSound;
				break;
			default:
				c = itemPickupSound;
				break;
			}
			playSound (c);
		} 

		public override void handleBehaviourPacket (PacketBehaviour p) {
			base.handleBehaviourPacket (p);
			if (p.id == 11) {
				PacketMetadata o = (PacketMetadata)p;
				rpc_playSound_o (o.metadata);
			}
		}

		private void createSoundDictionaries() {
			int i = 0;
			PlayerSound s = PlayerSound.ItemPickup;
			playerSoundsDecode.Add (i, s);
			playerSoundsEncode.Add (s, i);
			i++;
			s = PlayerSound.ConsumeFinish;
			playerSoundsDecode.Add (i, s);
			playerSoundsEncode.Add (s, i);
			i++;
			s = PlayerSound.Hurt;
			playerSoundsDecode.Add (i, s);
			playerSoundsEncode.Add (s, i);
			i++;
		}

		/*
		 * 
		 * Behavior-Necessary 
		 * 
		 */

		private void rpc_playSound_o(int sound) {
			PlayerSound s;
			playerSoundsDecode.TryGetValue (sound, out s);
			playSound (s);
		}

		private void Start() {
			source = GetComponent<AudioSource>();
			createSoundDictionaries();
		}

	}
		
	public enum PlayerSound {
		ItemPickup,
		ConsumeFinish,
		Hurt,
	}

}