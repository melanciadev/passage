using UnityEngine;
using System.Collections;

public class EndScreen:MonoBehaviour {
	public Texture2D[] good;
	public Texture2D[] bad;
	Texture2D[] tex;
	
	int type;
	
	Transform tr;
	TextMesh mesh;
	Renderer rend;
	
	float fade;
	bool fading;
	
	int state;
	float tempo;
	
	void Start() {
		tr = transform;
		mesh = tr.Find("text").GetComponent<TextMesh>();
		rend = tr.Find("bg").GetComponent<Renderer>();
		state = 0;
		tempo = 0;
		
		if (Level.dead) {
			type = 0; //morreu
		} else {
			Level.requiredAttAGot = Level.GetAtt(Level.requiredAttA) >= 99.9f;
			Level.requiredAttBGot = Level.GetAtt(Level.requiredAttB) >= 99.9f;
			if (!Level.requiredAttAGot || !Level.requiredAttBGot || !Level.requiredExitGot) {
				if (Level.requiredExitGot) {
					type = 1; //n conseguiu atributos
				} else if (Level.requiredAttAGot && Level.requiredAttBGot) {
					type = 2; //n conseguiu a saída certa
				} else {
					type = 0; //n conseguiu nada
				}
			} else {
				type = 3; //foi de boa
			}
		}
		tex = (type == 3)?good:bad;
		
		SetScreen();
	}
	
	void Update() {
		if (fading) {
			fade += Time.deltaTime;
			if (fade >= 1) {
				SetScreen();
			} else {
				mesh.color = new Color(1,1,1,Utils.Ease(1-fade));
			}
		} else if (fade > 0) {
			fade -= Time.deltaTime;
			if (fade <= Mathf.Epsilon) {
				fade = 0;
			} else {
				mesh.color = new Color(1,1,1,Utils.Ease(1-fade));
			}
		} else {
			tempo -= Time.deltaTime;
			if (tempo <= Mathf.Epsilon || PressHead() || PressBody()) {
				NextScreen();
			}
		}
		rend.material.mainTexture = tex[(int)(Time.time*6)%tex.Length];
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
		mesh.color = Color.clear;
		if (type == 0) {
			switch (state) {
				case 0:
					mesh.text = "";
					tempo = 0;
					break;
				case 1:
					mesh.text = "YOU ARE WORTHLESS.";
					tempo = 5;
					break;
				case 2:
					mesh.text = "";
					tempo = 0;
					break;
				default:
					Game.LoadScene("intro");
					tempo = 100;
					break;
			}
		} else if (type == 1) {
			switch (state) {
				case 0:
					mesh.text = "";
					tempo = 0;
					break;
				case 1:
					mesh.text = "YOU FAILED TO ACQUIRE ENLIGHTENMENT.";
					tempo = 5;
					break;
				case 2:
					mesh.text = "YOU ARE WORTHLESS.";
					tempo = 5;
					break;
				case 3:
					mesh.text = "";
					tempo = 0;
					break;
				default:
					Game.LoadScene("intro");
					tempo = 100;
					break;
			}
		} else if (type == 2) {
			switch (state) {
				case 0:
					mesh.text = "";
					tempo = 0;
					break;
				case 1:
					mesh.text = "YOU GOT LOST IN THE WAY.";
					tempo = 5;
					break;
				case 2:
					mesh.text = "YOU ARE WORTHLESS.";
					tempo = 5;
					break;
				case 3:
					mesh.text = "";
					tempo = 0;
					break;
				default:
					Game.LoadScene("intro");
					tempo = 100;
					break;
			}
		} else {
			switch (state) {
				case 0:
					mesh.text = "";
					tempo = 0;
					break;
				case 1:
					mesh.text = "CONGRATULATIONS, YOU ARE NOW ONE.";
					tempo = 5;
					break;
				case 2:
					mesh.text = "THROUGH "+Game.GetAttName(Level.requiredAttA)+" AND "+Game.GetAttName(Level.requiredAttB)+"\nYOU BECAME "+Game.GetAttCombination(Level.requiredAttA,Level.requiredAttB)+".";
					tempo = 10;
					break;
				case 3:
					mesh.text = "THE RITUAL IS COMPLETE.";
					tempo = 5;
					break;
				case 4:
					mesh.text = "";
					tempo = 0;
					break;
				default:
					Game.LoadScene("intro");
					tempo = 100;
					break;
			}
		}
	}
}