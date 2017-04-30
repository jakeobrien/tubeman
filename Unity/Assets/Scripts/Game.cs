using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour 
{
	public static bool GameStarted;

	public TubeMan tubeman1;
	public TubeMan tubeman2;
	public GameObject winnerUI;
	public Text winnerText;
	public Text countdownText;
	public float resultsDelay;
	public float restartDelay;

	private TubeMan _winner;

	private void Awake()
	{
		GameStarted = false;
	}

	private void Start()
	{
		tubeman1.DidDie += TubemanDidDie;
		tubeman2.DidDie += TubemanDidDie;
		StartCoroutine(Countdown());
	}

	private IEnumerator Countdown()
	{
		var countdown = 3;
		countdownText.gameObject.SetActive(true);
		while (countdown > 0)
		{
			countdownText.text = countdown.ToString();
			yield return new WaitForSeconds(1f);
			countdown--;
		}
		countdownText.gameObject.SetActive(false);
		StartGame();
	}

	private void StartGame()
	{
		GameStarted = true;
		tubeman1.StartGame();
		tubeman2.StartGame();
	}

	private void TubemanDidDie()
	{
		tubeman1.DidDie -= TubemanDidDie;
		tubeman2.DidDie -= TubemanDidDie;
		if (tubeman1.isDead && !tubeman2.isDead) _winner = tubeman1;
		else if (!tubeman1.isDead && tubeman2.isDead) _winner = tubeman1;
		else _winner = tubeman1.health < tubeman2.health ? tubeman1 : tubeman2;
		StartCoroutine(ShowGameOver());
	}

	private IEnumerator ShowGameOver()
	{
		yield return new WaitForSeconds(resultsDelay);
		ShowWinner();
		yield return new WaitForSeconds(restartDelay);
		RestartGame();
	}

	private void ShowWinner()
	{
		winnerUI.SetActive(true);
		winnerText.text = string.Format("{0} wins!", _winner.name);
	}

	private void RestartGame()
	{
		SceneManager.LoadScene("Main");
	}



}
