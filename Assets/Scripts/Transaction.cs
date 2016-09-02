using UnityEngine;

[System.Serializable]
public struct Transaction
{
	public TransactionType type;
	public int amount;
	public string sendAccount;
	public string receiveAccount;
	public Vector3 custom;


	public Transaction(TransactionType type, int amount, string sendAccount, string receiveAccount)
	{
		this.type = type;
		this.amount = amount;
		this.sendAccount = sendAccount;
		this.receiveAccount = receiveAccount;
		this.custom = Vector3.zero;
		if(type != TransactionType.Update) Debug.Assert(amount > 0);
	}
}

public enum TransactionType
{
	Transfer, // move to main account from transfer account
	Update // set main account balance
}