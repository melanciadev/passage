using UnityEngine;
using System.Collections;

public class Culture:Event {
	float tempo;
	
	static Texture2D[] tex = null;
	
	public override void Initialise() {
		emitter = true;
		signal = false;
		
		if (tex == null) {
			tex = new Texture2D[6];
			for (int a = 0; a < 6; a++) {
				tex[a] = Resources.Load<Texture2D>("events/culture/culture"+a);
			}
		}
		GetComponent<Renderer>().material.mainTexture = tex[type];
		
		tr.localPosition = Pos();
		tr.localScale = Vector3.one;
		tr.localRotation = Quaternion.identity;
		
		tempo = 12;
	}
	
	void Update() {
		if (Game.block) return;
		if (Level.me.player.y < y && Level.me.player.Around(this)) {
			Level.me.player.culture = .125f;
			if (!signal) {
				tempo -= Time.deltaTime;
				if (tempo <= Mathf.Epsilon) {
					tempo = 0;
					signal = true;
					switch (type) {
						case 0: Level.culture += 50; break;
						case 1: Level.culture += 20; break;
						case 2: Level.culture += 20; break;
						case 3: Level.culture += 20; break;
						case 4: Level.culture += 10; break;
						case 5: Level.culture += 20; break;
					}
					Emit();
				}
			}
		}
	}
	
	public override bool Receive(Event ev = null) {
		return signal;
	}
}