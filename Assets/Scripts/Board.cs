using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spewnity;

public class Board : MonoBehaviour
{
	public int rows = 10;
	public int cols = 16;
	public GameObject tilePrefab;
	public TerrainType[] terrainTypes;
	public Map<int> terrain;
	public float tileWidth = 50f;
	public float tileHeight = 58f;

	void Awake()
	{
		terrain = new Map<int>(cols, rows, 0);
		terrain.EachPosition((x, y) =>
			terrain[x, y] = Random.Range(0, 6));

		terrain.EachPosition((x, y) => spawnTerrain(x, y, terrain[x, y]));		
	}

	void Start()
	{
	}

	void Update()
	{
	}

	void OnValidate()
	{		
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Tile"))
			GameObject.Destroy(go);
		
		if(terrain != null) terrain.EachPosition((x, y) => updateTerrain(x, y));
	}

	private void spawnTerrain(int x, int y, int terrain)
	{
		Sprite spr = terrainTypes[terrain].sprite;
		GameObject tile = Instantiate(tilePrefab);
		tile.transform.parent = transform;
		SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
		sr.sprite = spr;
		float gox = tileWidth * (x + (y % 2 == 0 ? 0 : 0.5f));
		float goy = tileHeight * y;
		tile.transform.localPosition = new Vector2(gox, goy);
		tile.name = getNameFor(x, y);

	}

	private string getNameFor(int x, int y)
	{
		return "tile-" + x + "-" + y;
	}

	private void updateTerrain(int x, int y)
	{
		GameObject.Destroy(GameObject.Find(getNameFor(x, y)));
		spawnTerrain(x, y, terrain[x, y]);
	}
}


[System.Serializable]
public class TerrainType
{
	public string name;
	public Sprite sprite;
}

	