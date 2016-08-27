using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
	public bool drawingRoad = false;
	private Board board;

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

	public void mouseOver(Tile tile)
	{
//		// VERIFY NEIGHBORS TEST
//		if(Input.GetMouseButtonDown(0))
//		{
//			// Reset glow on all tiles
//			board.clearGlow();
//
//			// Get all tile neighbors
//			List<Tile> neighbors = board.getNeighbors(tile.x, tile.y);
//
//			// Set glow on these tiles
//			foreach(Tile n in neighbors)
//				board.setGlow(n.x, n.y, true);
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
			// Verify road can start here
			if(board.getNameForTerrainId(tile.terrainType) != "village")
			{
				// TODO This is not a village, but does it have a road?
				return;
			}
				
			drawingRoad = true;

			// TODO Place first road and start it
		}

	}
}

