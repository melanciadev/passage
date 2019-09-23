using UnityEngine;
using System.Collections;

public class Pressure:Event {
	Renderer rend;
	AudioSource aud;
	
	static AudioClip[] clips = null;
	bool over;
	float tempo;
	
	public override void Initialise() {
		emitter = true;
		signal = false;
		over = false;
		
		rend = GetComponent<Renderer>();
		aud = GetComponent<AudioSource>();
		
		if (clips == null) {
			clips = new AudioClip[] {
				Resources.Load<AudioClip>("events/pressure/press"),
				Resources.Load<AudioClip>("events/pressure/release")
			};
		}
		
		tr.localPosition = Pos(1.5f);
		tr.localRotation = Quaternion.identity;
		
		tempo = 0;
	}
	
	void Start() {
		rend.material.mainTextureOffset = new Vector2(0,signal?.5f:0);
		Animate();
	}
	
	void Update() {
		bool nover = Area(Level.me.player);
		if (over != nover) {
			if (nover) {
				signal = !signal;
				Emit();
				tempo = 1;
			}
			over = nover;
			rend.material.mainTextureOffset = new Vector2(over?.5f:0,signal?.5f:0);
			aud.PlayOneShot(clips[over?0:1]);
		}
		if (tempo > 0) {
			tempo -= Time.deltaTime*4;
			if (tempo < 0) tempo = 0;
			Animate();
		}
	}
	
	void Animate() {
		tr.localScale = new Vector3(Utils.Parabola(1.2f,1.4f,tempo),1.2f,1);
	}
	
	public override bool Receive(Event ev = null) {
		return signal;
	}
}