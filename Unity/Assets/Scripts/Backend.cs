using System.Collections;
using System;
using UnityEngine;
using SimpleFirebaseUnity;
using UnityEngine.UI;

public class Backend : MonoBehaviour 
{

	[System.Serializable]
	public class TubemanView
	{
		public Text name;
		public Text odds;
		public Text pot;
	}

	public class Tubeman
	{
		public Firebase root;
		public Firebase name;
		public Firebase odds;
		public Firebase pot;
		public TubemanView view;

	}

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
	private Tubeman _tubeman1;
	private Tubeman _tubeman2;

	private void Awake()
	{
		LayoutForAppState();
		Setup();
		Fetch();
		StartCoroutine(FetchAppState(0.8f));
	}

	private void Setup()
	{
		_rootRef = Firebase.CreateNew("tubeman-677a6.firebaseio.com", "GV6eYkao7ZXQzG5D3h3PsnFeJGLZt0YGtnktHBmo");
		// Init callbacks
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
		_appStateRef = _rootRef.Child("appState", true);
		_appStateRef.OnGetSuccess += (sender, snapshot) => { _appState = Int32.Parse(snapshot.RawJson); LayoutForAppState(); };
		_winnerRef = _rootRef.Child("winner", true);
		_usersRef = _rootRef.Child("users", true);
		_tubeman1 = SetupTubeman(tubeman1View, "tubeman1");
		_tubeman2 = SetupTubeman(tubeman2View, "tubeman2");
	}

	private Tubeman SetupTubeman(TubemanView view, string key)
	{
		var tubeman = new Tubeman();
		tubeman.root = _rootRef.Child(key, true);
		tubeman.name = tubeman.root.Child("name", true);
		tubeman.name.OnGetSuccess += (sender, snapshot) => view.name.text = snapshot.Value<string>();
		tubeman.odds = tubeman.root.Child("odds", true);
		tubeman.odds.OnGetSuccess += (sender, snapshot) => view.odds.text = snapshot.Value<string>();
		tubeman.pot = tubeman.root.Child("pot", true);
		tubeman.pot.OnGetSuccess += (sender, snapshot) => view.pot.text = Int32.Parse(snapshot.RawJson).ToString();
		return tubeman;
	}

	private void Fetch()
	{
		_appStateRef.GetValue();
		FetchTubeman(_tubeman1);
		FetchTubeman(_tubeman2);
	}

	private void FetchTubeman(Tubeman tubeman)
	{
		tubeman.name.GetValue();
		tubeman.odds.GetValue();
		tubeman.pot.GetValue();
	}

	private IEnumerator FetchAppState(float interval)
	{
		while (true)
		{
			_appStateRef.GetValue();
			_tubeman1.pot.GetValue();
			_tubeman2.pot.GetValue();
			yield return new WaitForSeconds(interval);
		}
	}

	private void LayoutForAppState()
	{
		Debug.Log(_appState);
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
