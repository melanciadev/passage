using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level:MonoBehaviour {
	public static Level me = null;
	[System.NonSerialized]
	public Transform tr;
	
	public static string requestedLevel = "";
	public static int requiredAttA,requiredAttB;
	public static bool requiredAttAGot,requiredAttBGot;
	public static int requiredExit;
	public static bool requiredExitGot;
	
	public string overrideLevel = "";
	public string currentLevel = "";
	public static bool useRoute;
	public static Route currentRoute;
	
	static GameObject stPrefab = null;
	static Dictionary<string,GameObject> evPrefab = new Dictionary<string,GameObject>();
	static GameObject coPrefab = null;
	static Dictionary<string,EventProps> eventProps = new Dictionary<string,EventProps>();
	static Dictionary<string,bool> visited = new Dictionary<string,bool>();
	
	public static bool dead;
	
	public static float health;
	public static float peace;
	public static float exploration;
	public static float power;
	public static float wealth;
	public static float culture;
	public static float penitence;
	public static float friendship;
	
	public static float GetAtt(int n) {
		switch (n) {
			case 0: return health;
			case 1: return peace;
			case 2: return exploration;
			case 3: return power;
			case 4: return wealth;
			case 5: return culture;
			case 6: return penitence;
			case 7: return friendship;
			default: return 100;
		}
	}
	
	struct EventProps {
		public Vector2 pos;
		public bool signal;
		
		public EventProps(Event ev) {
			pos = new Vector2(ev.x,ev.y);
			signal = ev.signal;
		}
		
		public void GetState(Event ev) {
			ev.x = pos.x;
			ev.y = pos.y;
			ev.signal = signal;
		}
	}
	
	public static void ResetWorld() {
		eventProps.Clear();
		dead = false;
		health = 100;
		peace = 100;
		exploration = 0;
		power = 0;
		wealth = 0;
		culture = 0;
		penitence = 0;
		friendship = 0;
	}
	
	[System.NonSerialized]
	public int width,height;
	[System.NonSerialized]
	public float heightR;
	public int[,] tileBg,tileEv,tileFg,tileCo;
	public List<int4> wires;
	
	[System.NonSerialized]
	public List<Event> events;
	[System.NonSerialized]
	public Player player;
	
	[System.NonSerialized]
	public float cursorX,cursorY;
	[System.NonSerialized]
	public float cursorAbsX,cursorAbsY;
	[System.NonSerialized]
	public bool onRadius;
	
	Transform camTr;
	[System.NonSerialized]
	public Camera cam;
	Transform staticTr;
	Transform eventTr;
	Transform collisionTr;
	
	Transform cursorTr;
	Renderer cursorRend;
	static Texture2D[] cursorTex = null;
	static Texture2D[] headIdle,headTalk,headPress,headCulture;
	[System.NonSerialized]
	public QuickAnim cursorPing,cursorHit;
	Renderer headCorner;
	
	Transform hudTr;
	ProgressBar[] hudProgress;
	
	void Start() {
		me = this;
		tr = transform;
		int x,y;
		
		camTr = tr.Find("cam");
		cam = camTr.GetComponent<Camera>();
		staticTr = tr.Find("static");
		eventTr = tr.Find("event");
		collisionTr = tr.Find("collision");
		
		cursorTr = camTr.Find("cursor");
		cursorRend = cursorTr.GetComponent<Renderer>();
		if (cursorTex == null) {
			cursorTex = new Texture2D[] {
				Resources.Load<Texture2D>("ui/cursor1"),
				Resources.Load<Texture2D>("ui/cursor2"),
				Resources.Load<Texture2D>("ui/cursor3")
			};
			headIdle = new Texture2D[] {
				Resources.Load<Texture2D>("ui/mind/parado1"),
				Resources.Load<Texture2D>("ui/mind/parado2"),
				Resources.Load<Texture2D>("ui/mind/parado3")
			};
			headTalk = new Texture2D[] {
				Resources.Load<Texture2D>("ui/mind/falando1"),
				Resources.Load<Texture2D>("ui/mind/falando2"),
				Resources.Load<Texture2D>("ui/mind/falando3")
			};
			headPress = new Texture2D[] {
				Resources.Load<Texture2D>("ui/mind/atacando1"),
				Resources.Load<Texture2D>("ui/mind/atacando2"),
				Resources.Load<Texture2D>("ui/mind/atacando3")
			};
			headCulture = new Texture2D[] {
				Resources.Load<Texture2D>("ui/mind/admirando1"),
				Resources.Load<Texture2D>("ui/mind/admirando2"),
				Resources.Load<Texture2D>("ui/mind/admirando3")
			};
		}
		cursorPing = cursorTr.Find("ping").GetComponent<QuickAnim>();
		cursorHit = cursorTr.Find("hit").GetComponent<QuickAnim>();
		headCorner = camTr.Find("mind").GetComponent<Renderer>();
		
		hudTr = camTr.Find("hud");
		hudProgress = new ProgressBar[] {
			hudTr.Find("health").GetComponent<ProgressBar>(),
			hudTr.Find("peace").GetComponent<ProgressBar>(),
			hudTr.Find("exploration").GetComponent<ProgressBar>(),
			hudTr.Find("power").GetComponent<ProgressBar>(),
			hudTr.Find("wealth").GetComponent<ProgressBar>(),
			hudTr.Find("culture").GetComponent<ProgressBar>(),
			hudTr.Find("penitence").GetComponent<ProgressBar>(),
			hudTr.Find("friendship").GetComponent<ProgressBar>()
		};
		for (x = 0; x < 8; x++) {
			hudProgress[x].Initialise();
		}
		
		var reader = new MapReader();
		if (overrideLevel.Length > 0) {
			currentLevel = overrideLevel;
		} else {
			currentLevel = requestedLevel;
		}
		reader.Read(currentLevel);
		
		//Debug.Log("size: "+width+"x"+height);
		//MapReader.DebugTiles(tileBg);
		//MapReader.DebugTiles(tileEv);
		//MapReader.DebugTiles(tileFg);
		//MapReader.DebugTiles(tileCo);
		
		heightR = 1f/height;
		events = new List<Event>();
		for (x = 0; x < width; x++) {
			for (y = 0; y < height; y++) {
				if (tileBg[x,y] > 0) AddStatic(tileBg[x,y],x,y,false);
				if (tileFg[x,y] > 0) AddStatic(tileFg[x,y],x,y,true);
				if (tileEv[x,y] > 0) AddEvent(tileEv[x,y],x,y);
				if (tileCo[x,y] == 1) AddCollision(x,y);
			}
		}
		Event a,b;
		int4 i;
		for (x = 0; x < wires.Count; x++) {
			a = b = null;
			i = wires[x];
			//Debug.Log("wire at "+i.x0+"x"+i.y0+" <-> "+i.x1+"x"+i.y1);
			if (tileEv[i.x0,i.y0] <= 0 || tileEv[i.x1,i.y1] <= 0) continue;
			for (y = 0; y < events.Count; y++) {
				if (Mathf.Approximately(events[y].x,i.x0) && Mathf.Approximately(events[y].y,i.y0)) {
					a = events[y];
					if (b != null) break;
				} else if (Mathf.Approximately(events[y].x,i.x1) && Mathf.Approximately(events[y].y,i.y1)) {
					b = events[y];
					if (a != null) break;
				}
			}
			if (a != null && b != null) {
				if (!a.connected.Contains(b)) a.connected.Add(b);
				if (!b.connected.Contains(a)) b.connected.Add(a);
			}
		}
		if (player == null) {
			AddEvent(1,width/2,height/2);
		}
		if (useRoute) {
			int f = (currentRoute.facing+2)%4;
			if (f == 0) {
				player.y = (height-1)*.5f;
				for (y = 1; y < height-1; y++) {
					if (tileCo[width-1,y] != 1) {
						player.y = y+.5f;
						break;
					}
				}
				player.x = width-1;
			} else if (f == 1) {
				player.x = (width-1)*.5f;
				for (x = width-2; x > 0; x--) {
					if (tileCo[x,height-1] != 1) {
						player.x = x-.5f;
						break;
					}
				}
				player.y = height-1;
			} else if (f == 2) {
				player.y = (height-1)*.5f;
				for (y = height-2; y > 0; y--) {
					if (tileCo[0,y] != 1) {
						player.y = y-.5f;
						break;
					}
				}
				player.x = 0;
			} else {
				player.x = (width-1)*.5f;
				for (x = 1; x < width-1; x++) {
					if (tileCo[x,0] != 1) {
						player.x = x+.5f;
						break;
					}
				}
				player.y = 0;
			}
			useRoute = false;
		}
		EventProps evs;
		for (x = 0; x < events.Count; x++) {
			events[x].Initialise();
			if (eventProps.TryGetValue(events[x].id,out evs)) {
				evs.GetState(events[x]);
			}
		}
		if (!visited.ContainsKey(currentLevel)) {
			visited[currentLevel] = true;
			exploration += 5;
			if (exploration > 100) exploration = 100;
		}
		
		UpdateCamera(true);
	}
	
	void AddStatic(int id,int x,int y,bool fg) {
		if (id < 1 || id > 46) return;
		if (stPrefab == null) {
			stPrefab = Resources.Load<GameObject>("statics/static");
		}
		var st = Instantiate<GameObject>(stPrefab).transform;
		st.name = "static";
		st.parent = staticTr;
		var comp = st.GetComponent<Static>();
		if (comp != null) {
			comp.x = x;
			comp.y = y;
			comp.type = id;
			comp.foreground = fg;
			comp.Initialise();
		}
	}
	
	void AddEvent(int id,int x,int y) {
		string n = string.Empty;
		int type = 0;
		switch (id) {
			case 1: n = "player"; break;
			case 3: n = "lever"; break;
			case 4: n = "pressure"; break;
			case 9: n = "enemy"; break;
			default:
				if (id == 10 || id == 11) {
					n = "door";
					type = id-10;
				} else if (id >= 16 && id <= 19) {
					n = "portal";
					switch (id) {
						case 16: type = 0; break;
						case 17: type = 2; break;
						case 18: type = 3; break;
						case 19: type = 1; break;
					}
				} else if (id >= 39 && id <= 50) {
					n = "item";
					type = id-39;
				} else if (id >= 51 && id <= 56) {
					n = "culture";
					type = id-51;
				} else if (id >= 57 && id <= 62) {
					n = "friend";
					type = id-57;
				} else {
					Debug.Log("warning! couldn't find an event prefab with id #"+id+"!");
					return;
				}
				break;
		}
		GameObject prefab;
		if (!evPrefab.TryGetValue(n,out prefab)) {
			prefab = Resources.Load<GameObject>("events/"+n+"/"+n);
			if (prefab == null) {
				Debug.Log("warning! event prefab \""+n+"\" doesn't exist!");
				return;
			}
			evPrefab.Add(n,prefab);
		}
		var st = Instantiate<GameObject>(prefab).transform;
		st.name = n;
		st.parent = eventTr;
		var comp = st.GetComponent<Event>();
		if (comp != null) {
			comp.x = x;
			comp.y = y;
			comp.type = type;
			comp.tr = st;
			comp.col = st.GetComponent<Collider2D>();
			comp.id = currentLevel+" "+events.Count;
			events.Add(comp);
		}
		var pl = st.GetComponent<Player>();
		if (pl != null) player = pl;
	}
	
	void AddCollision(int x,int y) {
		if (coPrefab == null) {
			coPrefab = Resources.Load<GameObject>("prefabs/collision");
		}
		var co = Instantiate<GameObject>(coPrefab).transform;
		co.name = "collision";
		co.parent = collisionTr;
		co.localScale = Vector3.one;
		co.localPosition = new Vector3(x,y,0);
		co.localRotation = Quaternion.identity;
	}
	
	void Update() {
		UpdateCamera();
		float s = cam.orthographicSize;
		var cursorPos = cam.ScreenToWorldPoint(Input.mousePosition);
		cursorX = cursorPos.x;
		cursorY = cursorPos.y;
		cursorTr.position = new Vector3(cursorX,cursorY,-9);
		cursorAbsX = (cursorX-camTr.localPosition.x)/(s*2*Window.prop)+.5f;
		cursorAbsY = (cursorY-camTr.localPosition.y)/(s*2)+.5f;
		const float radius = 2;
		onRadius = radius*radius > (new Vector2(player.x-cursorX,player.y-cursorY)).sqrMagnitude;
		if (onRadius) {
			cursorRend.material.color = new Color(0,0,0,.6f+.1f*Mathf.Sin(Time.time*6));
		} else {
			cursorRend.material.color = new Color(.8f,0,0,.8f+.1f*Mathf.Sin(Time.time*8));
		}
		cursorRend.material.mainTexture = cursorTex[(int)(Time.time*6)%3];
		float red = 0;
		if (health < 50) {
			red = (1+Mathf.Sin(Time.time*6))*.5f/Mathf.Max(1,health/20f);
		}
		headCorner.material.color = new Color(red,0,0,
			Mathf.Lerp(headCorner.material.color.a,(cursorAbsX > .7f && cursorAbsY < .35f)?.08f:1,Time.deltaTime*10)
		);
		if (cursorAbsX < .05f) {
			hudTr.localPosition = new Vector3(Mathf.Lerp(hudTr.localPosition.x,0,Time.deltaTime*10),hudTr.localPosition.y,hudTr.localPosition.z);
		} else {
			hudTr.localPosition = new Vector3(Mathf.Lerp(hudTr.localPosition.x,-4.5f,Time.deltaTime*10),hudTr.localPosition.y,hudTr.localPosition.z);
		}
		if (player.headTalk > 0) {
			headCorner.material.mainTexture = headTalk[(int)(player.headTalk*6)%3];
		} else if (player.pressTempo > 0) {
			int f;
			if (player.pressTempo > .4f) {
				f = 1;
			} else if (player.pressTempo > .2f) {
				f = 2;
			} else if (player.pressTempo > .1f) {
				f = 1;
			} else {
				f = 0;
			}
			headCorner.material.mainTexture = headPress[f];
		} else if (player.culture > 0) {
			headCorner.material.mainTexture = headCulture[(int)(Time.time*6)%3];
		} else {
			headCorner.material.mainTexture = headIdle[(int)(Time.time*6)%3];
		}
		if (health < 100) {
			health += Time.deltaTime*.5f;
			if (health > 100) health = 100;
		}
		if (peace < 100) {
			peace += Time.deltaTime*.5f;
			if (peace > 100) peace = 100;
		}
		hudProgress[0].Set(health);
		hudProgress[1].Set(peace);
		hudProgress[2].Set(exploration);
		hudProgress[3].Set(power);
		hudProgress[4].Set(wealth);
		hudProgress[5].Set(culture);
		hudProgress[6].Set(penitence);
		hudProgress[7].Set(friendship);
	}
	
	void UpdateCamera(bool force = false) {
		float h = cam.orthographicSize;
		float w = h*Window.prop;
		float x = player.x;
		float y = player.y;
		if (w*2 < width) {
			x = Mathf.Clamp(x,w-.5f,width-w-.5f);
		} else {
			x = width*.5f-.5f;
		}
		if (h*2 < height) {
			y = Mathf.Clamp(y,h-.5f,height-h-.5f);
		} else {
			y = height*.5f-.5f;
		}
		if (force) {
			camTr.localPosition = new Vector3(x,y,0);
		} else {
			camTr.localPosition = Vector3.Lerp(camTr.localPosition,new Vector3(x,y,0),Time.deltaTime*8);
		}
	}
	
	public bool Collide(float x0,float y0,float x1,float y1,Event ev,out float nx,out float ny) {
		nx = x0;
		ny = y0;
		int collide = 0;
		bool en = false;
		if (ev.col) {
			en = ev.col.enabled;
			ev.col.enabled = false;
		}
		if (!CollideInternal(nx,ny,x1,ny,ev)) {
			nx = x1;
		} else {
			collide++;
		}
		if (!CollideInternal(nx,ny,nx,y1,ev)) {
			ny = y1;
		} else {
			collide++;
		}
		if (en) ev.col.enabled = true;
		return collide > 1;
	}
	
	bool CollideInternal(float x0,float y0,float x1,float y1,Event ev) {
		var dir = new Vector2(x1-x0,y1-y0);
		float dist = Mathf.Max(Mathf.Abs(dir.x),Mathf.Abs(dir.y));
		float lerp = .025f/dist;
		float px = Utils.LerpFree(x0,x1,lerp);
		float py = Utils.LerpFree(y0,y1,lerp);
		return Physics2D.BoxCast(
			new Vector2(px,py),new Vector2(ev.width,ev.height),
			ev.tr.localRotation.eulerAngles.z,dir,dist
		).collider != null;
	}
	
	public void LeaveRoom(int f) {
		currentRoute = MapReader.GetRoute(currentLevel,f);
		if (currentRoute.facing < 0) return;
		requestedLevel = currentRoute.dest;
		useRoute = true;
		SaveState();
		Game.LoadScene("level");
	}
	
	public void SaveState() {
		for (int a = 0; a < events.Count; a++) {
			if (events[a] == player) continue;
			eventProps[events[a].id] = new EventProps(events[a]);
		}
	}
}