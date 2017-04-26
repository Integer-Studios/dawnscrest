﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;

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

		public virtual void setRecipe(Recipe r) {
			recipe = r;
			if (recipe != null) {
				setMaxStrength (5f);
				input = new ItemStack[r.input.GetLength (0)];
			} else {
				setMaxStrength (0f);
				input = null;
			}

			identity.sendBehaviourPacket (new PacketRecipe (this, r));
			identity.sendBehaviourPacket (new PacketItemStackArray (this, input));
		}

		public virtual void setInput(ItemStack[] s) {
			input = s;
			identity.sendBehaviourPacket (new PacketItemStackArray (this, s));
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

		public override void handleBehaviourPacket (PacketBehaviour p) {
			base.handleBehaviourPacket (p);
			if (p.id == 19) {
				PacketItemStackArray o = (PacketItemStackArray)p;
				rpc_setInput (o.stacks);
			} else if (p.id == 20) {
				PacketRecipe o = (PacketRecipe)p;
				rpc_setRecipe (o.recipe);
			}
		}

		/*
		* 
		* Server->Client Interface
		* 
		*/

		private void rpc_setRecipe(Recipe r) {
			recipe = r;
		}

		private void rpc_setInput(ItemStack[] i) {
			input = i;
		}

		/*
		* 
		* Private
		* 
		*/

		protected override void onComplete(Interactor i) {
			if (!isSatisfied())
				return;
			useRequirements ();
			base.onComplete (i);
		}

		private void useRequirements() {
			for(int i = 0; i < recipe.input.GetLength (0); i++) {
				input [i].size -= recipe.input [i].size;
				if (input [i].size <= 0)
					input [i] = null;
			}

			identity.sendBehaviourPacket (new PacketItemStackArray (this, input));
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
