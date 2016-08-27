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

		if(contents == "") serializeContents();
		else deserializeContents();
	}

	void Start()
	{
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
		terrain.EachPosition((x, y) => terrain[x, y] = int.Parse(contents[getContentsPosition(x,y)].ToString()));
	}

	public int getContentsPosition(int x, int y)
	{
		return x + y * cols;
	}

	public void updateTiles()
	{
		if(terrain == null)
			return;
		
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Tile")) GameObject.Destroy(go);
		terrain.EachPosition((x, y) => spawnTerrain(x, y));
	}

	public void setTile(int x, int y, int terrainId)
	{
		Debug.Assert(terrainId >= 0, "Expected terrainId >= 0 but found " + terrainId);
		Debug.Assert(terrainId < terrainTypes.Length, "Expected terrainId < " + terrainTypes.Length + " but found " + terrainId);
		terrain[x,y] = terrainId;
		serializeContents();

		Sprite spr = terrainTypes[terrainId].sprite;
		GameObject go = GameObject.Find(getNameFor(x,y));
		SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
		sr.sprite = spr;
	}

	public int getTile(int x, int y)
	{
		return terrain[x,y];
	}

	void OnValidate()
	{				
		if(terrain == null) return;

		deserializeContents();
		updateTiles();
	}

	private void spawnTerrain(int x, int y)
	{
		int terrainId = terrain[x, y];
		Debug.Assert(terrainId >= 0, "Expected terrainId >= 0 but found " + terrainId);
		Debug.Assert(terrainId < terrainTypes.Length, "Expected terrainId < " + terrainTypes.Length + " but found " + terrainId);
		Sprite spr = terrainTypes[terrainId].sprite;
		GameObject tileGO = Instantiate(tilePrefab);
		tileGO.transform.parent = transform;
		SpriteRenderer sr = tileGO.GetComponent<SpriteRenderer>();
		sr.sprite = spr;
		float gox = tileWidth * (x + (y % 2 == 1 ? 0 : 0.5f));
		float goy = tileHeight * y;
		tileGO.transform.localPosition = new Vector2(gox, goy);
		tileGO.name = getNameFor(x, y);
		Tile tile = tileGO.GetComponent<Tile>();
		tile.x = x;
		tile.y = y;
	}

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
}

	