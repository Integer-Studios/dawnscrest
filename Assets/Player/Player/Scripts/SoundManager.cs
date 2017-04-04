using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using PolyItem;

namespace PolyPlayer {
	public class SoundManager : NetworkBehaviour {

		public AudioClip itemPickupSound;
		public AudioClip hurtSound;
		public AudioClip burpSound;	

		private AudioSource source;
		private Dictionary<PlayerSound, int> playerSoundsEncode = new Dictionary<PlayerSound, int>();
		private Dictionary<int, PlayerSound> playerSoundsDecode = new Dictionary<int, PlayerSound>();

		public void rpcPlaySound(PlayerSound i) {
			int s;
			playerSoundsEncode.TryGetValue (i, out s);
			RpcPlaySound_o (s);
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

		[ClientRpc]
		private void RpcPlaySound_o(int sound) {
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