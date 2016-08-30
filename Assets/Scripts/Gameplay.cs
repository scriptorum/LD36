using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Spewnity;

public class Gameplay : MonoBehaviour
{
	private static int STARTING_BANK = 5;
	private static int TAX_RATE = 2;

	private Board board;
	private int _bank;
	private int _income;
	private int _tax;
	private int _kingsVault;
	private Transaction trans = new Transaction();

	public int turn;
	public bool gameOver = false;
	public int roadsPlaced = 0;
	public int villagesConnected = 0;

	public MessageBar messageBar;
	public CoinEvent bankChanged;
	public CoinEvent incomeChanged;
	public CoinEvent taxChanged;
	public CoinEvent kingsVaultChanged;

	void Awake()
	{
		board = GameObject.Find("Board").GetComponent<Board>();
		Tile.onMouseOver.AddListener(this.mouseOver);
	}

	void Start()
	{
		SoundManager.instance.Stop();
		SoundManager.instance.Play("start");

		bank = STARTING_BANK;
		tax = 1;
		income = 0;
		kingsVault = 0;
		villagesConnected = 0;
		messageBar.setMessage("Network all the villages! [H] for help.");
	}

	void Update()
	{
		if(Input.GetKeyUp(KeyCode.C))
		{
			trans.source = TransactionSource.Bank;
			kingsVault++;
		}
		
		if(Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene("Play");
		}
		else if(Input.GetKeyDown(KeyCode.Escape))
		{
			Board.replay = "";
			SceneManager.LoadScene("Main");
		}
	}

	public void fail(string msg)
	{
		SoundManager.instance.Play("deny");
		messageBar.setMessage(msg);
	}

	public void mouseOver(Tile tile)
	{
		if(gameOver) return;
		if(board.editMode) return;

		if(Input.GetKeyDown(KeyCode.Space))
		{
			nextTurn();
			// TODO SFX
			return;
		}

		// Start drawing road
		if(Input.GetMouseButtonDown(0))
		{
			// Road cannot start on village or replace roads
			if(tile.type.isVillage || tile.hasRoad)
			{
				fail("That area already has a sufficient road system.");
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
				if(roadsPlaced == 0) fail("Your first road must be next to a village.");
				else fail("That area is not adjacent to your road network.");
				return;
			}
			
			// Verify you can afford it
			int cost = tile.type.roadCost;
			if(bank < cost)
			{
				if(bank <= 0) fail("You're broke. Hit SPACE to advance to next year.");
				else fail("It costs " + cost + " coins to build over " + tile.type.name);
				return;
			}

			// Charge your account
			bank -= cost;
			trans.source = TransactionSource.Bank;
			kingsVault += cost;

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
						trans.source = TransactionSource.Custom;
						trans.custom = t.gameObject.transform;
						income += t.type.income;
						t.showKaching();
						SoundManager.instance.Play("village");
						board.setGlow(t.x, t.y, true);
					}
				}

			// Finally, place the road
			board.setRoad(tile.x, tile.y, true);
			roadsPlaced++;

			// Show dust
			tile.showDust();
			SoundManager.instance.Play("build");

			if(villagesConnected >= board.villagesFound)
			{
				SoundManager.instance.Play("win");
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
			trans.amount = value;
			bankChanged.Invoke(trans);
			trans.source = TransactionSource.None;
		}
	}

	public int income
	{
		get { return  _income; }
		set
		{ 
			_income = value;
			trans.amount = value;
			incomeChanged.Invoke(trans);
			trans.source = TransactionSource.None;
		}
	}

	public int tax
	{
		get { return  _tax; }
		set
		{ 
			_tax = value;
			trans.amount = value;
			taxChanged.Invoke(trans);
			trans.source = TransactionSource.None;
		}	
	}

	public int kingsVault
	{
		get { return  _kingsVault; }
		set
		{ 
			_kingsVault = value;
			trans.amount = value;
			kingsVaultChanged.Invoke(trans);
			trans.source = TransactionSource.None;
		}
	}

	public void nextTurn()
	{
		int net = income - tax;
		turn++;
		trans.source = TransactionSource.Income;
		bank += income;
		bank -= tax;
		trans.source = TransactionSource.Bank;
		kingsVault += tax;
		trans.source = TransactionSource.KingsVault;
		tax += TAX_RATE;
		if(bank < 0)
		{
			messageBar.setMessage("Taxed out of business. You connected " + villagesConnected +
			(villagesConnected == 1 ? " village." : " villages."));
			SoundManager.instance.Stop();
			SoundManager.instance.Play("lose");
			gameOver = true;
		}
		else
		{
			if(net >= 0) messageBar.setMessage("Year Summary: You gained " + (net == 1 ? "one coin" : net + " coins") + " after taxes.");
			else messageBar.setMessage("Year Summary: You lost " + (net == -1 ? "one coin" : -net + " coins") + " after taxes");
			SoundManager.instance.Stop();
			SoundManager.instance.Play("year");
		}
	}
}

[System.Serializable]
public class CoinEvent: UnityEvent<Transaction>
{
}
