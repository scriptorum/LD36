using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gameplay : MonoBehaviour
{
	private static int STARTING_BANK = 10;

	private Board board;
	private int _bank;
	private int _income;
	private int _tax;

	public CoinEvent bankChanged;
	public CoinEvent incomeChanged;
	public CoinEvent taxChanged;

	public bool drawingRoad = false;
	public int turn;
	public bool gameOver = false;
	public int roadsPlaced = 0;

	void Awake()
	{
		board = GameObject.Find("Board").GetComponent<Board>();
		Tile.onMouseOver.AddListener(this.mouseOver);
	}

	void Start()
	{
		bank = STARTING_BANK;
		tax = 1;
		income = 0;
	}

	void Update()
	{
	}

	public void mouseOver(Tile tile)
	{
		if(gameOver) return;
		
		if(Input.GetKeyDown(KeyCode.Space))
		{
			nextTurn();
			return;
		}
			
//		// VERIFY NEIGHBORS TEST
//		if(Input.GetMouseButtonDown(1))
//		{
//			// Reset glow on all tiles
//			board.clearGlow();
//
//			// Get all tile neighbors
//			List<Tile> neighbors = board.getNeighbors(tile.x, tile.y);
//
//			// Set glow on these tiles
//			foreach(Tile n in neighbors) board.setGlow(n.x, n.y, true);
//		}

		// While drawing road
		if(drawingRoad)
		{
			// Complete road
			if(Input.GetMouseButtonUp(0))
			{
				// TODO Affix roads, adjust cities, get points
				drawingRoad = false;
			}

			// Cancel road
			else if(Input.GetMouseButtonDown(1))
			{
				// TODO Remove roads
				drawingRoad = false;
			}
		}

		// Start drawing road
		else if(Input.GetMouseButtonDown(0))
		{
			// Road cannot start on village or replace roads
			if(tile.type.isVillage || tile.hasRoad) return;

			// If first road, must be next to village, otherwise next to existing road or connected village.
			bool allowed = false;
			List<Tile> neighbors = board.getNeighbors(tile.x, tile.y);
			foreach(Tile t in neighbors)
			{
				if(t.type.isVillage && roadsPlaced == 0) allowed = true;
				else if(t.hasRoad) allowed = true;				
			}
			if(!allowed) return;
			
			// Verify you can afford it
			int cost = tile.type.cost;
			if(bank < cost)
			{
				Debug.Log("Not enough ancient coin tech.");
				return;
			}

			// Charge your account
			bank -= cost;

			// Adjacent villages now have roads and level up
			foreach(Tile t in neighbors) if(t.type.isVillage)
				{
					t.hasRoad = true;
					if(!t.hasGlow)
					{
						income++;
						board.setGlow(t.x, t.y, true);
					}
				}
				
			// Finnaly, place the road
			board.setRoad(tile.x, tile.y, true);
			roadsPlaced++;
		}
	}

	public int bank
	{
		get { return  _bank; }
		set
		{ 
			_bank = value;
			bankChanged.Invoke(_bank);
		}
	}

	public int income
	{
		get { return  _income; }
		set
		{ 
			_income = value;
			incomeChanged.Invoke(_income);
		}
	}

	public int tax
	{
		get { return  _tax; }
		set
		{ 
			_tax = value;
			taxChanged.Invoke(_tax);
		}
	}

	public void nextTurn()
	{
		turn++;
		bank += income;
		bank -= tax++;
		if(bank < 0)
		{
			gameOver = true;
			Debug.Log("Game over!");
			// TODO Show gameover screen
		}
	}
}

[System.Serializable]
public class CoinEvent: UnityEvent<int>
{
}
