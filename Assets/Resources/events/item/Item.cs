using UnityEngine;
using System.Collections;

public class Item:Event {
	Renderer rend;
	AudioSource aud;
	float tempo;
	float size;
	
	static Texture2D[] tex = null;
	static AudioClip clip;
	
	public override void Initialise() {
		emitter = true;
		signal = false;
		
		if (tex == null) {
			tex = new Texture2D[12];
			for (int a = 0; a < 12; a++) {
				tex[a] = Resources.Load<Texture2D>("events/item/item"+a);
			}
			clip = Resources.Load<AudioClip>("events/item/pick");
		}
		rend = GetComponent<Renderer>();
		rend.material.mainTexture = tex[type];
		aud = GetComponent<AudioSource>();
		
		size = 1.5f;
		
		tr.localPosition = Pos();
		tr.localScale = new Vector3(size,size,1);
		tr.localRotation = Quaternion.identity;
	}
	
	void Start() {
		if (signal) Destroy(gameObject);
		tempo = 0;
	}
	
	void Update() {
		if (signal) {
			if (tempo > 0) {
				tempo -= Time.deltaTime*3;
				if (tempo <= Mathf.Epsilon) {
					rend.enabled = false;
				} else {
					tr.localScale = new Vector3(size*Utils.ParabolaOut(.8f,1,tempo),size*Utils.ParabolaIn(1.2f,1,tempo),1);
					rend.material.color = new Color(1,1,1,Utils.EaseOut(tempo));
				}
			}
			return;
		}
		if (Level.me.player.Pressed(this)) {
			signal = true;
			tempo = 1;
			switch (type) {
				case 0: Level.wealth += 20; break;
				case 1: Level.wealth += 20; break;
				case 2: Level.wealth += 10; break;
				case 3: Level.wealth += 10; break;
				case 4: Level.wealth += 20; break;
				case 5: Level.wealth += 10; break;
				case 6: Level.wealth += 5; break;
				case 7: Level.wealth += 5; break;
				case 8: Level.wealth += 10; break;
				case 9: Level.wealth += 50; break;
				case 10: Level.wealth += 20; break;
				case 11: Level.wealth += 20; break;
			}
			aud.PlayOneShot(clip,.6f);
			Emit();
		}
	}
	
	public override bool Receive(Event ev = null) {
		return signal;
	}
}