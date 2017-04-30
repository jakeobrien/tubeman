using System.Collections.Generic;
using System;

public class Odds
{

    public static List<Odds> Table = new List<Odds>()
    {
        new Odds { winPercent = 1f, oddsString = "0" },
        new Odds { winPercent = 0.8f, oddsString = "1/4" },
        new Odds { winPercent = 0.67f, oddsString = "1/2" },
        new Odds { winPercent = 0.57f, oddsString = "3/4" },
        new Odds { winPercent = 0.5f, oddsString = "1/1" },
        new Odds { winPercent = 0.4f, oddsString = "3/2" },
        new Odds { winPercent = 0.33f, oddsString = "2/1" },
        new Odds { winPercent = 0.20f, oddsString = "4/1" },
        new Odds { winPercent = 0.11f, oddsString = "8/1" }
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