using UnityEngine;
using System.Collections;

public static class Utils {
	//ease. most of it from mathfx, from the unify community wiki
	public static float EaseIn(float t) {
		if (t <= 0) return 0;
		if (t >= 1) return 1;
		return t*t;
	}
	public static float EaseIn(float start,float end,float t) {
		return start+EaseIn(t)*(end-start);
	}
	public static float EaseOut(float t) {
		if (t <= 0) return 0;
		if (t >= 1) return 1;
		return (2-t)*t;
	}
	public static float EaseOut(float start,float end,float t) {
		return start+EaseOut(t)*(end-start);
	}
	public static float Ease(float t) {
		if (t <= 0) return 0;
		if (t >= 1) return 1;
		return (3-2*t)*t*t;
	}
	public static float Ease(float start,float end,float t) {
		return start+Ease(t)*(end-start);
	}
	public static float Boing(float t) {
		if (t <= 0) return 0;
		if (t >= 1) return 1;
		return (Mathf.Sin(t*Mathf.PI*(.2f+2.5f*t*t*t))*EaseIn(1-t)+t)*(1+(1.2f*(1-t)));
	}
	public static float Boing(float start,float end,float t) {
		return start+Boing(t)*(end-start);
	}
	public static float Parabola(float t) {
		if (t <= 0) return 0;
		if (t >= 1) return 1;
		return 4*(t-t*t);
	}
	public static float Parabola(float start,float end,float t) {
		return start+Parabola(t)*(end-start);
	}
	public static float ParabolaIn(float t) {
		return Parabola(EaseIn(t));
	}
	public static float ParabolaIn(float start,float end,float t) {
		return start+ParabolaIn(t)*(end-start);
	}
	public static float ParabolaOut(float t) {
		return Parabola(EaseOut(t));
	}
	public static float ParabolaOut(float start,float end,float t) {
		return start+ParabolaOut(t)*(end-start);
	}
	
	//hm
	public static float LerpFree(float start,float end,float t) {
		return start+(end-start)*t;
	}
	
	//debug
	public static void DrawPoint(Vector3 point,Color colour,float duration = 0,bool depthTest = true) {
		Debug.DrawLine(new Vector3(point.x-.1f,point.y-.1f),new Vector3(point.x+.1f,point.y+.1f),colour,duration,depthTest);
		Debug.DrawLine(new Vector3(point.x+.1f,point.y-.1f),new Vector3(point.x-.1f,point.y+.1f),colour,duration,depthTest);
	}
}

public struct int2 {
	public int x,y;
	
	public int2(int x = 0,int y = 0) {
		this.x = x;
		this.y = y;
	}
}

public struct int4 {
	public int x0,y0,x1,y1;
	
	public int4(int x0 = 0,int y0 = 0,int x1 = 0,int y1 = 0) {
		this.x0 = x0;
		this.y0 = y0;
		this.x1 = x1;
		this.y1 = y1;
	}
}