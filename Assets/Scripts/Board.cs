using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spewnity;

public class Board : MonoBehaviour
{
	public static string replay = "";

	public GameObject tilePrefab;
	public TerrainType[] terrainTypes;
	public Map<int> terrain;
	public int rows = 10;
	public int cols = 16;
	public float tileWidth = 50f;
	public float tileHeight = 58f;
	public string contents = "";
	public int villagesFound = 0;
	public bool editMode = true;

	void Awake()
	{
		terrain = new Map<int>(cols, rows, 0);
	}

	void Start()
	{
		if(TransitionToScene.data == "random")
		{
			if(replay == "")
			{
				randomizeMap();
				serializeContents();
				replay = contents;
			}
			else
			{
				contents = replay;
				deserializeContents();
			}
		}
		else
		{
			if(contents == "") serializeContents();
			else deserializeContents();
			replay = "";
		}
		updateTiles();
	}

	public void serializeContents()
	{
		contents = "";
		terrain.EachPosition((x, y) => contents += terrain[x, y].ToString(), (int) Map<int>.Traversal.YFirst);
		updateVillageCount();
	}

	public void deserializeContents()
	{
		Debug.Assert(contents.Length == terrain.width * terrain.height, 
			"Expected contents length of " + (terrain.width * terrain.height) + " but found " + contents.Length);
		terrain.EachPosition((x, y) => terrain[x, y] = int.Parse(contents[getContentsPosition(x, y)].ToString()));
		updateVillageCount();
	}

	public void updateVillageCount()
	{
		villagesFound = 0;
		terrain.EachItem((int i) =>
		{
			if(terrainTypes[i].isVillage) villagesFound++;
		});
	}

	// If sorted, six results are returned, which may include nulls
	// Otherwise there may be fewer than six if at map edges
	public List<Tile> getNeighbors(int x, int y, bool isSorted = false)
	{
		List<Tile> neighbors = new List<Tile>();

		int[] centerOffsets = new int[] { -1, 1 };
		int[] oddOffsets = new int[] { -1, 0 };
		int[] evenOffsets = new int[] { 0, 1 };

		int ox = 0;
		for(int oy = -1; oy <= 1; oy++) for(int el = 0; el <= 1; el++)
			{
				if(oy == 0) ox = centerOffsets[el];
				else if(y % 2 == 0) ox = evenOffsets[el];
				else ox = oddOffsets[el];
				
				Tile tile = getTileAt(x + ox, y + oy);
				if(tile != null || isSorted) neighbors.Add(tile);
			}

		if(!isSorted) return neighbors;

		List<Tile> sorted = new List<Tile>();
		sorted.Add(neighbors[5]);
		sorted.Add(neighbors[3]);
		sorted.Add(neighbors[1]);
		sorted.Add(neighbors[0]);
		sorted.Add(neighbors[2]);
		sorted.Add(neighbors[4]);
		return sorted;		
	}

	public Tile getTileAt(int x, int y)
	{
		GameObject tileGO = GameObject.Find(getNameFor(x, y));
		if(tileGO == null) return null;
		return tileGO.GetComponent<Tile>();
	}

	public void setGlow(int x, int y, bool active)
	{
		GameObject tileGO = GameObject.Find(getNameFor(x, y));
		Tile tile = tileGO.GetComponent<Tile>();
		tile.hasGlow = active;
		GameObject glowGO = tileGO.GetChild("Glow");
		glowGO.SetActive(tile.hasGlow);
	}

	public void clearGlow()
	{
		terrain.EachPosition((x, y) => setGlow(x, y, false));
	}

	public void setRoad(int x, int y, bool hasRoad)
	{
		GameObject tileGO = GameObject.Find(getNameFor(x, y));
		Tile tile = tileGO.GetComponent<Tile>();
		tile.hasRoad = hasRoad;
		updateRoad(x, y);
	}

