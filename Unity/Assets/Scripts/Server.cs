﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SimpleFirebaseUnity;
using UnityEngine.UI;

public class Server : MonoBehaviour 
{

	public Button startGameButton;
	public Button tubeman1WonButton;
	public Button tubeman2WonButton;
	public Button nextGameButton;
	public TubemanView tubeman1View;
	public TubemanView tubeman2View;

	private int _appState;
	private Firebase _rootRef;
	private Firebase _appStateRef;
	private Firebase _usersRef;
	private Firebase _winnerRef;
	private int _winner;
	private float _potRatio;
	private ServerTubeman _tubeman1;
	private ServerTubeman _tubeman2;
	private Coroutine _syncCoroutine;

	[System.Serializable]
	public class TubemanView
	{
		public Text name;
		public Text odds;
		public Text pot;
	}

	private void Awake()
	{
		SetupRefs();
	}

	private void OnEnable()
	{
		Subscribe();
		LayoutForAppState();
		Fetch();
		_syncCoroutine = StartCoroutine(Sync(1f));
	}

	private void OnDisable()
	{
		if (_syncCoroutine != null) StopCoroutine(_syncCoroutine);
		Unsubscribe();
	}

	private void SetupRefs()
	{
		_rootRef = Firebase.CreateNew("tubeman-677a6.firebaseio.com", "GV6eYkao7ZXQzG5D3h3PsnFeJGLZt0YGtnktHBmo");
		_appStateRef = _rootRef.Child("appState", true);
		_winnerRef = _rootRef.Child("winner", true);
		_usersRef = _rootRef.Child("users", true);
		_tubeman1 = new ServerTubeman().Setup(tubeman1View, "tubeman1", _rootRef);
		_tubeman2 = new ServerTubeman().Setup(tubeman2View, "tubeman2", _rootRef);
	}

	private void Subscribe()
	{
		_appStateRef.OnGetSuccess += OnGetAppState;
		_tubeman1.Subscribe();
		_tubeman2.Subscribe();
		_tubeman1.OnPotUpdated = OnPotUpdated;
		_tubeman2.OnPotUpdated = OnPotUpdated;
		
		//Debug
		_rootRef.OnGetSuccess += GetOKHandler;
		_rootRef.OnGetFailed += GetFailHandler;
		_rootRef.OnSetSuccess += SetOKHandler;
		_rootRef.OnSetFailed += SetFailHandler;
		_rootRef.OnUpdateSuccess += UpdateOKHandler;
		_rootRef.OnUpdateFailed += UpdateFailHandler;
	}

	private void Unsubscribe()
	{
		_appStateRef.OnGetSuccess -= OnGetAppState;
		_tubeman1.Unsubscribe();
		_tubeman2.Unsubscribe();
		_tubeman1.OnPotUpdated = null;
		_tubeman2.OnPotUpdated = null;
	}

	private void Fetch()
	{
		_appStateRef.GetValue();
		_tubeman1.Fetch();
		_tubeman2.Fetch();
	}

	private IEnumerator Sync(float interval)
	{
		while (true)
		{
			_appStateRef.GetValue();
			_tubeman1.FetchPot();
			_tubeman2.FetchPot();
			yield return new WaitForSeconds(interval);
		}
	}

	private void OnGetAppState(Firebase sender, DataSnapshot snapshot)
	{
		_appState = Int32.Parse(snapshot.RawJson);
		LayoutForAppState();
	}

	private void LayoutForAppState()
	{
		startGameButton.gameObject.SetActive(_appState == 0);
		tubeman1WonButton.gameObject.SetActive(_appState == 1);
		tubeman2WonButton.gameObject.SetActive(_appState == 1);
		nextGameButton.gameObject.SetActive(_appState == 2);
	}

	private void OnPotUpdated()
	{
		if (_tubeman1.pot == 0 || _tubeman2.pot == 0) _potRatio = 0f;
		else _potRatio = (float)_tubeman1.pot / (float)_tubeman2.pot;
	}

	public void StartGame()
	{
		_appStateRef.SetValue("1", true);	
	}

	public void Tubeman1Won()
	{
		FightEnded("1");
	}

	public void Tubeman2Won()
	{
		FightEnded("2");
	}

	private void FightEnded(string tubeman)
	{
		SettleBets();
		_winnerRef.SetValue(tubeman, true);
		_appStateRef.SetValue("2", true);
	}

	private void SettleBets()
	{
		_usersRef.OnGetSuccess += SettleBetsHandler;
		_usersRef.GetValue();
	}

	private void SettleBetsHandler(Firebase sender, DataSnapshot snapshot)
	{
		if (_potRatio == 0f) return;
		var users = snapshot.Value<Dictionary<string,object>>();
		foreach (var user in users)
		{
			// Debug.Log(user.GetType());
			var uid = user.Key;
			var userDict = user.Value as Dictionary<string,object>;
			if (userDict == null) continue;
			var winningBetKey = "bet1";
			var losingBetKey = "bet2";
			var payoutOdds = 1f / _potRatio;
			if (_winner == 2) 
			{
				winningBetKey = "bet2";
				losingBetKey = "bet1";
				payoutOdds = _potRatio;
			}
			var WinningBet = userDict[winningBetKey] as Dictionary<string,object>;
			var winningBetAmount = 0;
			if (WinningBet != null) winningBetAmount = (int)(System.Int64)WinningBet["amount"];

			var winAmount = winningBetAmount + winningBetAmount * payoutOdds;
			var currentBank = (int)(System.Int64)userDict["bank"];
			var newBank = winAmount + currentBank;
			_usersRef.Child(uid).UpdateValue("{\"bank\":" + newBank.ToString() + "}", true, FirebaseParam.Empty);
			_usersRef.Child(uid).Child(winningBetKey).UpdateValue("{\"payout\":" + winAmount.ToString() + "}", true, FirebaseParam.Empty);
			_usersRef.Child(uid).Child(losingBetKey).UpdateValue("{\"payout\":0}", true, FirebaseParam.Empty);
			
		}
		_usersRef.OnGetSuccess -= SettleBetsHandler;
	}

	public void NextGame()
	{
		_appStateRef.SetValue("0", true);	
		_tubeman1.Reset();
		_tubeman2.Reset();
	}

    void GetOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] Get from key: <" + sender.FullKey + ">");
        DoDebug("[OK] Raw Json: " + snapshot.RawJson);
    }

    void GetFailHandler(Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] Get from key: <" + sender.FullKey + ">,  " + err.Message + " (" + (int)err.Status + ")");
    }

    void SetOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] Set from key: <" + sender.FullKey + ">");
    }

    void SetFailHandler(Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] Set from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

	void UpdateOKHandler(Firebase sender, DataSnapshot snapshot)
	{
		DoDebug("[OK] Update from key: <" + sender.FullKey + ">");
	}

	void UpdateFailHandler(Firebase sender, FirebaseError err)
	{
		DoDebug("[ERR] Update from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
	}

    void DoDebug(string str)
    {
        Debug.Log(str);
    }

}
