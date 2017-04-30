using System.Collections.Generic;
using System;

public class Odds
{

    public static List<Odds> Table = new List<Odds>()
    {
        new Odds { winPercent = 0.91f, oddsString = "1/10" },
        new Odds { winPercent = 0.89f, oddsString = "1/8" },
        new Odds { winPercent = 0.86f, oddsString = "1/6" },
        new Odds { winPercent = 0.83f, oddsString = "1/5" },
        new Odds { winPercent = 0.80f, oddsString = "1/4" },
        new Odds { winPercent = 0.75f, oddsString = "1/3" },
        new Odds { winPercent = 0.67f, oddsString = "1/2" },
        new Odds { winPercent = 0.6f,  oddsString = "2/3" },
        new Odds { winPercent = 0.57f, oddsString = "3/4" },
        new Odds { winPercent = 0.5f,  oddsString = "1/1" },
        new Odds { winPercent = 0.43f, oddsString = "4/3" },
        new Odds { winPercent = 0.4f,  oddsString = "3/2" },
        new Odds { winPercent = 0.33f, oddsString = "2/1" },
        new Odds { winPercent = 0.25f, oddsString = "3/1" },
        new Odds { winPercent = 0.2f,  oddsString = "4/1" },
        new Odds { winPercent = 0.17f, oddsString = "5/1" },
        new Odds { winPercent = 0.14f, oddsString = "6/1" },
        new Odds { winPercent = 0.11f, oddsString = "8/1" },
        new Odds { winPercent = 0.09f, oddsString = "10/1" }
    };

    public float winPercent;
    public string oddsString;

    public float GetPayout(float bet)
    {
        if (winPercent == 0f) return bet;
        return bet * 1f / winPercent;
    }

    public static Odds GetNearest(float winPercent)
    {
        var closestDistance = 0f;
        Odds closestOdds = null;
        foreach (var odds in Table)
        {
            var distance = Math.Abs(winPercent - odds.winPercent);
            if (closestOdds == null || distance < closestDistance)
            {
                closestDistance = distance;
                closestOdds = odds;
            }
        }
        return closestOdds;
    }

}