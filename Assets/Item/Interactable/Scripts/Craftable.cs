﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PolyItem {

	public class Craftable : Workable, ISaveable {

		// Vars : public, protected, private, hide
		public CraftingType cratingType;

		protected Recipe recipe;
		protected ItemStack[] input;

		/*
		* 
		* Public Interface
		* 
		*/

		// Server Interface

		[Server]
		public virtual void setRecipe(Recipe r) {
			Debug.Log ("craftable recipe: " + r);
			recipe = r;
			if (recipe != null) {
				setMaxStrength (5f);
				input = new ItemStack[r.input.GetLength (0)];
			} else {
				setMaxStrength (0f);
				input = null;
			}

			if (r == null)
				RpcSetRecipe (new NetworkItemStack (null), new NetworkItemStackArray (null));
			else
				RpcSetRecipe (new NetworkItemStack (r.output), new NetworkItemStackArray (r.input));
		}

		[Server]
		public virtual void setInput(ItemStack[] s) {
			Debug.Log ("craftable input: " + s);
			input = s;
			RpcSetInput (new NetworkItemStackArray (s));
		}

		// General Interface
			
		public override bool isInteractable(Interactor i) {
			return isSatisfied() && base.isInteractable(i);
		}

		public bool isSatisfied() {
			if (!isLoaded ())
				return false;

			if (input.GetLength (0) < recipe.input.GetLength (0))
				return false;

			for(int i = 0; i < recipe.input.GetLength (0); i++) {
				if (input [i] == null)
					return false;
				if (input [i].id != recipe.input[i].id)
					return false;
				if (input [i].size < recipe.input[i].size)
					return false;
			}

			return true;
		}

		public bool isLoaded() {
			return recipe != null;
		}

		public Recipe getRecipe() {
			return recipe;
		}

		public ItemStack[] getInput() {
			return input;
		}

		/*
		* 
		* Server->Client Interface
		* 
		*/

		[ClientRpc]
		private void RpcSetRecipe(NetworkItemStack o, NetworkItemStackArray i) {
			Debug.Log ("craftable rpc recipe");
			recipe = Recipe.unwrapRecipe(o,i);
			if (recipe != null)
				input = new ItemStack[recipe.input.GetLength (0)];
			else
				input = null;
		}

		[ClientRpc]
		private void RpcSetInput(NetworkItemStackArray i) {
			Debug.Log ("craftable rpc input");
			input = ItemStack.unwrapNetworkStackArray(i);
		}

		/*
		* 
		* Private
		* 
		*/

		protected override void onComplete(Interactor i) {
			useRequirements ();
			base.onComplete (i);
		}

		private void useRequirements() {
			if (!isLoaded ())
				return;
			Debug.Log ("craftable use requirements:");
			for(int i = 0; i < recipe.input.GetLength (0); i++) {
				Debug.Log ("input i: " + input [i]);
				Debug.Log ("recipe input i: " + recipe.input [i]);
				input [i].size -= recipe.input [i].size;
				if (input [i].size <= 0)
					input [i] = null;
			}

			RpcSetInput (new NetworkItemStackArray (input));
		}

		public virtual JSONObject write () {
			JSONObject obj = new JSONObject (JSONObject.Type.OBJECT);
			JSONObject recipeInput = new JSONObject (JSONObject.Type.ARRAY);
			JSONObject recipeOutput = new JSONObject (JSONObject.Type.OBJECT);
			JSONObject inputJSON = new JSONObject (JSONObject.Type.ARRAY);

			if (recipe != null) {
				foreach (ItemStack s in recipe.input) {
					recipeInput.Add (s.serializeJSON ());
				}
				if (input != null) {
					foreach (ItemStack s in input) {
						if (s != null)
							inputJSON.Add (s.serializeJSON ());
					}
				}
				recipeOutput = recipe.output.serializeJSON ();
			}

			obj.AddField ("recipeInput", recipeInput);
			obj.AddField ("recipeOutput", recipeOutput);
			obj.AddField ("input", inputJSON);
			obj.AddField ("type", "CRAFT");
			return obj;
		}

		public virtual void read (JSONObject obj) {
			JSONObject inputJSON = obj.GetField ("input");
			JSONObject outputJSON = obj.GetField ("output");
			JSONObject recipeJSON = obj.GetField ("recipe");
		
			ItemStack[] recipeArray = new ItemStack[recipeJSON.list.Count];
			int x = 0;
			foreach (JSONObject stackJSON in recipeJSON.list) {
				recipeArray [x] = new ItemStack (stackJSON);
				x++;
			}			

			ItemStack[] inputArray = new ItemStack[inputJSON.list.Count];
			x = 0;
			foreach (JSONObject stackJSON in inputJSON.list) {
				inputArray [x] = new ItemStack (stackJSON);
				x++;
			}

			if (recipeArray.Length != 0) {
				recipe = new Recipe (new ItemStack (outputJSON), recipeArray);
			}
			if (inputArray.Length != 0)
				input = inputArray;
			else 
				input = new ItemStack[0];

		}

		public string getType() {
			return "craftable";
		}

	}

}
