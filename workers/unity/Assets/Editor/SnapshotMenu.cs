using Polytechnica.Dawnscrest.Core;
using Improbable;
using Improbable.Worker;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Improbable.Math;
using Polytechnica.Dawnscrest.World;

namespace Polytechnica.Dawnscrest.Editor {
	public class SnapshotMenu : MonoBehaviour {

		[MenuItem("Improbable/Snapshots/Generate Default Snapshot")]
		[UsedImplicitly]
		private static void GenerateDefaultSnapshot() {
			var snapshotEntities = new Dictionary<EntityId, SnapshotEntity>();

			var currentEntityId = 1;
			snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateCharacterCreatorTemplate());
			snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateWorldTimeTemplate());

			WorldTerrain.GenerateHeightmap ();

			int max = WorldTerrain.size / WorldTerrain.chunkSize;
			float progMax = max * max;
			float prog = 0f;
			for (int z = 0; z < max; z++) {
				for (int x = 0; x < max; x++) {
					
					EditorUtility.DisplayProgressBar("Generating Snapshot", ("Building Chunk "+x+", "+z + " ("+prog+" of " + progMax + ")"), prog / progMax);

					// Load chunk heightmap into snapshot
					snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateTerrainChunkTemplate(x, z));
					
					//Load chunk objects from json
					string json = File.ReadAllText(WorldCreator.worldDirectory+x+"-"+z+".chunk");

					//Get object array
					WorldObjectChunk objectChunk = JsonUtility.FromJson<WorldObjectChunk>(json);
					foreach (WorldObject obj in objectChunk.objects) {
						//Add the entity
						snapshotEntities.Add (new EntityId (currentEntityId++), EntityTemplateFactory.CreateEntityTemplate (obj));
					}

					prog++;
				}
			}
			EditorUtility.ClearProgressBar ();

			SaveSnapshot(snapshotEntities);
		}

		private static void SaveSnapshot(IDictionary<EntityId, SnapshotEntity> snapshotEntities) {
			File.Delete(SimulationSettings.DefaultSnapshotPath);
			var maybeError = Snapshot.Save(SimulationSettings.DefaultSnapshotPath, snapshotEntities);

			if (maybeError.HasValue)
				Debug.LogErrorFormat("Failed to generate initial world snapshot: {0}", maybeError.Value);
			else
				Debug.LogFormat("Successfully generated initial world snapshot at {0}", SimulationSettings.DefaultSnapshotPath);
		}
	}
}
