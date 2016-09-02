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
	private static string BANK = "bank";
	private static string INCOME = "income";
	private static string TAX = "tax";
	private static string VAULT = "vault";
	private static string CUSTOM = "custom";

	private Board board;
	private Dictionary<string,int> accounts = new Dictionary<string, int>();

	public int turn;
	public bool gameOver = false;
	public int roadsPlaced = 0;
	public int villagesConnected = 0;

	public MessageBar messageBar;
	public CoinEvent onTransaction;

	#if UNITY_WEBGL && !UNITY_EDITOR
	public string JSPrompt(string prompt, string defaultInput = "")
	{
		return Prompt(prompt, defaultInput);
	}

	public void JSAlert(string msg)
	{
		Alert(msg);
	}

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void Alert(string msg);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern string Prompt(string prompt, string defaultInput);
	#endif

	void Awake()
	{
		accounts[BANK] = accounts[INCOME] = accounts[TAX] = accounts[VAULT] = 0;
		board = GameObject.Find("Board").GetComponent<Board>();
		Tile.onMouseOver.AddListener(this.mouseOver);
	}

	void Start()
	{
		SoundManager.instance.Stop();
		SoundManager.instance.Play("start");

		setBalance(BANK, STARTING_BANK);
		setBalance(TAX, 1);
		setBalance(INCOME, 0);
		setBalance(VAULT, 99999);
		villagesConnected = 0;
		messageBar.setMessage("Network all the villages! [H] for help.");
	}

	void parseCode(string code)
	{
		if(code.StartsWith("R"))
		{
			Board.replay = "";
			Board.seed = int.Parse(code.Remove(0, 1));
			SceneManager.LoadScene("Play");
		}
		else if(code.StartsWith("C"))
		{
			Board.replay = code.Remove(0, 1);
			SceneManager.LoadScene("Play");
		}
		else messageBar.setMessage("Failed to parse level code:" + code);
	}

	string makeCode()
	{
		return Board.replay == "" ? "R" + Board.seed : "C" + board.contents;
	}

	void Update()
	{		
		if(Input.GetKeyDown(KeyCode.R))
		{
			if(board.editMode) Board.replay = board.contents;
			SceneManager.LoadScene("Play");
		}
		else if(Input.GetKeyDown(KeyCode.Escape))
		{
			Board.replay = "";
			SceneManager.LoadScene("Main");
		}
		else if(Input.GetKeyDown(KeyCode.C))
		{
			string code = makeCode();

			#if UNITY_WEBGL && !UNITY_EDITOR
			JSAlert("The level code is: " + code);
			#else
			GUIUtility.systemCopyBuffer = code;
			messageBar.setMessage("Copied level to clipboard");
			#endif
		}
		else if(Input.GetKeyDown(KeyCode.P))
		{
			string code = "";

			#if UNITY_WEBGL && !UNITY_EDITOR
			code = JSPrompt("Enter a level code");
			#else
			code = GUIUtility.systemCopyBuffer;
			#endif
			parseCode(code);
		}
		else if(Input.GetKeyDown(KeyCode.E))
		{
			board.editMode = !board.editMode;
			messageBar.setMessage("Edit mode " + (board.editMode ? "on" : "off"));
			if(!board.editMode) Board.replay = board.contents;
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
			if(getBalance(BANK) < cost)
			{
				if(getBalance(BANK) <= 0) fail("You're broke. Hit SPACE to advance to next year.");
				else fail("It costs " + cost + " coins to build over " + tile.type.name);
				return;
			}

			// Charge your account for the road
			transferBalance(cost, BANK, CUSTOM, tile.transform.position);

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
						transferBalance(t.type.income, CUSTOM, INCOME, t.transform.position);
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

	public void nextTurn()
	{
		int net = getBalance(INCOME) - getBalance(TAX);
		turn++;
		if(net > 0) transferBalance(net, VAULT, BANK);
		else if(net < 0) transferBalance(net, BANK, VAULT);
		transferBalance(TAX_RATE, VAULT, TAX);
		if(getBalance(BANK) < 0)
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

	private void verifyAccount(string accountName)
	{
		if(accountName == CUSTOM) return;
		if(!accounts.ContainsKey(accountName)) throw new UnityException("Account not found:" + accountName);
	}

	private void adjustAccount(string accountName, int amount)
	{
		verifyAccount(accountName);
		if(accountName == CUSTOM) return;

		accounts[accountName] += amount;
	}

	private int setBalance(string account, int value)
	{
		verifyAccount(account);
		accounts[account] = value;
		Transaction trans = new Transaction(TransactionType.Update, value, null, account);
		onTransaction.Invoke(trans);
		return value;
	}

	private int getBalance(string accountName)
	{
		verifyAccount(accountName);
		return accounts[accountName];
	}
		

	// Transfer account receives the funds
	private void transferBalance(int amount, string sendAccount, string receiveAccount, Vector3 custom = new Vector3())
	{
		adjustAccount(sendAccount, -amount);
		adjustAccount(receiveAccount, amount);
		Transaction trans = new Transaction(TransactionType.Transfer, amount, sendAccount, receiveAccount);
		trans.custom = custom;
		onTransaction.Invoke(trans);
	}
}

[System.Serializable]
public class CoinEvent: UnityEvent<Transaction>
{
}