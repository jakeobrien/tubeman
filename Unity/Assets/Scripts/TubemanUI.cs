using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TubemanUI : MonoBehaviour 
{

    [SerializeField]
    private Text _nameText;
    [SerializeField]
    private Text _potText;
    [SerializeField]
    private Text _oddsText;
    [SerializeField]
    private Slider _healthSlider;
    
    public string Name
    {
        set { _nameText.text = value; }
    }

    public int Pot
    {
        set { _potText.text = string.Format("${0}", value); }
    }

    public string Odds
    {
        set { _oddsText.text = value; }
    }

}
