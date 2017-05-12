using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PolyItem;

namespace PolyNet {

	public class PacketHelper {

		public static void read(ref BinaryReader reader, ref Recipe recipe) {
			int x = reader.ReadInt32 ();
			if (x != -1) {
				recipe = new Recipe ();
				read (ref reader, ref recipe.input);
				read (ref reader, ref recipe.output);
			} else
				recipe = null;
		}

		public static void write(ref BinaryWriter writer, Recipe recipe) {
			if (recipe == null)
				writer.Write (-1);
			else {
				writer.Write (1);
				write (ref writer, recipe.input);
				write (ref writer, recipe.output);
			}
		}

		public static void read(ref BinaryReader reader, ref ItemStack[] stacks) {
			int x = reader.ReadInt32 ();
			if (x != -1) {
				stacks = new ItemStack [reader.ReadInt32 ()];
				for (int i = 0; i < stacks.GetLength (0); i++) {
					read (ref reader, ref stacks [i]);
				}
			} else
				stacks = null;
		}

		public static void write(ref BinaryWriter writer, ItemStack[] stacks) {
			if (stacks == null)
				writer.Write (-1);
			else {
				writer.Write (1);
				writer.Write (stacks.GetLength (0));
				for (int i = 0; i < stacks.GetLength (0); i++) {
					write (ref writer, stacks [i]);
				}
			}
		}

		public static void read (ref BinaryReader reader, ref ItemStack stack) {
			if (reader.ReadBoolean ())
				stack = new ItemStack (reader.ReadInt32 (), reader.ReadInt32 (), reader.ReadInt32 ());
			else
				stack = null;
		}

		public static void write(ref BinaryWriter writer, ItemStack stack) {
			if (stack != null) {
				writer.Write (true);
				writer.Write (stack.id);
				writer.Write (stack.quality);
				writer.Write (stack.size);
			} else
				writer.Write (false);
		}

		public static void read (ref BinaryReader reader, ref GameObject obj) {
			int id = reader.ReadInt32 ();
			if (id == -1)
				obj = null;
			else
				obj = PolyNetWorld.getObject (id).gameObject;
		}

		public static void write(ref BinaryWriter writer, GameObject obj) {
			int i = -1;
			PolyNetIdentity identity = obj.GetComponentInParent<PolyNetIdentity>();
			if (identity != null)
				i = identity.getInstanceId ();
			writer.Write (i);
		}

	}

}