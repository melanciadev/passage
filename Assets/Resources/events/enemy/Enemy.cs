using UnityEngine;
using System.Collections;

public class Enemy:Event {
	public float vel = 1;
	const float attackStart = .75f;
	const float attackEnd = .25f;
	const float damageGet = .25f;
	float attackTempo;
	float damageTempo;
	float pulseTempo;
	float colourTempo;
	float life;
	
	static bool loadTexture = true;
	static Texture2D[] texIdle,texAttack,texDead;
	static AudioClip[] clips;
	float deadTempo;
	Transform sprite;
	Renderer rend;
	AudioSource aud;
	
	public override void Initialise() {
		tr.localScale = Vector3.one;
		tr.localRotation = Quaternion.identity;
		
		signal = false;
		emitter = true;
		range = 1f;
		offset = .3f;
		life = 3;
		
		attackTempo = 0;
		damageTempo = 0;
		pulseTempo = 0;
		colourTempo = 0;
		
		if (loadTexture) {
			loadTexture = false;
			texIdle = new Texture2D[] {
				Resources.Load<Texture2D>("events/enemy/inimigo_idle1"),
				Resources.Load<Texture2D>("events/enemy/inimigo_idle2"),
				Resources.Load<Texture2D>("events/enemy/inimigo_idle3"),
				Resources.Load<Texture2D>("events/enemy/inimigo_idle4"),
				Resources.Load<Texture2D>("events/enemy/inimigo_idle5"),
				Resources.Load<Texture2D>("events/enemy/inimigo_idle6"),
				Resources.Load<Texture2D>("events/enemy/inimigo_idle7"),
				Resources.Load<Texture2D>("events/enemy/inimigo_idle8"),
				Resources.Load<Texture2D>("events/enemy/inimigo_idle9"),
				Resources.Load<Texture2D>("events/enemy/inimigo_idle10"),
				Resources.Load<Texture2D>("events/enemy/inimigo_idle11"),
				Resources.Load<Texture2D>("events/enemy/inimigo_idle12")
			};
			texAttack = new Texture2D[] {
				Resources.Load<Texture2D>("events/enemy/inimigo_atacando1"),
				Resources.Load<Texture2D>("events/enemy/inimigo_atacando2"),
				Resources.Load<Texture2D>("events/enemy/inimigo_atacando3"),
				Resources.Load<Texture2D>("events/enemy/inimigo_atacando4"),
				Resources.Load<Texture2D>("events/enemy/inimigo_atacando5"),
				Resources.Load<Texture2D>("events/enemy/inimigo_atacando6"),
				Resources.Load<Texture2D>("events/enemy/inimigo_atacando7"),
				Resources.Load<Texture2D>("events/enemy/inimigo_atacando8"),
				Resources.Load<Texture2D>("events/enemy/inimigo_atacando9")
			};
			texDead = new Texture2D[] {
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu1"),
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu2"),
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu3"),
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu4"),
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu5"),
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu6"),
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu7"),
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu8"),
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu9"),
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu10"),
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu11"),
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu12"),
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu13"),
				Resources.Load<Texture2D>("events/enemy/inimogo_morteu14")
			};
			clips = new AudioClip[] {
				Resources.Load<AudioClip>("events/enemy/attack"),
				Resources.Load<AudioClip>("events/enemy/damage"),
				Resources.Load<AudioClip>("events/enemy/dead")
			};
		}
		sprite = tr.Find("sprite");
		rend = sprite.GetComponent<Renderer>();
		aud = GetComponent<AudioSource>();
	}
	
	void Start() {
		tr.localPosition = Pos();
		if (signal) Destroy(gameObject);
	}
	
	void Update() {
		if (Game.block) return;
		if (pulseTempo > 0) {
			pulseTempo -= Time.deltaTime*4;
			if (pulseTempo < 0) pulseTempo = 0;
			sprite.localScale = new Vector3(Utils.ParabolaIn(1.2f,1.4f,pulseTempo),Utils.ParabolaOut(1.2f,1.4f,pulseTempo),1);
		}
		if (colourTempo > 0) {
			colourTempo -= Time.deltaTime*4;
			if (colourTempo < 0) colourTempo = 0;
			rend.material.color = Color.Lerp(Color.white,Color.red,colourTempo);
		}
		if (signal) {
			if (deadTempo > 0) {
				deadTempo -= Time.deltaTime*.8f;
				if (deadTempo <= Mathf.Epsilon) {
					deadTempo = 0;
					rend.enabled = false;
				} else {
					rend.material.mainTexture = texDead[(int)((1-deadTempo)*texDead.Length)];
				}
			}
			return;
		}
		float x,y;
		bool walk = Slide(out x,out y);
		if (Level.me.player.Attacked(this)) Damage(Level.me.player,1);
		if (Level.me.player.Pressed(this)) Damage(this,0);
		if (damageTempo > 0) {
			attackTempo = 0;
			damageTempo -= Time.deltaTime;
			if (damageTempo < 0) damageTempo = 0;
			rend.material.mainTexture = texIdle[0];
		} else if (attackTempo > 0) {
			if (attackTempo > attackEnd) {
				attackTempo -= Time.deltaTime;
				if (attackTempo <= attackEnd) {
					if (Around(Level.me.player)) {
						Level.me.player.Damage(this,20);
						aud.PlayOneShot(clips[0]);
					}
					if (attackTempo < 0) attackTempo = 0;
				}
			} else {
				attackTempo -= Time.deltaTime;
				if (attackTempo < 0) attackTempo = 0;
			}
			rend.material.mainTexture = texAttack[((int)((1-attackTempo/(attackStart+attackEnd))*texAttack.Length))%texAttack.Length];
		} else {
			float nx = Level.me.player.x-this.x;
			float ny = Level.me.player.y-this.y;
			float v = vel*Time.deltaTime;
			if (Mathf.Abs(nx) > .15f) {
				if (nx > 0) {
					x += v;
					tr.localScale = Vector3.one;
				} else {
					x -= v;
					tr.localScale = new Vector3(-1,1,1);
				}
				walk = true;
			}
			if (Mathf.Abs(ny) > .15f) {
				if (ny > 0) {
					y += v;
				} else {
					y -= v;
				}
				walk = true;
			}
			if (Around(Level.me.player)) {
				attackTempo = attackStart+attackEnd;
			}
			rend.material.mainTexture = texIdle[(int)((Time.time%1)*texIdle.Length)];
		}
		if (walk) {
			Move(x,y);
			tr.localPosition = Pos();
		}
	}
	
	public override void Damage(Event ev,float amount = 1) {
		base.Damage(ev,amount);
		if (signal) return;
		life -= amount;
		if (life <= Mathf.Epsilon) {
			signal = true;
			life = 0;
			col.enabled = false;
			Emit();
			Level.power += 8;
			if (Level.power > 100) Level.power = 100;
			deadTempo = 1;
			aud.PlayOneShot(clips[2]);
		} else {
			damageTempo = damageGet;
			aud.PlayOneShot(clips[1]);
		}
		pulseTempo = 1;
		if (amount > 0) colourTempo = 1;
	}
	
	public override bool Receive(Event ev = null) {
		return signal;
	}
}