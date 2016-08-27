using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spewnity;

public class Board : MonoBehaviour
{
	public GameObject tilePrefab;
	public TerrainType[] terrainTypes;
	public Map<int> terrain;
	public int rows = 10;
	public int cols = 16;
	public float tileWidth = 50f;
	public float tileHeight = 58f;
	public string contents = "";
	public bool editMode = true;

	void Awake()
	{
		terrain = new Map<int>(cols, rows, 0);
	}

	void Start()
	{
		if(contents == "") serializeContents();
		else deserializeContents();
		updateTiles();
	}

	void Update()
	{
	}

	public void serializeContents()
	{
		contents = "";
		terrain.EachPosition((x, y) => contents += terrain[x, y].ToString(), (int) Map<int>.Traversal.YFirst);
	}

	public void deserializeContents()
	{
		Debug.Assert(contents.Length == terrain.width * terrain.height, 
			"Expected contents length of " + (terrain.width * terrain.height) + " but found " + contents.Length);
		terrain.EachPosition((x, y) => terrain[x, y] = int.Parse(contents[getContentsPosition(x, y)].ToString()));
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
		tile.glow = active;
		GameObject glowGO = tileGO.GetChild("Glow");
		glowGO.SetActive(tile.glow);
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
			if(tile.isVillage) showRoad = false;
			if(n == null || !n.hasRoad) showRoad = false;				
			roadGO.SetActive(showRoad);

			if(showRoad && updateNeighbors)
				updateRoad(n.x, n.y, false);
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
		tile.terrainType = terrainId;
		tile.hasRoad = tile.isVillage = terrainTypes[terrainId].isVillage;
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
		glowGO.SetActive(tile.glow);
		updateRoad(x, y);
	}

	public int getTerrainId(int x, int y)
	{
		return terrain[x, y];
	}

	void OnValidate()
	{				
		if(terrain == null) return;

		deserializeContents();
		updateTiles();
	}

	//	public string getNameForTerrainId(int terrainId)
	//	{
	//		return terrainTypes[terrainId].name;
	//	}

	private string getNameFor(int x, int y)
	{
		return "tile-" + x + "-" + y;
	}
}


[System.Serializable]
public class TerrainType
{
	public string name;
	public Sprite sprite;
	public bool isVillage;
}

	