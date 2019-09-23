using UnityEngine;
using System.Collections;

public class ProgressBar:MonoBehaviour {
	Transform bg;
	float v;
	
	public void Initialise(float t = 0) {
		bg = transform.Find("bg");
		v = t+1;
		Set(t);
	}
	
	public void Set(float t) {
		if (Mathf.Approximately(t,v)) return;
		float tt = Mathf.Clamp01(t/100f);
		bg.localScale = new Vector3(tt*3.2f,.88f,1);
		bg.localPosition = new Vector3(tt*1.6f-1.6f,0,.5f);
		v = t;
	}
}