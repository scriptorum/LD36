using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spewnity;

public class CoinBar : MonoBehaviour
{
	private static string CUSTOM = "custom";

	public List<CoinBankAccount> accounts;
	public GameObject coinPrefab;

	private float animDuration = 0.2f;
	private ActionQueue aq;

	void Awake()
	{
		aq = gameObject.AddComponent<ActionQueue>();
	}


	void Start()
	{
		foreach(CoinBankAccount acct in accounts) updateText(acct);
	}

	void Update()
	{
	}

	public CoinBankAccount getAccount(string accountName)
	{
		if(accountName == CUSTOM) return null;
		CoinBankAccount acct = accounts.Find((a) => a.name == accountName);
		if(acct == null) throw new UnityException("Cannot find " + accountName);
		return acct;
	}

	public void updateText(CoinBankAccount acct)
	{
		if(acct.text == null) return;
		acct.text.text = acct.value.ToString();
	}

	public void onTransaction(Transaction trans)
	{
		switch(trans.type)
		{
			case TransactionType.Transfer:
				transferCoins(trans);
				break;

			case TransactionType.Update:
				CoinBankAccount account = getAccount(trans.receiveAccount);
				account.value = trans.amount;
				updateText(account);
				break;
		}
	}

	private void transferCoins(Transaction trans)
	{
		for(int i = 0; i < trans.amount; i++) transferCoin(trans);

		if(!aq.running) aq.Run();	
	}

	private void transferCoin(Transaction trans)
	{
		CoinBankAccount sendAcct = getAccount(trans.sendAccount);
		CoinBankAccount receiveAcct = getAccount(trans.receiveAccount);
		Vector3 sendVector = (trans.sendAccount == CUSTOM ? trans.custom : sendAcct.getVector());
		Vector3 receiveVector = (trans.receiveAccount == CUSTOM ? trans.custom : receiveAcct.getVector());

		// Only actually remove coin from transfer account if this is a transfer, otherwise just gain coin at main account
		if(sendAcct != null) aq.Add(() =>
			{
				sendAcct.value -= 1;
				updateText(sendAcct);
			});
		aq.Instantiate(coinPrefab, sendVector); // will select object
		aq.Add(() =>
		{
			aq.Pause();
			StartCoroutine(aq.selectedGameObject.transform.LerpPosition(receiveVector, animDuration, null, (tf) => aq.Resume()));
		});
//		aq.Log("Coin from " + sendAcct.name + " arrived at " + receiveAcct.name);
		if(receiveAcct != null) aq.Add(() =>
			{
				receiveAcct.value += 1;
				updateText(receiveAcct);
			});
		aq.Destroy();
	}
}

[System.Serializable]
public class CoinBankAccount
{
	public string name;
	public Text text;
	public Transform position;
	public int value;

	public Vector3 getVector()
	{
		if(position != null) return position.position;
		else if(text != null) return text.transform.position;
		else throw new UnityException("No vector available for account " + name);
	}
}