using UnityEngine;
using System.Collections;

public class Static:MonoBehaviour {
	static Texture2D[] tex = null;
	
	const int positionFg = -2;
	const int positionBg = 2;
	
	[System.NonSerialized]
	public int x = 0,y = 0,type = 0;
	[System.NonSerialized]
	public bool foreground = false;
	[System.NonSerialized]
	public Transform tr = null;
	[System.NonSerialized]
	public Renderer rend = null;
	
	public virtual void Initialise() {
		if (tex == null) {
			tex = new Texture2D[46];
			for (int a = 0; a < 46; a++) {
				tex[a] = Resources.Load<Texture2D>("statics/"+(a+1));
			}
		}
		tr = transform;
		tr.localPosition = new Vector3(x,y,foreground?positionFg:positionBg);
		tr.localScale = Vector3.one;
		tr.localRotation = Quaternion.identity;
		rend = GetComponent<Renderer>();
		rend.material.mainTexture = tex[type-1];
	}
}