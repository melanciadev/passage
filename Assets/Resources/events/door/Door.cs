using UnityEngine;
using System.Collections;

public class Door:Event {
	Renderer rend;
	AudioSource aud;
	
	static AudioClip clip = null;
	Texture2D texOn,texOff;
	bool noSound = false;
	float tempo = 0;
	float typeH;
	
	public override void Initialise() {
		receiver = true;
		
		if (clip == null) {
			clip = Resources.Load<AudioClip>("events/door/toggle");
		}
		rend = GetComponent<Renderer>();
		aud = GetComponent<AudioSource>();
		const float s = .125f;
		if (type == 0) {
			typeH = 0;
			width = 1;
			height = s;
		} else {
			typeH = .5f;
			width = s;
			height = 1;
		}
		((BoxCollider2D)col).size = new Vector2(width,height);
		
		tr.localPosition = Pos();
		tr.localScale = Vector3.one;
		tr.localRotation = Quaternion.identity;
	}
	
	void Start() {
		noSound = true;
		Receive();
		noSound = false;
		tempo = signal?0:1;
		rend.material.mainTextureOffset = new Vector2(tempo*.75f,typeH);
	}
	
	void Update() {
		if (signal) {
			if (tempo > 0) {
				tempo -= Time.deltaTime*8;
				if (tempo < 0) tempo = 0;
				rend.material.mainTextureOffset = new Vector2(Mathf.CeilToInt(tempo*2)*.25f,typeH);
			}
		} else {
			if (tempo < 1) {
				tempo += Time.deltaTime*8;
				if (tempo > 1) tempo = 1;
				rend.material.mainTextureOffset = new Vector2(Mathf.FloorToInt(tempo*2+1)*.25f,typeH);
			}
		}
	}
	
	public override bool Receive(Event ev = null) {
		ReceiveAnd(ev);
		if (!noSound && col.enabled == signal) {
			aud.PlayOneShot(clip);
		}
		col.enabled = !signal;
		return signal;
	}
}