using UnityEngine;
using System.Collections;

public class Player:Event {
	public float vel = 2;
	public float radius = 2;
	
	[System.NonSerialized]
	public float culture = 0;
	[System.NonSerialized]
	public float headTalk = 0;
	[System.NonSerialized]
	public float pressTempo = 0;
	
	bool pressed,attacked,onRadius,talked;
	float holdTempo;
	
	static bool loadTexture = true;
	static Texture2D[] texIdle,texCulture,texAttack,texChirp;
	static Texture2D[] texUp,texDown,texLeft,texRight;
	public static AudioClip[] clips;
	float chirpTempo;
	float attackTempo;
	float animTempo;
	Texture2D[] animFrames;
	float pulseTempo;
	
	Transform sprite;
	Renderer rend;
	QuickAnim ping;
	AudioSource aud;
	
	public override void Initialise() {
		if (loadTexture) {
			loadTexture = false;
			texIdle = new Texture2D[] {
				Resources.Load<Texture2D>("events/player/parado1"),
				Resources.Load<Texture2D>("events/player/parado2"),
				Resources.Load<Texture2D>("events/player/parado3")
			};
			texCulture = new Texture2D[] {
				Resources.Load<Texture2D>("events/player/admirando1"),
				Resources.Load<Texture2D>("events/player/admirando2"),
				Resources.Load<Texture2D>("events/player/admirando3")
			};
			texAttack = new Texture2D[] {
				Resources.Load<Texture2D>("events/player/atacando1"),
				Resources.Load<Texture2D>("events/player/atacando2"),
				Resources.Load<Texture2D>("events/player/atacando3"),
				Resources.Load<Texture2D>("events/player/atacando4")
			};
			texChirp = new Texture2D[] {
				Resources.Load<Texture2D>("events/player/falando1"),
				Resources.Load<Texture2D>("events/player/falando2"),
				Resources.Load<Texture2D>("events/player/falando3")
			};
			texUp = new Texture2D[] {
				Resources.Load<Texture2D>("events/player/andando_tras1"),
				Resources.Load<Texture2D>("events/player/andando_tras2"),
				Resources.Load<Texture2D>("events/player/andando_tras3"),
				Resources.Load<Texture2D>("events/player/andando_tras4"),
				Resources.Load<Texture2D>("events/player/andando_tras5"),
				Resources.Load<Texture2D>("events/player/andando_tras6")
			};
			texDown = new Texture2D[] {
				Resources.Load<Texture2D>("events/player/andando_baixo1"),
				Resources.Load<Texture2D>("events/player/andando_baixo2"),
				Resources.Load<Texture2D>("events/player/andando_baixo3"),
				Resources.Load<Texture2D>("events/player/andando_baixo4"),
				Resources.Load<Texture2D>("events/player/andando_baixo5"),
				Resources.Load<Texture2D>("events/player/andando_baixo6")
			};
			texLeft = new Texture2D[] {
				Resources.Load<Texture2D>("events/player/andando_esquerda1"),
				Resources.Load<Texture2D>("events/player/andando_esquerda2"),
				Resources.Load<Texture2D>("events/player/andando_esquerda3")
			};
			texRight = new Texture2D[] {
				Resources.Load<Texture2D>("events/player/andando_direita1"),
				Resources.Load<Texture2D>("events/player/andando_direita2"),
				Resources.Load<Texture2D>("events/player/andando_direita3")
			};
			clips = new AudioClip[] {
				Resources.Load<AudioClip>("events/player/hit"),
				Resources.Load<AudioClip>("events/player/body"),
				Resources.Load<AudioClip>("events/player/head"),
				Resources.Load<AudioClip>("events/player/attack"),
				Resources.Load<AudioClip>("events/player/press")
			};
		}
		
		tr.localPosition = Pos();
		tr.localScale = Vector3.one;
		tr.localRotation = Quaternion.identity;
		
		sprite = tr.Find("sprite");
		rend = sprite.GetComponent<Renderer>();
		ping = tr.Find("ping").GetComponent<QuickAnim>();
		aud = GetComponent<AudioSource>();
		
		range = 1.5f;
		offset = .3f;
		pressed = false;
		attacked = false;
		holdTempo = 0;
		
		chirpTempo = 0;
		attackTempo = 0;
		animTempo = 0;
		pulseTempo = 0;
	}
	
	void Start() {
		tr.localPosition = Pos();
	}
	
