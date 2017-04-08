using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour 
{
	[System.Serializable]
	public class Player
	{
		public KeyCode key;
		public TubeMan tubeMan;
	}

	public Player[] players;

	public void Update()
	{
		foreach (var player in players)
		{
			player.tubeMan.isFanOn = Input.GetKey(player.key);
		}
	}

}
