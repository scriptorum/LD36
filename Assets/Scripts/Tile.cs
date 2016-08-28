using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Spewnity;

public class Tile : MonoBehaviour
{
	public static TileEvent onMouseOver = new TileEvent();

	public int x;
	public int y;
	public bool hasGlow = false;
	public bool hasRoad = false;
	public TerrainType type;
	private ParticleSystem dustFX;
	private ParticleSystem kachingFX;

	void Awake()
	{
		dustFX = gameObject.GetChild("Dust").GetComponent<ParticleSystem>();
		kachingFX = gameObject.GetChild("Kaching").GetComponent<ParticleSystem>();
	}

	void Start()
	{
	}

	void Update()
	{
	}

	void OnMouseOver()
	{
		Tile.onMouseOver.Invoke(this);
	}

	public void showDust()
	{
		dustFX.Play();
	}

	public void showKaching()
	{
		kachingFX.Play();
	}
}

public class TileEvent : UnityEvent<Tile>
{
}

