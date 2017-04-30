using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour 
{
	public TubeMan tubeman1;
	public TubeMan tubeman2;
	public GameObject winnerUI;
	public Text winnerText;

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
		ShowWinner();
	}

	private void ShowWinner()
	{
		winnerUI.SetActive(true);
		winnerText.text = string.Format("{0} wins!", _winner.name);
	}

}
