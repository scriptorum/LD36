using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinBar : MonoBehaviour 
{
	public Text bankText;
	public Text incomeText;
	public Text taxText;

	void Awake()
	{
	}

	void Start()
	{
	}

	void Update()
	{
	}

	public void onBankChanged(int amount)
	{
		bankText.text = amount.ToString();
	}

	public void onIncomeChanged(int amount)
	{
		incomeText.text = amount.ToString();
	}

	public void onTaxChanged(int amount)
	{
		taxText.text = amount.ToString();
	}
}

