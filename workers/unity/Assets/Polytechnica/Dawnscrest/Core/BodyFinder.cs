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
using Polytechnica.Dawnscrest.Menu;

namespace Polytechnica.Dawnscrest.Core {

	/*
	 * This is a client-side class used upon connection to spatial to locate a player's
	 * active character and embody that character.
	 * It is also responsible for creating a new family if a player has no characters.
	 */
	public class BodyFinder {

		private static EntityId activeCharacter;

		/*
		 * Creates a new Family request, this first part simply queries to find the CharacterCreator,
		 * a component set up server side to handle creation requests
		 */
		public static void CreateFamily(int house) {
			LoadingMenu.stage = 3;

			EntityQuery characterQuery = Query.HasComponent<CharacterCreatorController> ().ReturnOnlyEntityIds ();
			SpatialOS.WorkerCommands.SendQuery(characterQuery, queryResult => OnCreateQueryResult(queryResult, house));
		}

		/*
		 * Creates a new Logout request
		 */
		public static void Logout() {
			SpatialOS.WorkerCommands.SendCommand (Character.Commands.Logout.Descriptor, new Nothing (), activeCharacter)
				.OnFailure(error => OnLogoutFailure(error)).OnSuccess(response => OnLogoutSuccess());
		}

		/*
		 * Using the CharacterCreators entity id, a request is set to it to create new family
		 */
		private static void OnCreateQueryResult(ICommandCallbackResponse<EntityQueryResult> result, int houseId) {
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
			Polytechnica.Dawnscrest.Player.AppearanceSet g = SettingsManager.appearanceSet;
			SpatialOS.WorkerCommands.SendCommand (CharacterCreatorController.Commands.CreateFamily.Descriptor, new CreateFamilyRequest ((uint)houseId, g.sex, (uint)g.hairColor, (uint)g.eyeColor, (uint)g.build, (uint)g.hair, (uint)g.facialHair, 0), characterCreatorEntityId)
				.OnFailure(error => OnCreateFailure(error)).OnSuccess(response => OnCreateSuccess(houseId));
		}

		private static void OnCreateFailure(ICommandErrorDetails error) {
			Debug.Log("Create Family command failed - you probably tried to connect too soon. Try again in a few seconds.");
		}

		private static void OnCreateSuccess(int houseId) {
			Debug.Log("Create Family Success");
			FindBody (houseId);
		}

		/*
		 * Begins the process of embodiement, sends a query for all characters in the world.
		 * TODO Optimize this query process by having an SQL link to families
		 */
		public static void FindBody(int house) {
			LoadingMenu.stage = 4;

			EntityQuery characterQuery = Query.HasComponent<Character> ().ReturnComponents (1003);
			SpatialOS.WorkerCommands.SendQuery(characterQuery, queryResult => OnQueryResult(queryResult, house));
		}

		/*
		 * Upon succesful character query, parse out the users family and active player, then call
		 * a request to embody that character.
		 */
		private static void OnQueryResult(ICommandCallbackResponse<EntityQueryResult> result, int houseId) {
			if (!result.Response.HasValue || result.StatusCode != StatusCode.Success) {
				Debug.Log ("query Failed");
				return;
			}
			var queriedEntities = result.Response.Value;
			Debug.Log("Found " + queriedEntities.EntityCount + " nearby entities with a character component");

			// Parse query for family and active character
			Map<EntityId, Entity> characters = queriedEntities.Entities;
			Map<EntityId, Entity> family = new Map<EntityId, Entity>();
			activeCharacter = new EntityId();
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

			// There is an active character
			if (activeCharacterFound) {
				Debug.Log ("Active Character Found, Embodying...");
				SpatialOS.WorkerCommands.SendCommand (Character.Commands.Embody.Descriptor, new Nothing (), activeCharacter)
					.OnFailure(error => OnEmbodyFailure(error));
			// TODO handling death
			} else if (family.Count > 0) {
				Debug.Log ("Looks like you fucking died - nice job retard");
			} else {
				Debug.Log ("Welcome to Dawnscrest - lets start your house");
				CreateFamily (houseId);
			}

		}

		private static void OnEmbodyFailure(ICommandErrorDetails error) {
			Debug.LogWarning("Embody command failed - you probably tried to connect too soon. Try again in a few seconds.");
		}

		private static void OnLogoutFailure(ICommandErrorDetails error) {
			Debug.LogWarning("Logout command failed - you probably tried to connect too soon. Try again in a few seconds.");
		}

		private static void OnLogoutSuccess() {
			SpatialOS.Disconnect ();
//			Bootstrap.menuManager.StopGame ();
		}


	}

}