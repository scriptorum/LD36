﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Help : MonoBehaviour 
{
	public GameObject other;

	void Awake()
	{
	}

	void Start()
	{
	}

	void Update()
	{
	}

	void OnMouseDown()
	{
		gameObject.SetActive(false);
		other.SetActive(true);
	}
}

