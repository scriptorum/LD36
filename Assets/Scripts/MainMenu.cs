using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spewnity;

public class MainMenu : MonoBehaviour
{
	private static float loopAtSampleTime = 16f;
	private static int sampleRate = 44000;
	private static int cpuAdjust = 500; // Given that time will pass between measurement and assignment, this is a random stab at the period.

	void Start()
	{
		SoundManager.instance.Stop();

		// I added a delay property to Sound which does a great job of synching the two sounds
		// HOWEVER on WebGL it refuses to loop a delayed sound, which kills the whole point.
		// So, I tried this to in order to fix the timing bug, which works well on my
		// machine but isn't really any better on WebGL. Sigh.
		SoundManager.instance.GetSound("theme-loop").volume = 0;
		SoundManager.instance.Play("theme-intro");
		Invoke("onIntroFinished", loopAtSampleTime);
	}

	public void onIntroFinished()
	{
		// Fade intro out (which has 1 sec of overlap) and loop in for a less clicky result
		SoundManager.instance.FadeIn("theme-loop", 0.1f);
		SoundManager.instance.FadeOut("theme-intro", 0.1f);

		// Assuming we've arrived here "late" adjust the loop's time point to match the intro's "overlap" position
		AudioSource loop = SoundManager.instance.GetSource("theme-loop");
		int introTimeBeforeLoop = (int) (sampleRate * loopAtSampleTime);
		int introTimeNow = SoundManager.instance.GetSource("theme-intro").timeSamples;
		loop.timeSamples = introTimeNow - introTimeBeforeLoop - cpuAdjust;
	}
}

