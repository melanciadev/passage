using UnityEngine;
using System.Collections;

public class Portal:Event {
	Renderer rend;
	Texture2D[][] tex = null;
	
	public override void Initialise() {
		rend = GetComponent<Renderer>();
		
		if (tex == null) {
			tex = new Texture2D[][] {
				new Texture2D[8],
				new Texture2D[8],
				new Texture2D[8],
				new Texture2D[8]
			};
			for (int a = 0; a < 4; a++) {
				for (int b = 0; b < 8; b++) {
					tex[a][b] = Resources.Load<Texture2D>("events/portal/portal"+a.ToString()+b.ToString());
				}
			}
		}
		
		tr.localPosition = Pos();
		tr.localScale = Vector3.one;
		tr.localRotation = Quaternion.identity;
	}
	
	void Update() {
		rend.material.mainTexture = tex[type][(int)(Time.time*6)%8];
		if (Game.block) return;
		if (Area(Level.me.player)) {
			Level.requiredExitGot = Level.requiredExit == type;
			Game.LoadScene("end");
		}
	}
}