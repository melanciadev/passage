using UnityEngine;
using System.Collections;

public class Friend:Event {
	float answer;
	float tempo;
	float size;
	static Texture2D[] tex = null;
	static AudioClip[] clips;
	Transform sprite;
	AudioSource aud;
	
	public override void Initialise() {
		emitter = true;
		signal = false;
		answer = 0;
		tempo = 0;
		
		sprite = tr.Find("sprite");
		aud = GetComponent<AudioSource>();
		
		if (tex == null) {
			tex = new Texture2D[6];
			clips = new AudioClip[6];
			for (int a = 0; a < 6; a++) {
				tex[a] = Resources.Load<Texture2D>("events/friend/friend"+a);
				clips[a] = Resources.Load<AudioClip>("events/friend/friend"+a);
			}
		}
		sprite.GetComponent<Renderer>().material.mainTexture = tex[type];
		
		if (type == 0) {
			size = 1;
		} else if (type == 3) {
			size = 1.3f;
		} else {
			size = 2;
		}
		
		tr.localPosition = Pos();
		tr.localScale = Vector3.one;
		tr.localRotation = Quaternion.identity;
		sprite.localScale = new Vector3(size,size,1);
	}
	
	void Update() {
		if (Game.block) return;
		if (tempo > 0) {
			tempo -= Time.deltaTime;
			if (tempo < 0) tempo = 0;
			float t;
			if (tempo > 1.5f) {
				t = 1-(tempo-1.5f)*2;
			} else if (tempo < .5f) {
				t = tempo*2;
			} else {
				t = 1;
			}
			sprite.localScale = new Vector3(
				size+Mathf.Sin(Time.time*12)*t*.25f,
				size+Mathf.Cos(Time.time*12)*t*.25f,
			1);
		}
		if (answer > 0) {
			answer -= Time.deltaTime;
			if (answer <= Mathf.Epsilon) {
				answer = 0;
				if (tempo < .5f) {
					tempo = 2-tempo;
				} else if (tempo < 1.5f) {
					tempo = 1.5f;
				}
				aud.PlayOneShot(clips[type]);
				if (!signal) {
					signal = true;
					switch (type) {
						case 0: Level.friendship += 50; break;
						case 1: Level.friendship += 5; break;
						case 2: Level.friendship += 50; break;
						case 3: Level.friendship += 10; break;
						case 4: Level.friendship += 20; break;
						case 5: Level.friendship += 20; break;
					}
					Emit();
				}
			}
		} else if (Level.me.player.Talked(this)) {
			answer = 1;
		}
	}
	
	public override bool Receive(Event ev = null) {
		return signal;
	}
}