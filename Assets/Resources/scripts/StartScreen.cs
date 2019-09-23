using UnityEngine;
using System.Collections;

public class StartScreen:MonoBehaviour {
	public Texture2D[] bodyTex,headTex;
	public Texture2D[] keyboardTex,mouseTex;
	
	Texture2D[] tex1,tex2;
	
	Transform tr;
	Renderer overlay;
	TextMesh mesh;
	Renderer body,head;
	
	float fade;
	bool fading;
	
	int state;
	float tempo;
	bool pressHead,pressBody;
	
	void Start() {
		tr = transform;
		overlay = tr.Find("overlay").GetComponent<Renderer>();
		mesh = tr.Find("text").GetComponent<TextMesh>();
		body = tr.Find("body").GetComponent<Renderer>();
		head = tr.Find("head").GetComponent<Renderer>();
		state = 0;
		tempo = 0;
		pressHead = pressBody = false;
		overlay.enabled = true;
		head.enabled = false;
		body.enabled = false;
		tex1 = bodyTex;
		tex2 = headTex;
		SetScreen();
		
		Random.seed = System.Environment.TickCount;
		Level.requiredAttA = ((int)(Random.value*7)%7)+1;
		Level.requiredAttB = ((int)(Random.value*7)%7)+1;
		if (Level.requiredAttB == Level.requiredAttA) {
			Level.requiredAttB = (Level.requiredAttB+1)%8;
		}
		Level.requiredExit = (int)(Random.value*4)%4;
	}
	
	void Update() {
		if (fading) {
			fade += Time.deltaTime;
			if (fade >= 1) {
				SetScreen();
			} else {
				overlay.material.color = new Color(0,0,0,Utils.Ease(fade));
			}
		} else if (fade > 0) {
			fade -= Time.deltaTime;
			if (fade <= Mathf.Epsilon) {
				fade = 0;
			} else {
				overlay.material.color = new Color(0,0,0,Utils.Ease(fade));
			}
		} else if (pressHead || pressBody) {
			if ((pressHead && PressHead()) || (pressBody && PressBody())) {
				NextScreen();
			}
		} else {
			tempo -= Time.deltaTime;
			if (tempo <= Mathf.Epsilon || PressHead() || PressBody()) {
				NextScreen();
			}
		}
		int f;
		if (tex1.Length == 1) {
			f = 0;
		} else {
			f = (int)(Time.time*6)%tex1.Length;
		}
		body.material.mainTexture = tex1[f];
		head.material.mainTexture = tex2[f];
	}
	
	bool PressHead() {
		return Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
	}
	
	bool PressBody() {
		return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
	}
	
	void NextScreen() {
		fading = true;
		fade = 0;
		state++;
	}
	
	void SetScreen() {
		fading = false;
		fade = 1;
		overlay.material.color = Color.black;
		pressHead = pressBody = false;
		switch (state) {
			case 0:
				mesh.text = "HELLO, INITIATES.";
				tempo = 5;
				break;
			case 1:
				mesh.text = "THE PASSAGE IS OPEN AND UNDERWAY,\nIT IS TIME TO BEGIN YOUR RITUAL.";
				tempo = 10;
				break;
			case 2:
				body.enabled = true;
				head.enabled = true;
				mesh.text = "TO ACHIEVE SUCCESS\nYOU MUST WORK TOGETHER.";
				tempo = 10;
				break;
			case 3:
				tex1 = keyboardTex;
				tex2 = mouseTex;
				mesh.text = "USE THESE CONTROLS\nTO ACT UPON THE\nENVIRONMENT.";
				tempo = 10;
				break;
			case 4:
				tex1 = bodyTex;
				tex2 = headTex;
				mesh.text = "NOW, THE BODY MUST\nCLOSE THEIR EYES\nFOR US TO PROCEED.\n(click to continue)";
				pressHead = true;
				break;
			case 5:
				body.enabled = false;
				mesh.text = "TO COMPLETE THE RITUAL,\nONE MUST ACQUIRE...\n\n"+Game.GetAttName(Level.requiredAttA)+" and "+Game.GetAttName(Level.requiredAttB)+".\n(click to continue)";
				pressHead = true;
				break;
			case 6:
				body.enabled = true;
				mesh.text = "NOW, THE MIND MUST\nCLOSE THEIR EYES, AND\nTHE BODY MUST OPEN THEIRS.\n(press space to continue)";
				pressBody = true;
				break;
			case 7:
				head.enabled = false;
				mesh.text = "TO SUCCEED, ONE MUST REACH...\n\n"+Game.GetDirName(Level.requiredExit)+" PORTAL.\n(press space to continue)";
				pressBody = true;
				break;
			case 8:
				head.enabled = true;
				mesh.text = "OPEN YOUR\nMIND'S EYE NOW.\n(press anything to continue)";
				pressHead = true;
				pressBody = true;
				break;
			case 9:
				mesh.text = "NEITHER IS ALLOWED\nTO COMMUNICATE OUTSIDE\nTHE GAME BY ANY MEANS.";
				tempo = 12;
				break;
			case 10:
				body.enabled = false;
				head.enabled = false;
				mesh.text = "DO NOT FAIL.";
				tempo = 5;
				break;
			case 11:
				mesh.text = "";
				tempo = 0;
				break;
			default:
				Game.LoadScene("level");
				tempo = 100;
				break;
		}
	}
}