﻿using System.Collections;
using System;
using UnityEngine;
using SimpleFirebaseUnity;
using UnityEngine.UI;

public class Backend : MonoBehaviour 
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
	private Tubeman _tubeman1;
	private Tubeman _tubeman2;

	[System.Serializable]
	public class TubemanView
	{
		public Text name;
		public Text odds;
		public Text pot;
	}

	private class Tubeman
	{
		public Action OnPotUpdated;
		public string name;
		public string odds;
		public int pot;
		private TubemanView _view;
		private Firebase _rootRef;
		private Firebase _nameRef;
		private Firebase _oddsRef;
		private Firebase _potRef;

		public Tubeman Setup(TubemanView view, string key, Firebase rootRef)
		{
			_view = view;
			_rootRef = rootRef.Child(key, true);
			_nameRef = _rootRef.Child("name", true);
			_oddsRef = _rootRef.Child("odds", true);
			_potRef = _rootRef.Child("pot", true);
			return this;
		}

		public void Subscribe()
		{
			_nameRef.OnGetSuccess += OnGetName;
			_oddsRef.OnGetSuccess += OnGetOdds;
			_potRef.OnGetSuccess += OnGetPot;
		}

		public void Unsubscribe()
		{
			_nameRef.OnGetSuccess -= OnGetName;
			_oddsRef.OnGetSuccess -= OnGetOdds;
			_potRef.OnGetSuccess -= OnGetPot;
		}

		public void Fetch()
		{
			_nameRef.GetValue();
			_oddsRef.GetValue();
			_potRef.GetValue();
		}

		public void FetchPot()
		{
			_potRef.GetValue();
		}

		private void OnGetName(Firebase sender, DataSnapshot snapshot)
		{
			name = snapshot.Value<string>();
			_view.name.text = name;
		}

		private void OnGetOdds(Firebase sender, DataSnapshot snapshot)
		{
			odds = snapshot.Value<string>();
			_view.odds.text = odds;
		}

		private void OnGetPot(Firebase sender, DataSnapshot snapshot)
		{
			pot = snapshot.Value<int>();
			_view.pot.text = pot.ToString();
			if (OnPotUpdated != null) OnPotUpdated();
		}

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
		StartCoroutine(Sync(1f));
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void SetupRefs()
	{
		_rootRef = Firebase.CreateNew("tubeman-677a6.firebaseio.com", "GV6eYkao7ZXQzG5D3h3PsnFeJGLZt0YGtnktHBmo");
		_appStateRef = _rootRef.Child("appState", true);
		_winnerRef = _rootRef.Child("winner", true);
		_usersRef = _rootRef.Child("users", true);
		_tubeman1 = new Tubeman().Setup(tubeman1View, "tubeman1", _rootRef);
		_tubeman2 = new Tubeman().Setup(tubeman1View, "tubeman2", _rootRef);
	}

	private void Subscribe()
	{
		_appStateRef.OnGetSuccess += OnGetAppState;
		_tubeman1.Subscribe();
		_tubeman2.Subscribe();
		//Debug
		_rootRef.OnGetSuccess += GetOKHandler;
		_rootRef.OnGetFailed += GetFailHandler;
		_rootRef.OnSetSuccess += SetOKHandler;
		_rootRef.OnSetFailed += SetFailHandler;
		_rootRef.OnUpdateSuccess += UpdateOKHandler;
		_rootRef.OnUpdateFailed += UpdateFailHandler;
		_rootRef.OnPushSuccess += PushOKHandler;
		_rootRef.OnPushFailed += PushFailHandler;
		_rootRef.OnDeleteSuccess += DelOKHandler;
		_rootRef.OnDeleteFailed += DelFailHandler;
	}

	private void Unsubscribe()
	{
		_appStateRef.OnGetSuccess -= OnGetAppState;
		_tubeman1.Unsubscribe();
		_tubeman2.Unsubscribe();
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
		_usersRef.OnGetSuccess -= SettleBetsHandler;
		_usersRef.GetValue();
	}

	private void SettleBetsHandler(Firebase sender, DataSnapshot snapshot)
	{
		_usersRef.OnGetSuccess -= SettleBetsHandler;
		// Debug.Log(snapshot.Value());
		// Debug.Log(typeof(snapshot.Value());
	}
	public void NextGame()
	{
		_appStateRef.SetValue("0", true);	
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

    void DelOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] Del from key: <" + sender.FullKey + ">");
    }

    void DelFailHandler(Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] Del from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void PushOKHandler(Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] Push from key: <" + sender.FullKey + ">");
    }

    void PushFailHandler(Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] Push from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }
    void DoDebug(string str)
    {
        Debug.Log(str);
    }

}
