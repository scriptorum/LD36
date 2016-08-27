﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public Board board;
	public int x;
	public int y;
	public int level = 0;

	void Awake()
	{
		board = GameObject.Find("Board").GetComponent<Board>();
	}

	void Start()
	{
	}

	void Update()
	{
	}

	void OnMouseOver()
	{
		if(!board.editMode) return;

		for(int i = 0; i < board.terrainTypes.Length; i++)
		{
			if(Input.GetKey(KeyCode.Alpha0 + i))
			{
				if(i != board.getTile(x,y))
					board.setTile(x, y, i);
				break;
			}
		}

		if(Input.GetKey(KeyCode.L))
		{
			level = (++level % 2);
			board.updateTile(x,y);
		}
	}
}

