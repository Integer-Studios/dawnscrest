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

namespace Polytechnica.Dawnscrest.Core {

	public class CharacterCreator : MonoBehaviour {

		[Require] private CharacterCreatorController.Writer CreatorControllerWriter;

		private Dictionary<int, CreationRequest> creationsInProgress = new  Dictionary<int, CreationRequest>();

		private void OnEnable() {
			CreatorControllerWriter.CommandReceiver.OnCreateFamily.RegisterAsyncResponse (CreateFamily);
		}

		private void OnDisable() {
			CreatorControllerWriter.CommandReceiver.OnCreateFamily.DeregisterResponse ();
		}

		private void CreateFamily(ResponseHandle<CharacterCreatorController.Commands.CreateFamily, CreateFamilyRequest, Nothing> responseHandle) {
			int houseId = (int)responseHandle.Request.houseId;
			creationsInProgress.Add (houseId, new CreationRequest(0, responseHandle));
			CreateCharacterWithReservedId (houseId, true);
		}

		private void CreateCharacterWithReservedId(int houseId, bool active) {
			SpatialOS.Commands.ReserveEntityId(CreatorControllerWriter)
				.OnSuccess(reservedEntityId => CreateCharacter(houseId, active, reservedEntityId))
				.OnFailure(failure => OnFailedReservation(failure, houseId, active));
		}

		private void OnFailedReservation(ICommandErrorDetails response, int houseId, bool active) {
			Debug.LogError("Failed to Reserve EntityId for Character: " + response.ErrorMessage + ". Retrying...");
			CreateCharacterWithReservedId(houseId, active);
		}

		private void CreateCharacter(int houseId, bool active, EntityId entityId) {
			var charEntityTemplate = EntityTemplateFactory.CreateCharacterTemplate(houseId, true);
			SpatialOS.Commands.CreateEntity(CreatorControllerWriter, entityId, "Character", charEntityTemplate)
				.OnFailure(failure => OnFailedCharacterCreation(failure, houseId, active,  entityId))
				.OnSuccess(response => OnCreationSuccess(houseId));
		}

		private void OnFailedCharacterCreation(ICommandErrorDetails response, int houseId, bool active, EntityId entityId) {
			Debug.LogError("Failed to Create Character Entity: " + response.ErrorMessage + ". Retrying...");
			CreateCharacter(houseId, active, entityId);
		}

		private void OnCreationSuccess(int houseId) {
			CreationRequest req; 
			if (!creationsInProgress.TryGetValue (houseId, out req)) {
				Debug.LogError ("Lost House's Request, Quitting! Fuck! This is Fucked!");
				return;
			}
			req.amountCreated++;
			if (req.amountCreated == 3) {
				req.ResponseHandle.Respond (new Nothing ());
			} else {
				CreateCharacterWithReservedId (houseId, false);
			}
		}

		private class CreationRequest {
			public int amountCreated;
			public ResponseHandle<CharacterCreatorController.Commands.CreateFamily, CreateFamilyRequest, Nothing> ResponseHandle;
			public CreationRequest(int a, ResponseHandle<CharacterCreatorController.Commands.CreateFamily, CreateFamilyRequest, Nothing> r) {
				amountCreated = a;
				ResponseHandle = r;
			}
		}

	}

}
