using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNet;

public class ResourceTester : MonoBehaviour {

	public float size;
	public float density;

	public GameObject[] prefabs;

	// Use this for initialization
	void Start () {
		if (!FindObjectOfType<PolyNetManager> ().isClient) {
			StartCoroutine (test ());
		}
	}

	private IEnumerator test() {
		yield return new WaitForSeconds (1f);
		for (int i = 0; i < (int)(density * Mathf.Pow(size,2)); i++) {
			GameObject g = Instantiate (prefabs [Random.Range (0, prefabs.GetLength (0))]);
			g.transform.position = new Vector3 (Random.Range (size / -2f, size / 2f), 0f, Random.Range (size / -2f, size / 2f));
			PolyNetWorld.spawnObject (g);
			if (i % 25 == 0)
				yield return null;
		}
	}

}
