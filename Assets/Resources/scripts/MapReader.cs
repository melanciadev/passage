using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MapReader {
	public void Read(string id) {
		var text = Resources.Load<TextAsset>("maps/"+id).text;
		using (var reader = XmlReader.Create(new StringReader(text))) {
			Read(reader);
		}
	}
	
	public void Read(XmlReader xml) {
		var lvl = Level.me;
		var charComma = new char[] {','};
		var charPoints = new char[] {' ',','};
		
		int tileWidth = 0;
		var tiles = new SortedDictionary<int,int2>();
		bool foreground = false;
		int width = 32,height = 32;
		float ix = 0,iy = 0;
		
		string s;
		int w,h,id;
		float ww,hh;
		
		while (xml.Read()) {
			if (xml.NodeType == XmlNodeType.Element) {
				switch (xml.Name) {
					case "map":
						if (!GetInt(xml,"width",out w) || !GetInt(xml,"height",out h)) continue;
						lvl.width = w;
						lvl.height = h;
						if (!GetInt(xml,"tilewidth",out w) || !GetInt(xml,"tileheight",out h)) continue;
						width = w;
						height = h;
						lvl.tileBg = new int[lvl.width,lvl.height];
						lvl.tileEv = new int[lvl.width,lvl.height];
						lvl.tileFg = new int[lvl.width,lvl.height];
						lvl.tileCo = new int[lvl.width,lvl.height];
						lvl.wires = new List<int4>();
						break;
					case "tileset":
						if (!GetString(xml,"name",out s)) continue;
						switch (s) {
							case "st": id = 0; break;
							case "ev": id = 1; break;
							case "co": id = 2; break;
							default: id = -1; break;
						}
						if (id < 0) continue;
						int start,count;
						if (!GetInt(xml,"firstgid",out start) || !GetInt(xml,"tilecount",out count)) continue;
						tiles[start] = new int2(id,start+count);
						break;
					case "layer":
						if (!GetInt(xml,"width",out w) || !GetString(xml,"name",out s)) continue;
						tileWidth = w;
						s = s.Trim().ToLower();
						switch (s) {
							case "e":
							case "ev":
							case "event":
							case "events":
								foreground = true;
								break;
						}
						break;
					case "object":
						if (!GetFloat(xml,"x",out ww) || !GetFloat(xml,"y",out hh)) continue;
						ix = ww;
						iy = hh;
						break;
					case "polyline":
						if (!GetString(xml,"points",out s)) continue;
						var pos = s.Split(charPoints,System.StringSplitOptions.RemoveEmptyEntries);
						if (pos.Length < 4 || pos.Length%2 != 0) continue;
						float x0,y0,x1,y1;
						if (!float.TryParse(pos[0],out x0) || !float.TryParse(pos[1],out y0)) continue;
						if (!float.TryParse(pos[pos.Length-2],out x1) || !float.TryParse(pos[pos.Length-1],out y1)) continue;
						var p = new int4(
							Mathf.FloorToInt((x0+ix)/width),
							lvl.height-1-Mathf.FloorToInt((y0+iy)/height),
							Mathf.FloorToInt((x1+ix)/width),
							lvl.height-1-Mathf.FloorToInt((y1+iy)/height)
						);
						if (p.x0 < 0 || p.x0 >= lvl.width || p.x1 < 0 || p.x1 >= lvl.width) continue;
						if (p.y0 < 0 || p.y0 >= lvl.height || p.y1 < 0 || p.y1 >= lvl.height) continue;
						if (p.x0 == p.x1 && p.y0 == y1) continue;
						lvl.wires.Add(p);
						break;
				}
			} else if (tileWidth > 0 && xml.NodeType == XmlNodeType.Text) {
				var asd = xml.Value.Split(charComma,System.StringSplitOptions.RemoveEmptyEntries);
				int x,y,t;
				int gid;
				for (int a = 0; a < asd.Length; a++) {
					x = a%tileWidth;
					y = a/tileWidth;
					if (x >= lvl.width || y >= lvl.height) continue;
					if (!int.TryParse(asd[a],out t) || t <= 0) continue;
					gid = -1;
					foreach (var item in tiles) {
						if (t >= item.Key && t < item.Value.y) {
							gid = item.Value.x;
							t = t-item.Key+1;
							break;
						}
					}
					if (gid == 0) {
						if (foreground) {
							lvl.tileFg[x,lvl.height-1-y] = t;
						} else {
							lvl.tileBg[x,lvl.height-1-y] = t;
						}
					} else if (gid == 1) {
						lvl.tileEv[x,lvl.height-1-y] = t;
					} else if (gid == 2) {
						lvl.tileCo[x,lvl.height-1-y] = t;
					}
				}
				tileWidth = 0;
			}
		}
	}
	
	bool GetString(XmlReader xml,string n,out string val) {
		val = xml.GetAttribute(n);
		return val != null;
	}
	
	bool GetInt(XmlReader xml,string n,out int val) {
		string s = xml.GetAttribute(n);
		if (s == null) {
			val = 0;
			return false;
		}
		return int.TryParse(s,out val);
	}
	
	bool GetFloat(XmlReader xml,string n,out float val) {
		string s = xml.GetAttribute(n);
		if (s == null) {
			val = 0;
			return false;
		}
		return float.TryParse(s,out val);
	}
	
	public static void DebugTiles(int[,] asd) {
		int w = asd.GetLength(0);
		int h = asd.GetLength(1);
		string aaa = "";
		for (int y = 0; y < h; y++) {
			for (int x = 0; x < w; x++) {
				aaa += asd[x,y]+",";
			}
			aaa += '\n';
		}
		Debug.Log(aaa);
	}
	
	static Dictionary<string,List<Route>> routes;
	
	public static void SetRouteList() {
		routes = new Dictionary<string,List<Route>>();
		//AddRoute(2,"asd","dsa");
		AddRoute(0,"C","N1");
		AddRoute(1,"N1","N2");
		AddRoute(0,"O1","N2");
		AddRoute(0,"N2","N3");
		AddRoute(0,"N3","N4");
		AddRoute(3,"N1","L2");
		AddRoute(0,"L2","N6");
		AddRoute(3,"N3","N5");
		AddRoute(3,"N5","N6");
		AddRoute(0,"N5","PN");
		AddRoute(3,"C","L1");
		AddRoute(0,"L1","L2");
		AddRoute(3,"L2","L3");
		AddRoute(3,"L3","L4");
		AddRoute(2,"L3","L5");
		AddRoute(3,"L5","PL");
		AddRoute(2,"L5","L6");
		AddRoute(2,"C","S1");
		AddRoute(3,"S1","S2");
		AddRoute(3,"S2","L6");
		AddRoute(0,"S2","L1");
		AddRoute(2,"S2","S3");
		AddRoute(2,"S3","S4");
		AddRoute(1,"S3","S5");
		AddRoute(2,"S5","PS");
		AddRoute(1,"S5","S6");
		AddRoute(1,"C","O1");
		AddRoute(2,"O1","O2");
		AddRoute(3,"O2","S1");
		AddRoute(2,"O2","S6");
		AddRoute(1,"O2","O3");
		AddRoute(1,"O3","O4");
		AddRoute(0,"O3","O5");
		AddRoute(0,"O5","O6");
		AddRoute(1,"O5","PO");
		AddRoute(3,"O6","N2");
		/*
		AddRoute(0,"C","N1");
		AddRoute(1,"C","O1");
		AddRoute(2,"C","S1");
		AddRoute(3,"C","L1");
		AddRoute(0,"O1","O2");
		AddRoute(1,"O2","O3");
		AddRoute(0,"O3","O4");
		AddRoute(1,"O4","O5");
		AddRoute(2,"O5","O6");
		AddRoute(2,"O6","O7");
		AddRoute(1,"O7","PO");
		AddRoute(1,"O6","M1");
		AddRoute(0,"O2","M5");
		AddRoute(1,"M5","O4");
		AddRoute(1,"O1","M3");
		AddRoute(1,"M3","M2");
		AddRoute(2,"M3","M4");
		AddRoute(3,"M4","S1");
		AddRoute(2,"S1","S2");
		AddRoute(2,"S2","S3");
		AddRoute(1,"S3","S4");
		AddRoute(0,"S4","S5");
		AddRoute(1,"S5","S6");
		AddRoute(2,"S6","S7");
		AddRoute(3,"S7","S8");
		AddRoute(3,"S8","S9");
		AddRoute(3,"S9","S10");
		AddRoute(2,"S10","PS");
		AddRoute(0,"S10","M6");
		AddRoute(3,"N1","N2");
		AddRoute(0,"N2","N3");
		//AddRoute(1,"N3","O2",1);
		AddRoute(1,"N3","N4");
		AddRoute(0,"N4","N5");
		AddRoute(0,"N5","N6");
		AddRoute(3,"N6","N7");
		AddRoute(0,"N7","N8");
		AddRoute(1,"N8","PN");
		AddRoute(2,"L1","M7");
		AddRoute(3,"L1","L2");
		AddRoute(0,"L2","N2");
		AddRoute(2,"L2","L3");
		AddRoute(2,"L3","L4");
		AddRoute(3,"L4","L5");
		AddRoute(0,"L5","L6");
		AddRoute(0,"L6","M9");
		AddRoute(3,"M9","M11");
		AddRoute(3,"L6","M10");
		AddRoute(0,"M10","M11");
		AddRoute(0,"L6","L7");
		AddRoute(0,"L7","L8");
		AddRoute(0,"L8","L9");
		//AddRoute(0,"L9","M8",0,1);
		AddRoute(0,"N3","M8");
		AddRoute(3,"L9","L10");
		AddRoute(2,"L10","L11");
		AddRoute(3,"L11","L12");
		AddRoute(3,"L12","PL");
		//*/
		/*
		var item = routes["M8"];
		string asd = "M8";
		for (int a = 0; a < item.Count; a++) {
			asd += "\n> ["+item[a].facing+"/"+item[a].index0+"] "+item[a].dest+" ("+item[a].index1+")";
		}
		Debug.Log(asd);
		foreach (var item in routes) {
			string asd = item.Key;
			for (int a = 0; a < item.Value.Count; a++) {
				asd += "\n> ["+item.Value[a].facing+"/"+item.Value[a].index0+"] "+item.Value[a].dest+" ("+item.Value[a].index1+")";
			}
			Debug.Log(asd);
		}
		*/
	}
	
	static void AddRoute(int facing,string from,string to) {
		List<Route> list;
		if (!routes.TryGetValue(from,out list)) {
			list = new List<Route>();
			routes.Add(from,list);
		}
		list.Add(new Route(facing,to));
		if (!routes.TryGetValue(to,out list)) {
			list = new List<Route>();
			routes.Add(to,list);
		}
		list.Add(new Route((facing+2)%4,from));
	}
	
	public static Route GetRoute(string from,int facing) {
		List<Route> r;
		if (!routes.TryGetValue(from,out r)) return new Route();
		for (int a = 0; a < r.Count; a++) {
			if (r[a].facing == facing) {
				return r[a];
			}
		}
		return new Route();
	}
}

public struct Route {
	public int facing;
	public string dest;
	
	public Route(bool asd = false) {
		facing = -1;
		dest = string.Empty;
	}
	
	public Route(int facing,string dest) {
		this.facing = facing;
		this.dest = dest;
	}
}