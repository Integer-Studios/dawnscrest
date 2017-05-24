using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Polytechnica.Dawnscrest.Player;

public class Photobooth : MonoBehaviour {

	public AppearanceVisualizer model;
	public Camera cam;

	public Sprite GetProfilePicture(AppearanceSet appearance) {
		model.SetAppearance (appearance);
		return TakePhoto (cam);
	}

	private Sprite TakePhoto(Camera virtualCam) {
		int sqr = 512;
		virtualCam.aspect = 1.0f;
		RenderTexture tempRT = new RenderTexture (sqr, sqr, 24);
		virtualCam.targetTexture = tempRT;
		virtualCam.Render ();
		RenderTexture.active = tempRT;
		Texture2D virtualPhoto = new Texture2D (sqr, sqr, TextureFormat.RGB24, false);
		virtualPhoto.ReadPixels (new Rect (0, 0, sqr, sqr), 0, 0);
		virtualPhoto.Apply ();
		RenderTexture.active = null;
		virtualCam.targetTexture = null;
		return Sprite.Create (virtualPhoto, new Rect (0f, 0f, virtualPhoto.width, virtualPhoto.height), new Vector2 (0.5f, 0.5f), 100f);
	}

}
