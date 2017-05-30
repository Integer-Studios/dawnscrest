using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Unity.Visualizer;
using Improbable.Entity.Component;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable;
using Improbable.Math;
using Polytechnica.Dawnscrest.Player;

namespace Polytechnica.Dawnscrest.Core {

	/*
	 * This class creates new characters
	 */
	public class CharacterCreator : MonoBehaviour {

		[Require] private CharacterCreatorController.Writer creatorControllerWriter;

		private Dictionary<int, CreationRequest> creationsInProgress = new  Dictionary<int, CreationRequest>();

		private void OnEnable() {
			creatorControllerWriter.CommandReceiver.OnCreateFamily.RegisterAsyncResponse (CreateFamily);
		}

		private void OnDisable() {
			creatorControllerWriter.CommandReceiver.OnCreateFamily.DeregisterResponse ();
		}

		/*
		 * Command handler for character creation
		 * Starts first character creation process, then adds the job to the queue
		 */
		private void CreateFamily(ResponseHandle<CharacterCreatorController.Commands.CreateFamily, CreateFamilyRequest, Nothing> responseHandle) {
			int houseId = (int)responseHandle.Request.houseId;
			AppearanceSet a = new AppearanceSet (responseHandle.Request.sex, (int)responseHandle.Request.hairColor, (int)responseHandle.Request.eyeColor, (int)responseHandle.Request.build, (int)responseHandle.Request.hair, (int)responseHandle.Request.facialHair, (int)responseHandle.Request.eyebrow);
			Debug.LogWarning (a.ToString ());
			creationsInProgress.Add (houseId, new CreationRequest(0, responseHandle, a));
			CreateCharacterWithReservedId (houseId, true);
		}

		/*
		 * Reserves a new entity ID for the new character
		 */
		private void CreateCharacterWithReservedId(int houseId, bool active) {
			SpatialOS.Commands.ReserveEntityId(creatorControllerWriter)
				.OnSuccess(reservedEntityId => CreateCharacter(houseId, active, reservedEntityId))
				.OnFailure(failure => OnFailedReservation(failure, houseId, active));
		}

		private void OnFailedReservation(ICommandErrorDetails response, int houseId, bool active) {
			Debug.LogError("Failed to Reserve EntityId for Character: " + response.ErrorMessage + ". Retrying...");
			CreateCharacterWithReservedId(houseId, active);
		}

		/*
		 * Uses the entity ID to spawn a new character for the given house
		 */
		private void CreateCharacter(int houseId, bool active, EntityId entityId) {
			CreationRequest req; 
			if (!creationsInProgress.TryGetValue (houseId, out req)) {
				Debug.LogError ("Lost House's Request, Quitting! Fuck! This is Fucked!");
				return;
			}
			AppearanceSet aSet = req.appearance;
			if (active) {
				var charEntityTemplate = EntityTemplateFactory.CreateCharacterTemplate(houseId, active, req.appearance);
				SpatialOS.Commands.CreateEntity(creatorControllerWriter, entityId, "Character", charEntityTemplate)
					.OnFailure(failure => OnFailedCharacterCreation(failure, houseId, active,  entityId))
					.OnSuccess(response => OnCreationSuccess(houseId));
			} else {
				AppearanceSet s = AppearanceSet.GetGeneticVariation (req.appearance);
				var charEntityTemplate = EntityTemplateFactory.CreateCharacterTemplate(houseId, active, s);
				SpatialOS.Commands.CreateEntity(creatorControllerWriter, entityId, "Character", charEntityTemplate)
					.OnFailure(failure => OnFailedCharacterCreation(failure, houseId, active,  entityId))
					.OnSuccess(response => OnCreationSuccess(houseId));
			}
		}

		private void OnFailedCharacterCreation(ICommandErrorDetails response, int houseId, bool active, EntityId entityId) {
			Debug.LogError("Failed to Create Character Entity: " + response.ErrorMessage + ". Retrying...");
			CreateCharacter(houseId, active, entityId);
		}

		/*
		 * Handles when a single character is completed
		 */
		private void OnCreationSuccess(int houseId) {
			CreationRequest req; 
			// Get response handle for creation request out of the map
			if (!creationsInProgress.TryGetValue (houseId, out req)) {
				Debug.LogError ("Lost House's Request, Quitting! Fuck! This is Fucked!");
				return;
			}
			// increment the job status
			req.amountCreated++;
			if (req.amountCreated == 3) {
				// If the job is done, respond to the creation request
				req.ResponseHandle.Respond (new Nothing ());
			} else {
				// Else, create another character for the family
				CreateCharacterWithReservedId (houseId, false);
			}
		}

		/*
		 * Struct-like class for creation request map
		 */
		private class CreationRequest {
			public int amountCreated;
			public ResponseHandle<CharacterCreatorController.Commands.CreateFamily, CreateFamilyRequest, Nothing> ResponseHandle;
			public AppearanceSet appearance;
			public CreationRequest(int a, ResponseHandle<CharacterCreatorController.Commands.CreateFamily, CreateFamilyRequest, Nothing> r, AppearanceSet ap) {
				amountCreated = a;
				ResponseHandle = r;
				appearance = ap;
			}
		}

	}

}
