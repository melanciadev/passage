using UnityEngine;
using System.Collections;

//directly from rock porra

public class CamSet:MonoBehaviour {
	public bool fullRect = true;
	public Rect rect = new Rect(0,0,1,1);
	Camera cam;
	
	void Start() {
		cam = GetComponent<Camera>();
		if (fullRect) {
			cam.rect = Window.fullRect;
		} else {
			cam.rect = Window.CamRect(rect);
		}
	}
	
	void LateUpdate() {
		if (Window.update) {
			if (fullRect) {
				cam.rect = Window.fullRect;
			} else {
				cam.rect = Window.CamRect(rect);
			}
		}
	}
}

public static class Window {
	public static int width;
	public static int height;
	public static float prop;
	public static float realProp;
	public static bool update = false;
	public static Rect fullRect = new Rect(0,0,1,1);
	
	const float maxProp = 16f/9f;
	const float minProp = maxProp;
	static bool propIgnore = false;
	static bool propType = false; //true: > maxProp; false: < minProp
	static float propMin = 0;
	static float propSize = 0;
	
	public static void Start() {
		width = height = 1;
	}
	
	public static void Update() {
		if (width != Screen.width || height != Screen.height) {
			update = true;
			SetValues();
		} else {
			update = false;
		}
	}
	
	static void SetValues() {
		width = Screen.width;
		height = Screen.height;
		realProp = prop = (float)width/height;
		if (prop > maxProp) {
			prop = maxProp;
			propIgnore = false;
			propType = true;
			propSize = maxProp/realProp;
			propMin = (1-propSize)/2f;
			fullRect.Set(propMin,0,propSize,1);
		} else if (prop < minProp) {
			prop = minProp;
			propIgnore = false;
			propType = false;
			propSize = realProp/minProp;
			propMin = (1-propSize)/2f;
			fullRect.Set(0,propMin,1,propSize);
		} else {
			propIgnore = true;
			fullRect.Set(0,0,1,1);
		}
	}
	
	public static Rect CamRect(Rect r) {
		if (propIgnore) return r;
		return CamRect(r.xMin,r.yMin,r.width,r.height);
	}
	
	public static Rect CamRect(float x0,float y0,float x1,float y1) {
		if (propIgnore) return new Rect(x0,y0,x1,y1);
		x1 += x0;
		y1 += y0;
		if (propType) {
			if (x0 < 0) x0 = 0;
			if (x1 > 1) x1 = 1;
			x0 = propMin+propSize*x0;
			x1 = propMin+propSize*x1;
		} else {
			if (y0 < 0) y0 = 0;
			if (y1 > 1) y1 = 1;
			y0 = propMin+propSize*y0;
			y1 = propMin+propSize*y1;
		}
		return Rect.MinMaxRect(x0,y0,x1,y1);
	}
	
	public static float CamToWindowX(float x) {
		if (propType) return (propMin+propSize*x)*Screen.width;
		return x*Screen.width;
	}
	
	public static float CamToWindowY(float y) {
		if (propType) return y*Screen.height;
		return (propMin+propSize*y)*Screen.height;
	}
	
	public static float WindowToCamX(float x) {
		if (propType) return (x/Screen.width-propMin)/propSize;
		return x/Screen.width;
	}
	
	public static float WindowToCamY(float y) {
		if (propType) return y/Screen.height;
		return (y/Screen.height-propMin)/propSize;
	}
}