using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spewnity;

public class MainMenu : MonoBehaviour 
{
	void Awake()
	{
	}

	void Start()
	{
		SoundManager.instance.Stop();
		SoundManager.instance.Play("theme-intro", (s)=> SoundManager.instance.Play("theme-loop"));
	}

	void Update()
	{
	}
}

