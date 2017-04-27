using System;
using SimpleFirebaseUnity;

public class ServerTubeman
{
    public Action OnPotUpdated;
    public string name;
    public string odds;
    public int pot;
    private Server.TubemanView _view;
    private Firebase _rootRef;
    private Firebase _nameRef;
    private Firebase _oddsRef;
    private Firebase _potRef;

    public ServerTubeman Setup(Server.TubemanView view, string key, Firebase rootRef)
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

    public void Reset()
    {
        _potRef.SetValue("0", true);
        _oddsRef.SetValue("1:1", true);
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