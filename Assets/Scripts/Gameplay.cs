using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
	public Text coinText;
	public bool drawingRoad = false;
	public int coins = 0;

	private Board board;

	void Awake()
	{
		board = GameObject.Find("Board").GetComponent<Board>();
		Tile.onMouseOver.AddListener(this.mouseOver);
	}

	void Start()
	{
		coins = 20;
	}

	void Update()
	{
	}

	public void mouseOver(Tile tile)
	{
		// VERIFY NEIGHBORS TEST
		if(Input.GetMouseButtonDown(1))
		{
			// Reset glow on all tiles
			board.clearGlow();

			// Get all tile neighbors
			List<Tile> neighbors = board.getNeighbors(tile.x, tile.y);

			// Set glow on these tiles
			foreach(Tile n in neighbors) board.setGlow(n.x, n.y, true);
		}

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
			if(adjustCoins(-tile.terrainType.cost) == false)
				return;
			
			// Road cannot start on village, villages are assumed to have roads
			if(tile.terrainType.isVillage)
			{
				// TODO This is not a village, but does it have a road?
				return;
			}
				
//			drawingRoad = true;

			// Place road
			board.setRoad(tile.x, tile.y, true);
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
}
	