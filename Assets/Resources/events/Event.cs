using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Event:MonoBehaviour {
	public static Vector3 Pos(float x,float y,float offset) {
		return new Vector3(x,y+offset,Level.me.heightR*y);
	}
	public virtual Vector3 Pos(float z) {
		return new Vector3(x,y+offset,z);
	}
	
	public virtual Vector3 Pos() {
		return Pos(x,y,offset);
	}
	
	public List<Event> connected = new List<Event>();
	//[System.NonSerialized]
	public float x,y,velX,velY;
	[System.NonSerialized]
	public float offset;
	[System.NonSerialized]
	public int type;
	[System.NonSerialized]
	public string id;
	
	[System.NonSerialized]
	public Transform tr;
	[System.NonSerialized]
	public Collider2D col;
	
	public float width = 1,height = 1;
	[System.NonSerialized]
	public bool signal = false;
	[System.NonSerialized]
	public float range;
	[System.NonSerialized]
	public bool emitter = false;
	[System.NonSerialized]
	public bool receiver = false;
	
	public virtual void Initialise() {
		//
	}
	
	public virtual bool Receive(Event ev = null) {
		return ReceiveFirst(ev);
	}
	
	//receives the first signal it finds
	public virtual bool ReceiveFirst(Event ev = null) {
		signal = false;
		for (int a = 0; a < connected.Count; a++) {
			if (!connected[a].emitter || connected[a] == ev) continue;
			signal = connected[a].Receive();
			break;
		}
		return signal;
	}
	
	//receives any positive signal
	public virtual bool ReceiveOr(Event ev = null) {
		for (int a = 0; a < connected.Count; a++) {
			if (!connected[a].emitter || connected[a] == ev) continue;
			if (connected[a].Receive()) {
				signal = true;
				return true;
			}
		}
		signal = false;
		return false;
	}
	
	//receives positive if every single signal is positive, negative otherwise
	public virtual bool ReceiveAnd(Event ev = null) {
		for (int a = 0; a < connected.Count; a++) {
			if (!connected[a].emitter || connected[a] == ev) continue;
			if (!connected[a].Receive()) {
				signal = false;
				return false;
			}
		}
		signal = true;
		return true;
	}
	
	public virtual void Emit(Event ev = null) {
		bool emitted = false;
		if (emitter) {
			for (int a = 0; a < connected.Count; a++) {
				if (connected[a] == ev) continue;
				connected[a].Emit(this);
				emitted = true;
			}
		}
		if (!emitted && receiver) {
			Receive();
		}
	}
	
	public virtual bool Move(float x,float y) {
		//if (Mathf.Approximately(x,0) && Mathf.Approximately(y,0)) return false;
		return MoveTo(this.x+x,this.y+y);
	}
	
	public virtual bool MoveTo(float x,float y) {
		float ox = this.x;
		float oy = this.y;
		this.x = x;
		this.y = y;
		return Collide(ox,oy);
	}
	
	public virtual bool Collide(float x,float y) {
		float nx,ny;
		bool collide = Level.me.Collide(x,y,this.x,this.y,this,out nx,out ny);
		this.x = nx;
		this.y = ny;
		return collide;
	}
	
	public virtual bool Around(Event ev) {
		return Around(ev.x,ev.y);
	}
	
	public virtual bool Around(float x,float y) {
		return (new Vector2(x-this.x,y-this.y)).sqrMagnitude < range*range;
	}
	
	public virtual bool Area(Event ev) {
		return Area(ev.x,ev.y);
	}
	
	public virtual bool Area(float x,float y) {
		return x >= this.x-width*.5f && x <= this.x+width*.5f && y >= this.y-height*.5f && y <= this.y+height*.5f;
	}
	
	public virtual void Damage(Event ev,float amount = 1) {
		var dir = (new Vector2(x-ev.x,y-ev.y)).normalized;
		velX = dir.x;
		velY = dir.y;
	}
	
	public virtual bool Slide(out float vx,out float vy) {
		bool slide = false;
		vx = 0;
		vy = 0;
		if (Mathf.Abs(velX) > 0) {
			int sign = (int)Mathf.Sign(velX);
			velX -= Time.deltaTime*sign;
			if ((int)Mathf.Sign(velX) != sign) {
				velX = 0;
			} else {
				slide = true;
				vx = velX*3.5f*Time.deltaTime;
			}
		}
		if (Mathf.Abs(velY) > 0) {
			slide = true;
			int sign = (int)Mathf.Sign(velY);
			velY -= Time.deltaTime*sign;
			if ((int)Mathf.Sign(velY) != sign) {
				velY = 0;
			} else {
				slide = true;
				vy = velY*3.5f*Time.deltaTime;
			}
		}
		return slide;
	}
}