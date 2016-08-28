using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spewnity;

public class MessageBar : MonoBehaviour 
{
	public float startY;
	public float endY = -4.2f;
	public float moveSpeed = 1.0f;
	public float holdTime = 3.0f;
	public Text messageText;
	private ActionQueue aq;

	void Awake()
	{
		aq = gameObject.AddComponent<ActionQueue>();
		startY = transform.position.y;
		gameObject.SetActive(false);
	}

	void Start()
	{
	}

	void Update()
	{
	}

	public void setMessage(string msg)
	{
		messageText.text = msg;
		transform.position = new Vector3(transform.position.x, startY, transform.position.z);
		gameObject.SetActive(true);

		aq.Reset();

		aq.Add(() => StartCoroutine(transform.LerpPosition(new Vector3(transform.position.x, endY, transform.position.z), moveSpeed)));
		aq.Delay(moveSpeed);
		aq.Delay(holdTime);
		aq.Add(() => StartCoroutine(transform.LerpPosition(new Vector3(transform.position.x, startY, transform.position.z), moveSpeed)));
		aq.Delay(moveSpeed);
		aq.Add(() => gameObject.SetActive(false));
		aq.Run();
	}
}

