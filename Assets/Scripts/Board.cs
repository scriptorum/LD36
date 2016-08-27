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

	void Awake()
	{
		terrain = new Map<int>(cols, rows, 0);
		int order = (int) Map<int>.Traversal.YFirst;

		if(contents == "")
		{
			terrain.EachPosition((x, y) =>
			{
				terrain[x, y] = Random.Range(0, terrainTypes.Length);
				contents += terrain[x, y].ToString();
			}, order
			);

		}
		else deserializeContents();
		
		terrain.EachPosition((x, y) => spawnTerrain(x, y));		
	}

	public void deserializeContents()
	{
		Debug.Log("Deserializing " + contents);
		Debug.Log("Terrain size:" + terrain.width + "x" + terrain.height);
		Debug.Assert(contents.Length == terrain.width * terrain.height, 
			"Expected contents length of " + (terrain.width * terrain.height) + " but found " + contents.Length);
		terrain.EachPosition((x, y) => terrain[x, y] = int.Parse(contents[x + y * cols].ToString()));
	}

	void Start()
	{
	}

	void Update()
	{
	}

	void OnValidate()
	{				
		if(terrain == null) return;

		deserializeContents();

		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Tile")) GameObject.Destroy(go);
		terrain.EachPosition((x, y) => spawnTerrain(x, y));
	}

	private void spawnTerrain(int x, int y)
	{
		int terrainId = terrain[x, y];
		Debug.Assert(terrainId >= 0, "Expected terrainId >= 0 but found " + terrainId);
		Debug.Assert(terrainId < terrainTypes.Length, "Expected terrainId < " + terrainTypes.Length + " but found " + terrainId);
		Sprite spr = terrainTypes[terrainId].sprite;
		GameObject tile = Instantiate(tilePrefab);
		tile.transform.parent = transform;
		SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
		sr.sprite = spr;
		float gox = tileWidth * (x + (y % 2 == 1 ? 0 : 0.5f));
		float goy = tileHeight * y;
		tile.transform.localPosition = new Vector2(gox, goy);
		tile.name = getNameFor(x, y);
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

	