﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
	public static TileEvent onMouseOver = new TileEvent();

	public int x;
	public int y;
	public bool hasGlow = false;
	public bool hasRoad = false;
	public TerrainType type;

	void Awake()
	{
	}

	void Start()
	{
	}

	void Update()
	{
	}

	void OnMouseOver()
	{
		Tile.onMouseOver.Invoke(this);
	}
}

public class TileEvent : UnityEvent<Tile>
{
}

