using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour 
{
	public TubeMan tubeman1;
	public TubeMan tubeman2;

	private TubeMan _winner;

	private void Start()
	{
		tubeman1.DidDie += TubemanDidDie;
		tubeman2.DidDie += TubemanDidDie;
	}

	private void TubemanDidDie()
	{
		if (tubeman1.isDead && !tubeman2.isDead) _winner = tubeman1;
		else if (!tubeman1.isDead && tubeman2.isDead) _winner = tubeman1;
		else _winner = tubeman1.health < tubeman2.health ? tubeman1 : tubeman2;
	}
	
}
