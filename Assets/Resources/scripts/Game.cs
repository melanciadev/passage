using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Game:MonoBehaviour {
	public static Game me = null;
	[System.NonSerialized]
	public Transform tr;
	AudioSource aud;
	Renderer fade;
	
	public static bool block;
	[System.NonSerialized]
	public string prevScene;
	[System.NonSerialized]
	public string currentScene;
	[System.NonSerialized]
	public string nextScene;
	[System.NonSerialized]
	public bool newScene;
	[System.NonSerialized]
	public float sceneTempo;
	[System.NonSerialized]
	public bool fadeAudio;
	
	void Start() {
		if (me != null) {
			Destroy(gameObject);
			return;
		}
		me = this;
		DontDestroyOnLoad(gameObject);
		tr = transform;
		aud = GetComponent<AudioSource>();
		fade = tr.Find("fade/fade").GetComponent<Renderer>();
		
		Window.Start();
		
		sceneTempo = 0;
		AnimateFade();
		block = true;
		
		currentScene = SceneManager.GetActiveScene().name;
		SetNewScene(string.Empty,currentScene);
		MapReader.SetRouteList();
		fadeAudio = false;
		
		#if UNITY_EDITOR
		#else
		Cursor.visible = false;
		#endif
	}
	
	void Update() {
		if (newScene) {
			sceneTempo -= Time.deltaTime*2;
			if (sceneTempo <= Mathf.Epsilon) {
				if (fadeAudio) {
					fadeAudio = false;
					aud.Stop();
				}
				sceneTempo = 0;
				newScene = false;
				SceneManager.LoadScene(nextScene);
				SetNewScene(currentScene,nextScene);
				currentScene = nextScene;
			} else if (fadeAudio) {
				aud.volume = sceneTempo;
			}
			AnimateFade();
		} else if (sceneTempo < 1) {
			sceneTempo += Time.deltaTime*2;
			if (sceneTempo >= 1) {
				sceneTempo = 1;
				block = false;
			}
			AnimateFade();
		}
		
		Window.Update();
	}
	
	public static void LoadScene(string n) {
		me.nextScene = n;
		me.newScene = true;
		me.sceneTempo = 1;
		block = true;
		if (n != "level") {
			me.fadeAudio = true;
		}
	}
	
	void SetNewScene(string prev,string curr) {
		if (curr == "level" && prev != "level") {
			Level.ResetWorld();
			Level.requestedLevel = "C";
			Level.useRoute = false;
			aud.volume = 1;
			aud.time = 0;
			aud.Play();
		}
	}
	
	void AnimateFade() {
		if (sceneTempo < 1) {
			fade.enabled = true;
			fade.material.color = new Color(0,0,0,1-Utils.Ease(sceneTempo));
		} else {
			fade.enabled = false;
		}
	}
	
	public static string GetDirName(int id) {
		switch (id) {
			case 0: return "NORTH";
			case 1: return "WEST";
			case 2: return "SOUTH";
			case 3: return "EAST";
			default: return string.Empty;
		}
	}
	
	public static string GetAttName(int id) {
		switch (id) {
			case 0: return "HEALTH";
			case 1: return "PEACE";
			case 2: return "EXPLORATION";
			case 3: return "POWER";
			case 4: return "WEALTH";
			case 5: return "CULTURE";
			case 6: return "PENITENCE";
			case 7: return "FRIENDSHIP";
			default: return string.Empty;
		}
	}
	
	public static string GetAttCombination(int a,int b) {
		if (a > b) return GetAttCombination(b,a);
		switch (a*10+b) {
			case 12: return "A HERMIT";
			case 13: return "A MONK";
			case 14: return "A BARON";
			case 15: return "A SCHOLAR";
			case 16: return "A SAINT";
			case 17: return "A BENEFACTOR";
			case 23: return "A SLAYER";
			case 24: return "AN ADVENTURER";
			case 25: return "AN EXPLORER";
			case 26: return "A WANDERER";
			case 27: return "AN AMBASSADOR";
			case 34: return "A CONQUEROR";
			case 35: return "A GOVERNOR";
			case 36: return "A VIGILANTE";
			case 37: return "A GUARDIAN";
			case 45: return "A CURATOR";
			case 46: return "A PARDONER";
			case 47: return "A TRADER";
			case 56: return "A ZEALOT";
			case 57: return "A TEACHER";
			case 67: return "A MARTYR";
			default: return string.Empty;
		}
	}
}