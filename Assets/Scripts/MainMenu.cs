using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spewnity;

public class MainMenu : MonoBehaviour
{
	private Sound intro;
	private Sound loop;

	void Awake()
	{
	}

	void Start()
	{
		SoundManager.instance.Stop();

		// I added a delay property to Sound which does a great job of synching the two sounds
		// HOWEVER on WebGL it refuses to loop a delayed sound, which kills the whole point.
		// So, I tried this to in order to fix the timing bug, which works well on my
		// machine but isn't really any better on WebGL. Sigh.
		loop = SoundManager.instance.Play("theme-loop");
		loop.source.volume = 0;
		intro = SoundManager.instance.Play("theme-intro", onIntroFinished);
	}

	public void onIntroFinished(Sound sound)
	{
		loop.source.volume = 1;
		intro.source.Stop();
	}
}

