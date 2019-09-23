using UnityEngine;
using System.Collections;

public class Lever:Event {
	Renderer rend;
	AudioSource aud;
	
	static AudioClip clip = null;
	float tempo;
	
	public override void Initialise() {
		emitter = true;
		signal = false;
		
		rend = GetComponent<Renderer>();
		aud = GetComponent<AudioSource>();
		
		if (clip == null) {
			clip = Resources.Load<AudioClip>("events/lever/toggle");
		}
		
		tr.localPosition = Pos();
		tr.localRotation = Quaternion.identity;
		
		tempo = 0;
	}
	
	void Start() {
		rend.material.mainTextureOffset = signal?(new Vector2(.5f,0)):Vector2.zero;
		Animate();
	}
	
	void Update() {
		if (!Game.block && Level.me.player.Pressed(this)) {
			signal = !signal;
			rend.material.mainTextureOffset = signal?(new Vector2(.5f,0)):Vector2.zero;
			tempo = 1;
			Emit();
			aud.PlayOneShot(clip);
			Animate();
		} else if (tempo > 0) {
			tempo -= Time.deltaTime*4;
			if (tempo < 0) tempo = 0;
			Animate();
		}
	}
	
	void Animate() {
		tr.localScale = new Vector3(Utils.ParabolaIn(1.2f,1.4f,tempo),Utils.ParabolaOut(1.2f,1.4f,tempo),1);
	}
	
	public override bool Receive(Event ev = null) {
		return signal;
	}
}