using UnityEngine;

[System.Serializable]
public struct Transaction
{
	public int amount;
	public TransactionSource source;
	public Transform custom;

	public Transaction(int amount, TransactionSource transType)
	{
		this.amount = amount;
		this.source = transType;
		this.custom = null;
	}

	public Transaction(int amount, Transform customSource)
	{
		this.amount = amount;
		this.source = TransactionSource.Custom;
		this.custom = customSource;
	}
}

[System.Serializable]
public enum TransactionSource
{
	None,
	KingsVault,
	Bank,
	Income,
	Tax,
	Custom
}
