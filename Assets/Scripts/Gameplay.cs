// TODO The game prevents you from connected 2 villages at once, on maps that support it.
// TODO I've scene situations where your bank balance stays at one even though your taxes exceed your income, although it may be more of an issue of jamming the SPACE bar
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Gameplay : MonoBehaviour
{
	private static int STARTING_BANK = 5;
	private static int TAX_RATE = 2;

	private Board board;
	private int _bank;
	private int _income;
	private int _tax;

	public int turn;
	public bool gameOver = false;
	public int roadsPlaced = 0;
	public int villagesConnected = 0;

	public MessageBar messageBar;
	public CoinEvent bankChanged;
	public CoinEvent incomeChanged;
	public CoinEvent taxChanged;

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
		villagesConnected = 0;
		messageBar.setMessage("Network all the villages!");
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.R))
		{
//			if(gameOver)
			SceneManager.LoadScene("Play");
		}

		else if(Input.GetKeyDown(KeyCode.Escape))
		{
			SceneManager.LoadScene("Main");
		}
	}

	public void mouseOver(Tile tile)
	{
		if(gameOver) return;
		if(board.editMode) return;

		if(Input.GetKeyDown(KeyCode.Space))
		{
			nextTurn();
			return;
		}

		// Start drawing road
		if(Input.GetMouseButtonDown(0))
		{
			// Road cannot start on village or replace roads
			if(tile.type.isVillage || tile.hasRoad)
			{
				messageBar.setMessage("That area already has a sufficient road system.");
				return;
			}

			// If first road, must be next to village, otherwise next to existing road or connected village.
			bool allowed = false;
			List<Tile> neighbors = board.getNeighbors(tile.x, tile.y);
			foreach(Tile t in neighbors)
			{
				if(t.type.isVillage && roadsPlaced == 0) allowed = true;
				else if(t.hasRoad) allowed = true;				
			}
			if(!allowed)
			{
				if(roadsPlaced == 0)
					messageBar.setMessage("Your first road must be next to a village.");
				else messageBar.setMessage("That area is not adjacent to your road network.");
				return;
			}
			
			// Verify you can afford it
			int cost = tile.type.roadCost;
			if(bank < cost)
			{
				if(bank <= 0) messageBar.setMessage("You're broke. Hit SPACE to advance to next year.");
				else messageBar.setMessage("It costs " + cost + " coins to build over " + tile.type.name);

				// TODO Flash bank balance and make noise

				return;
			}

			// Charge your account
			bank -= cost;

			// Adjacent villages now have roads and level up
			int incomeGainedThisTurn = 0;
			int villagesConnectedThisTurn = 0;
			foreach(Tile t in neighbors) if(t.type.isVillage)
				{
					t.hasRoad = true;
					if(!t.hasGlow)
					{
					villagesConnected++;
						villagesConnectedThisTurn++;
						incomeGainedThisTurn += t.type.income;
						income += t.type.income;
						board.setGlow(t.x, t.y, true);
					}
				}

			// Finally, place the road
			board.setRoad(tile.x, tile.y, true);
			roadsPlaced++;

			if(villagesConnected >= board.villagesFound)
			{
				messageBar.setMessage("You did it! You're a master trader!");
				gameOver = true;
			}
			else if(roadsPlaced == 1) messageBar.setMessage("Your first village gives you an income of " + incomeGainedThisTurn);
			else if(incomeGainedThisTurn > 0) messageBar.setMessage((villagesConnectedThisTurn == 1 ? "That village has" : 
				"Those villages have") + " raised your income by " + incomeGainedThisTurn);
				
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
		int net = income - tax;
		turn++;
		bank += income;
		bank -= tax;
		tax += TAX_RATE;
		if(bank < 0)
		{
			messageBar.setMessage("Taxed out of business. You connected " + villagesConnected + 
				(villagesConnected == 1 ? " village." : " villages."));
			gameOver = true;
			// TODO Show gameover screen
		}
		else if(net >= 0)
			messageBar.setMessage("Year Summary: You gained " + (net == 1 ? "one coin" : net + " coins") + " after taxes.");
		else messageBar.setMessage("Year Summary: You lost " + (net == -1 ? "one coin" : -net + " coins") + " after taxes");
	}
}

[System.Serializable]
public class CoinEvent: UnityEvent<int>
{
}
