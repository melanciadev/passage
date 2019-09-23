using UnityEngine;
using System.Collections;

public class Intro:MonoBehaviour {
	public Texture2D[] tex;
	
	bool p1confirm,p2confirm;
	
	Transform tr;
	TextMesh mesh;
	AudioSource aud;
	Renderer logo;
	
	void Start() {
		p1confirm = p2confirm = false;
		tr = transform;
		mesh = tr.Find("inst").GetComponent<TextMesh>();
		mesh.text = "press ENTER and RIGHT MOUSE BUTTON to begin";
		aud = GetComponent<AudioSource>();
		logo = tr.Find("logo").GetComponent<Renderer>();
	}
	
	void Update() {
		logo.material.mainTexture = tex[(int)(Time.time*6)%tex.Length];
		if (Game.block) return;
		if (Input.GetKeyDown(KeyCode.Return)) {
			aud.PlayOneShot(aud.clip);
			p1confirm = true;
			if (p2confirm) {
				Game.LoadScene("start");
			} else {
				mesh.text = "now press RIGHT MOUSE BUTTON to begin";
			}
		}
		if (Input.GetMouseButtonDown(1)) {
			aud.PlayOneShot(aud.clip);
			p2confirm = true;
			if (p1confirm) {
				Game.LoadScene("start");
			} else {
				mesh.text = "now press ENTER to begin";
			}
		}
	}
}