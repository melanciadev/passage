using UnityEngine;
using System.Collections;

public class QuickAnim:MonoBehaviour {
	public int frames = 0;
	public float fps = 1;
	
	Transform tr;
	Renderer rend;
	float frame;
	bool playing;
	int width;
	Vector3 np;
	bool fixPos;
	Vector3 fixTo;
	
	void Start() {
		tr = transform;
		rend = GetComponent<Renderer>();
		rend.enabled = false;
		frame = 0;
		playing = false;
		width = Mathf.RoundToInt(1f/rend.material.mainTextureScale.x);
		if (width < 1) width = 1;
		np = tr.localPosition;
		fixPos = false;
		fixTo = Vector3.zero;
	}
	
	public void Play(bool fixPos = false) {
		frame = 0;
		playing = true;
		rend.enabled = true;
		tr.localPosition = np;
		if (fixPos) {
			this.fixPos = true;
			fixTo = tr.position;
		} else {
			this.fixPos = false;
		}
	}
	
	public void Pause() {
		playing = false;
	}
	
	public void Stop() {
		playing = false;
		rend.enabled = false;
		frame = 0;
	}
	
	void Update() {
		if (!playing) return;
		if (fixPos) {
			tr.position = fixTo;
		}
		frame += Time.deltaTime*fps;
		if (frame >= frames) {
			Stop();
		} else {
			int f = (int)frame;
			rend.material.mainTextureOffset = new Vector2(
				(f%width)*rend.material.mainTextureScale.x,
				(f/width)*rend.material.mainTextureScale.y
			);
		}
	}
}