	public void updateRoad(int x, int y, bool updateNeighbors = true)
	{
		GameObject tileGO = GameObject.Find(getNameFor(x, y));
		Tile tile = tileGO.GetComponent<Tile>();
		List<Tile> neighbors = getNeighbors(x, y, true);
		int index = 1;
		foreach(Tile n in neighbors)
		{
			GameObject roadGO = tileGO.GetChild("Road" + index++);
			bool showRoad = tile.hasRoad;
			if(tile.type.isVillage) showRoad = false;
			if(n == null || !(n.hasRoad || n.type.isVillage)) showRoad = false;				
			roadGO.SetActive(showRoad);

			if(showRoad && updateNeighbors) updateRoad(n.x, n.y, false);
		}
	}

	public int getContentsPosition(int x, int y)
	{
		return x + y * cols;
	}

	public void updateTiles()
	{
		if(terrain == null) return;
		
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Tile")) GameObject.Destroy(go);
		terrain.EachPosition((x, y) => spawnTerrain(x, y));
	}

	public void setTile(int x, int y, int terrainId)
	{
		Debug.Assert(terrainId >= 0, "Expected terrainId >= 0 but found " + terrainId);
		Debug.Assert(terrainId < terrainTypes.Length, "Expected terrainId < " + terrainTypes.Length + " but found " + terrainId);
		terrain[x, y] = terrainId;
		serializeContents();
		updateTile(x, y);
	}

	// Creates a new tile
	private void spawnTerrain(int x, int y)
	{
		int terrainId = terrain[x, y];
		Debug.Assert(terrainId >= 0, "Expected terrainId >= 0 but found " + terrainId);
		Debug.Assert(terrainId < terrainTypes.Length, "Expected terrainId < " + terrainTypes.Length + " but found " + terrainId);
		GameObject tileGO = Instantiate(tilePrefab);
		tileGO.transform.parent = transform;
		float gox = tileWidth * (x + (y % 2 == 1 ? 0 : 0.5f));
		float goy = tileHeight * y;
		tileGO.transform.localPosition = new Vector2(gox, goy);
		tileGO.name = getNameFor(x, y);
		Tile tile = tileGO.GetComponent<Tile>();
		tile.x = x;
		tile.y = y;
		tile.type = terrainTypes[terrainId];
		updateTile(x, y);
	}

	// Sets the tile's sprite, glow, and roads
	public void updateTile(int x, int y)
	{
		int terrainId = terrain[x, y];
		Sprite spr = terrainTypes[terrainId].sprite;
		GameObject go = GameObject.Find(getNameFor(x, y));
		SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
		sr.sprite = spr;
		Tile tile = go.GetComponent<Tile>();
		GameObject glowGO = go.GetChild("Glow");
		glowGO.SetActive(tile.hasGlow);
		updateRoad(x, y);
	}

	public int getTerrainId(int x, int y)
	{
		return terrain[x, y];
	}

	// Deserializes incorrectly, not worth the refactor, move on
	//	void OnValidate()
	//	{
	//		if(terrain == null) return;
	//		if(contents != priorContents)
	//		{
	//			priorContents = contents;
	//			deserializeContents();
	//			updateTiles();
	//		}
	//	}

	private string getNameFor(int x, int y)
	{
		return "tile-" + x + "-" + y;
	}

	private void randomizeMap()
	{
		int border = 1;

		terrain.EachPosition((x, y) =>
		{
			int tile = 0;
//			if(x >= border && y >= border && x < cols - border && y < rows - border)
//			{
			tile = Random.Range(0, 4);
//			}
			terrain[x, y] = tile;
		});

		List<int> villages = new List<int>();
		int count = 5;
		for(int terrainId = 4; terrainId <= 6; terrainId++)
		{
			count--;
			int adjusted = count + Random.Range(-1, 2);
			for(int i = 0; i < adjusted; i++)
			{
				villages.Add(terrainId);
//				Debug.Log(count);
			}
		}

		while(villages.Count > 0)
		{			
			int x = Random.Range(border, cols - border); 
			int y = Random.Range(border, rows - border); 
			if(y >= 10) continue;
			if(x >= border && y >= border && x < cols - border && y < rows - border)
			{
				int v = villages[0];
				villages.RemoveAt(0);
				terrain[x, y] = v;
			}
		}
	}
}


[System.Serializable]
public class TerrainType
{
	public string name;
	public Sprite sprite;
	public bool isVillage;
	public int roadCost;
	public int income;
}

	