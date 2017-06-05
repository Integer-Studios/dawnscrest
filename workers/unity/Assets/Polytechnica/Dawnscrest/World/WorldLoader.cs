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
using Polytechnica.Dawnscrest.Core;
using System.IO;

namespace Polytechnica.Dawnscrest.World {

	public class WorldLoader : MonoBehaviour {

		public static string worldObjectsDirectory = "/Users/quinnfinney/Code/World/";

		private WorldTerrain terrain;

		[Require] private WorldTransform.Writer transformWriter;

		public static  void LoadChunks() {
			WorldTerrain.GenerateHeightmap ();
			Chunk[] chunks = FindObjectsOfType<Chunk> ();
			foreach (Chunk c in chunks) {
				WorldTerrain.LoadChunkTerrain (c);
				LoadChunkObjects (c);
			}
			Debug.LogWarning ("Successfully Loaded " + chunks.Length + " Chunks.");
		}
			
		private void LoadChunkObjects(Chunk c) {
			string json = LoadChunkObjectJSON (c);
			WorldObjectChunk objectChunk = JsonUtility.FromJson<WorldObjectChunk>(json);
			foreach (WorldObject obj in objectChunk.objects) {
				LoadObject(obj);
			}
		}

		private string LoadChunkObjectJSON(Chunk c) {
			ChunkIndex i = c.GetIndex ();
			return File.ReadAllText(worldObjectsDirectory+i.x+"-"+i.z+".chunk");
		}

		private void LoadObject(WorldObject w) {
			SpatialOS.Commands.ReserveEntityId(transformWriter)
				.OnSuccess(reservedEntityId => CreateEntity(reservedEntityId, w))
				.OnFailure(failure => OnFailedReservation(failure, w));
		}

		private void OnFailedReservation(ICommandErrorDetails response, WorldObject w) {
			Debug.LogError("Failed to Reserve EntityId for Entity: " + response.ErrorMessage + ". Retrying...");
			LoadObject(w);
		}

		private void CreateEntity(EntityId entityId, WorldObject w) {
			var charEntityTemplate = EntityTemplateFactory.CreateEntityTemplate(w);
			SpatialOS.Commands.CreateEntity(transformWriter, entityId, w.name, charEntityTemplate)
				.OnFailure(failure => OnFailedCharacterCreation(failure, w,  entityId))
				.OnSuccess(response => OnCreationSuccess(w));
		}

		private void OnFailedCharacterCreation(ICommandErrorDetails response, WorldObject w, EntityId entityId) {
			Debug.LogError("Failed to Create Entity: " + response.ErrorMessage + ". Retrying...");
			CreateEntity(entityId, w);
		}

		private void OnCreationSuccess(WorldObject w) {
			// Continue Loading
			Debug.LogWarning ("Successfully Created Entity");
		}


	}

}