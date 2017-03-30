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
		public MaterialSoundEntry[] materialSoundEntries;
		public MaterialKey defaultMaterialKey;		

		private static AudioSource source;
		private static SoundManager soundManager;
		private static Dictionary<PlayerSound, int> playerSoundsEncode = new Dictionary<PlayerSound, int>();
		private static Dictionary<int, PlayerSound> playerSoundsDecode = new Dictionary<int, PlayerSound>();
		private static Dictionary<MaterialKey, AudioClip[]> materialSounds = new Dictionary<MaterialKey, AudioClip[]>();

		public static void rpcPlaySound(PlayerSound i) {
			int s;
			playerSoundsEncode.TryGetValue (i, out s);
			soundManager.RpcPlaySound_o (s);
		}

		public static void playDefaultSound() {
			AudioClip[] c;
			materialSounds.TryGetValue (soundManager.defaultMaterialKey, out c);
			playSound (c[Random.Range(0,c.GetLength(0))]);
		}

		public static void playSound(ItemStack s, GameObject g) {
			MaterialType mat2 = MaterialType.Earth;
			Interactable i = g.GetComponent<Interactable> ();
			if (i != null) {
				mat2 = i.material;
			} else if (g.layer == 4) {
				mat2 = MaterialType.Water;
			} else if (g.layer == 8) {
				//get block here later
				mat2 = MaterialType.Earth;
			} else {
				playDefaultSound ();
				return;
			}
			MaterialType mat1 = MaterialType.Footstep;
			if (s != null)
				mat1 = ItemManager.getMaterial (s);

			playSound(mat1, mat2);
		}

		public static void playSound(MaterialType m1, MaterialType m2) {
			AudioClip[] c;
			materialSounds.TryGetValue (new MaterialKey (m1, m2), out c);
			if (c != null)
				playSound (c[Random.Range(0,c.GetLength(0))]);
			else
				playDefaultSound ();
		}

		public static void playSound(AudioClip c) {
			source.clip = c;
			source.Play ();
		}

		public static void playSound(PlayerSound i) {
			AudioClip c;
			switch (i) {
			case PlayerSound.ItemPickup:
				c = soundManager.itemPickupSound;
				break;
			case PlayerSound.ConsumeFinish:
				c = soundManager.burpSound;
				break;
			case PlayerSound.Hurt:
				c = soundManager.hurtSound;
				break;
			default:
				c = soundManager.itemPickupSound;
				break;
			}
			playSound (c);
		} 

		private static void createSoundDictionaries() {
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

			foreach (MaterialSoundEntry mse in soundManager.materialSoundEntries) {
				materialSounds.Add (mse.key, mse.sound);
			}
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
			soundManager = this;
			createSoundDictionaries();
		}

	}
		
	public enum PlayerSound {
		ItemPickup,
		ConsumeFinish,
		Hurt,
	}

	[System.Serializable]
	public struct MaterialSoundEntry {
		public MaterialKey key;
		public AudioClip[] sound;
	}

	[System.Serializable]
	public struct MaterialKey {
		public MaterialType mat1;
		public MaterialType mat2;
		public MaterialKey(MaterialType m1, MaterialType m2) {
			mat1 = m1;
			mat2 = m2;
		}
		public override bool Equals(object obj) {
			if (!(obj is MaterialKey))
				return false;
			MaterialKey k2 = (MaterialKey) obj;
			return (k2.contains (mat1) && k2.contains (mat2));
		}
		public override int GetHashCode() {
			if (mat1 < mat2)
				return mat1.GetHashCode() ^ mat2.GetHashCode();
			else
				return mat2.GetHashCode() ^ mat1.GetHashCode();
		}
		public bool contains(MaterialType m) {
			return (mat1 == m || mat2 == m);
		}
	}
}
