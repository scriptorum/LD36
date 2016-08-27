using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEditor : MonoBehaviour
{
	Board board;

	void Awake()
	{
		board = GameObject.Find("Board").GetComponent<Board>();
		Tile.onMouseOver.AddListener(this.mouseOver);
	}

	void Start()
	{
	}

	void Update()
	{
	}

	private void mouseOver(Tile tile)
	{
		if(!board.editMode) return;

		for(int i = 0; i < board.terrainTypes.Length; i++)
		{
			if(Input.GetKey(KeyCode.Alpha0 + i))
			{
				if(i != board.getTerrainId(tile.x, tile.y)) board.setTile(tile.x, tile.y, i);
				break;
			}
		}

		if(Input.GetKeyDown(KeyCode.L))
		{
			tile.glow = !tile.glow;
			board.updateTile(tile.x, tile.y);
		}
	}
}

