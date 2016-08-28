using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
	public Text coinText;
	public bool drawingRoad = false;
	public int coins = 0;
	public int turn;
	public int income;
	public int taxes;
	public bool gameOver = false;
	public int roadsPlaced = 0;

	private Board board;

	void Awake()
	{
		board = GameObject.Find("Board").GetComponent<Board>();
		Tile.onMouseOver.AddListener(this.mouseOver);
	}

	void Start()
	{
		coins = 10;
		adjustCoins(0);

		taxes = 1;
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
			
			// Adjacent villages now have roads and level up
			foreach(Tile t in neighbors) if(t.type.isVillage)
				{
					t.hasRoad = true;
					board.setGlow(t.x, t.y, true);
				}

			// Incur the cost
			int cost = tile.type.cost;
			if(adjustCoins(-cost) == false) return;

			// Place road
			board.setRoad(tile.x, tile.y, true);
			roadsPlaced++;
		}
	}

	// Pass negative to spend coins
	// Returns false if you can't cover the cost
	public bool adjustCoins(int amount)
	{
		int newSum = coins + amount;
		if(newSum < 0)
		{
			Debug.Log("Out of ancient coin tech.");
			return false;
		}

		coins = newSum;

		coinText.text = coins.ToString();

		return true;
	}

	public void nextTurn()
	{
		turn++;
		adjustCoins(income);
		if(adjustCoins(taxes) == false)
		{
			gameOver = true;
			// TODO Show gameover screen
		}
		taxes++;
	}
}
	