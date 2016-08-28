using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spewnity;

public class CoinBar : MonoBehaviour
{
	public Text bankText;
	public Text incomeText;
	public Text taxText;
	public Vector3 kingsVaultLocation;
	public GameObject coinPrefab;

	private int curBank = 0;
	private int curIncome = 0;
	private int curTax = 0;
//	private int kingsVault = 0;
//	private float animDuration = 0.5f;
//	private ActionQueue aq;

	void Awake()
	{
//		aq = gameObject.AddComponent<ActionQueue>();
	}

	void Start()
	{
	}

	void Update()
	{
	}

	public void onBankChanged(Transaction trans)
	{
		sendCoins(trans, bankText, ref curBank);
	}

	public void onIncomeChanged(Transaction trans)
	{
		sendCoins(trans, incomeText, ref curIncome);
	}

	public void onTaxChanged(Transaction trans)
	{
		sendCoins(trans, taxText, ref curTax);
	}

	public void onKingsVaultChanged(Transaction trans)
	{
//		sendCoins(trans, null, ref kingsVault);
	}

	private void sendCoins(Transaction trans, Text target, ref int account)
	{
		target.text = trans.amount.ToString();
		// UGH! This is taking too much time to figure out. I have to scrap it.
//		int coins = trans.amount - account;	
//		Debug.Log("Coins:" + coins + " Amount:" + trans.amount + " type:" + trans.source + " target:" + (target == null ? "null" :  target.name));
//		if(coins <= 0 || trans.source == TransactionSource.None)
//		{
//			if(target != null) target.text = trans.amount.ToString();
//			account = trans.amount;
//			return;
//		}
//
//		Vector3 source = kingsVaultLocation;
//		Vector3 dest = kingsVaultLocation;
//		switch(trans.source)
//		{
//			case TransactionSource.Bank:
//				source = bankText.transform.position;
//				break;
//
//			case TransactionSource.Income:
//				source = incomeText.transform.position;
//				break;
//
//			case TransactionSource.Tax:
//				source = taxText.transform.position;
//				break;
//
//			case TransactionSource.None:
//			case TransactionSource.KingsVault:
//				break;
//
//			case TransactionSource.Custom:
//				source = trans.custom.position;
//				break;
//		}			
//
//		// Animate coins
//		for(int i = 0; i < trans.amount; i++)
//		{
//			aq.Instantiate(coinPrefab, source);
//			aq.Add(() => aq.selectedGameObject.transform.LerpPosition(dest, animDuration));
//			aq.Delay(animDuration);
//			aq.Add(() => {
//				int cur = int.Parse(target.text);
//				cur++;
//				target.text = cur.ToString();
//			});
//			aq.Destroy();
//		}
//
//		if(!aq.running) aq.Run();
	}
}