	void Update() {
		attacked = false;
		pressed = false;
		talked = false;
		if (pulseTempo > 0) {
			pulseTempo -= Time.deltaTime*4;
			if (pulseTempo < 0) pulseTempo = 0;
			sprite.localScale = new Vector3(Utils.ParabolaIn(1.2f,1.4f,pulseTempo),Utils.ParabolaOut(1.2f,1.4f,pulseTempo),1);
			rend.material.color = Color.Lerp(Color.white,Color.red,pulseTempo);
		}
		if (headTalk > 0) {
			headTalk -= Time.deltaTime;
			if (headTalk < 0) headTalk = 0;
		}
		if (pressTempo > 0) {
			pressTempo -= Time.deltaTime;
			if (pressTempo < 0) pressTempo = 0;
		}
		if (Game.block || Level.dead) return;
		pressed = Level.me.onRadius && Input.GetMouseButtonDown(0);
		float x,y;
		bool walk = Slide(out x,out y);
		if (holdTempo > 0) {
			holdTempo -= Time.deltaTime;
			if (holdTempo < 0) holdTempo = 0;
		}
		int facing = -1;
		if (chirpTempo <= Mathf.Epsilon && holdTempo <= Mathf.Epsilon) {
			bool up = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
			bool down = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
			bool left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
			bool right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
			float v = vel*Time.deltaTime;
			if (left != right) {
				if (right) {
					facing = 0;
					x += v;
				} else {
					facing = 2;
					x -= v;
				}
				walk = true;
			}
			if (up != down) {
				if (up) {
					facing = 1;
					y += v;
				} else {
					facing = 3;
					y -= v;
				}
				walk = true;
			}
			attacked = Input.GetKeyDown(KeyCode.Space);
		}
		if (walk) {
			if (Move(x,y)) {
				facing = -1;
			}
			tr.localPosition = Pos();
			if (this.x > Level.me.width-.75f) {
				Level.me.LeaveRoom(0);
			} else if (this.y > Level.me.height-.75f) {
				Level.me.LeaveRoom(1);
			} else if (this.x < -.25f) {
				Level.me.LeaveRoom(2);
			} else if (this.y < -.25f) {
				Level.me.LeaveRoom(3);
			}
		}
		if (Input.GetKeyDown(KeyCode.Return)) {
			chirpTempo = texChirp.Length;
			talked = true;
			ping.Play(true);
			aud.PlayOneShot(clips[1]);
		}
		if (Input.GetMouseButtonDown(1)) {
			Level.me.cursorPing.Play(true);
			headTalk = 1;
			aud.PlayOneShot(clips[2]);
		}
		if (attacked) {
			holdTempo = .5f;
			attackTempo = texAttack.Length;
			aud.PlayOneShot(clips[3]);
		}
		bool cultureAnim = false;
		if (culture > 0) {
			culture -= Time.deltaTime*16;
			if (culture < 0) culture = 0;
			cultureAnim = true;
		}
		const float animVel = 8;
		var frames = animFrames;
		if (attackTempo > 0) {
			attackTempo -= Time.deltaTime*animVel;
			if (attackTempo < 0) attackTempo = 0;
			animFrames = texAttack;
		} else if (chirpTempo > 0) {
			chirpTempo -= Time.deltaTime*animVel;
			if (chirpTempo < 0) chirpTempo = 0;
			animFrames = texChirp;
		} else if (facing == 0) {
			animFrames = texRight;
		} else if (facing == 1) {
			animFrames = texUp;
		} else if (facing == 2) {
			animFrames = texLeft;
		} else if (facing == 3) {
			animFrames = texDown;
		} else if (cultureAnim) {
			animFrames = texCulture;
		} else {
			animFrames = texIdle;
		}
		if (animFrames != frames) {
			animTempo = 0;
		} else {
			animTempo += Time.deltaTime*animVel;
			while (animTempo >= animFrames.Length) animTempo -= animFrames.Length;
		}
		rend.material.mainTexture = animFrames[(int)animTempo];
	}
	
	public bool Pressed(Event ev) {
		if (pressed && ev.Area(Level.me.cursorX,Level.me.cursorY)) {
			pressed = false;
			Level.me.cursorHit.Play(true);
			pressTempo = .5f;
			aud.PlayOneShot(clips[4]);
			return true;
		}
		return false;
	}
	
	public bool Attacked(Event ev) {
		if (attacked && Around(ev.x,ev.y)) {
			attacked = false;
			Level.peace -= 4;
			if (Level.peace < 0) Level.peace = 0;
			aud.PlayOneShot(clips[0]);
			return true;
		}
		return false;
	}
	
	public bool Talked(Event ev) {
		if (talked && Around(ev.x,ev.y)) {
			talked = false;
			return true;
		}
		return false;
	}
	
	public override bool Receive(Event ev = null) {
		return false;
	}
	
	public override void Damage(Event ev,float amount = 1) {
		base.Damage(ev,amount);
		if (Level.dead) return;
		Level.health -= amount;
		if (Level.health <= Mathf.Epsilon) {
			Level.dead = true;
			Level.health = 0;
			Game.LoadScene("end");
		} else {
			holdTempo = .5f;
		}
		aud.PlayOneShot(clips[0]);
		Level.penitence += amount*.4f;
		if (Level.penitence > 100) Level.penitence = 100;
		pulseTempo = 1;
	}
}