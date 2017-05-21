using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable;
using Improbable.Core;
using Improbable.Unity.Core;
using Improbable.Unity.Core.EntityQueries;
using Improbable.Worker;
using Improbable.Worker.Query;
using Improbable.Collections;
using Improbable.Unity.Core.Acls;

namespace Polytechnica.Realms.Core {

	public class BodyFinder {
		public static int houseId = 1;
		public static bool isNewPlayer;

		//
		// Basic Plan
		//
		// 1. query for character with player id and active = true
		// 2. set the acl to client auth
		// 3. see if it enables the controller? or like enable it yourself?
		// 4. if it doesnt do this auto also disable the server-side NPC stuff

		public static void CreateFamily() {
			EntityQuery characterQuery = Query.HasComponent<CharacterCreatorController> ().ReturnOnlyEntityIds ();
			SpatialOS.WorkerCommands.SendQuery(characterQuery, queryResult => OnCreateQueryResult(queryResult));
		}

		private static void OnCreateQueryResult(ICommandCallbackResponse<EntityQueryResult> result) {
			if (!result.Response.HasValue || result.StatusCode != StatusCode.Success) {
				Debug.LogError("CharacterCreatorController query failed. SpatialOS workers probably haven't started yet.  Try again in a few seconds.");
				return;
			}

			var queriedEntities = result.Response.Value;
			if (queriedEntities.EntityCount < 1) {
				Debug.LogError("Failed to find CharacterCreatorController. SpatialOS probably hadn't finished spawning the initial snapshot. Try again in a few seconds.");
				return;
			}

			var characterCreatorEntityId = queriedEntities.Entities.First.Value.Key;
			SpatialOS.WorkerCommands.SendCommand (CharacterCreatorController.Commands.CreateFamily.Descriptor, new CreateFamilyRequest ((uint)houseId), characterCreatorEntityId)
				.OnFailure(error => OnCreateFailure(error)).OnSuccess(response => OnCreateSuccess());
		}

		private static void OnCreateFailure(ICommandErrorDetails error) {
			Debug.LogWarning("Create Family command failed - you probably tried to connect too soon. Try again in a few seconds.");
		}

		private static void OnCreateSuccess() {
			Debug.LogWarning("Create Family Success");
		}

		public static void FindBody() {
			EntityQuery characterQuery = Query.HasComponent<Character> ().ReturnComponents (1003);
			SpatialOS.WorkerCommands.SendQuery(characterQuery, queryResult => OnQueryResult(queryResult));
		}

		private static void OnQueryResult(ICommandCallbackResponse<EntityQueryResult> result) {
			if (!result.Response.HasValue || result.StatusCode != StatusCode.Success) {
				Debug.Log ("query Failed");
				return;
			}
			var queriedEntities = result.Response.Value;
			Debug.Log("Found " + queriedEntities.EntityCount + " nearby entities with a character component");
			if (queriedEntities.EntityCount < 1) {
				return;
			}

			// Parse query for family and active character
			Map<EntityId, Entity> characters = queriedEntities.Entities;
			Map<EntityId, Entity> family = new Map<EntityId, Entity>();
			EntityId activeCharacter = new EntityId();
			bool activeCharacterFound = false;
			foreach (var c in characters) {
				Entity e = c.Value;
				var status = e.Get<Character> ().Value.Get ();
				if (status.Value.houseId == houseId) {
					family.Add (c.Key, c.Value);
					if (status.Value.active == true) {
						activeCharacterFound = true;
						activeCharacter = c.Key;
					}
				}
			}

			if (activeCharacterFound) {
				Debug.Log ("Active Character Found, Embodying...");
				SpatialOS.WorkerCommands.SendCommand (Character.Commands.Embody.Descriptor, new Nothing (), activeCharacter)
					.OnFailure(error => OnEmbodyFailure(error));
			} else if (family.Count > 0) {
				Debug.Log ("Looks like you fucking died - nice job retard");
			} else if (isNewPlayer) {
				Debug.Log ("Welcome to Manifest Destiny - lets start your house");
			} else {
				Debug.Log ("Wow. They are actually all dead. you dipshit. you have to restart.");
			}

		}

		private static void OnEmbodyFailure(ICommandErrorDetails error) {
			Debug.LogWarning("Embody command failed - you probably tried to connect too soon. Try again in a few seconds.");
		}
	}

}