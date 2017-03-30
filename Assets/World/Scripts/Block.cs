using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyWorld {
	[System.Serializable]
	public class Block {
		public int id;
		public Color color;

		public static Block getBlock(int id) {
			foreach (Block b in blocks) {
				if (b.id == id)
					return b;
			}
			return null;
		}

		public static Block[] blocks;

		public static void setBlocks(Block[] b) {
			blocks = b;
		}

	}
}