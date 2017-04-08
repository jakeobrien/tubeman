using UnityEngine;
using System.Collections;

/// <summary>
/// Range. Takes min and max values, and return random between.
/// </summary>

[System.Serializable]
public class Range 
{

	public float Min;
	public float Max;

	public Range(float min, float max)
	{
		Min = min;
		Max = max;
	}

	public float RandomInRange
	{
		get 
		{ 
			return Random.Range(Min, Max); 
		}
	}

	public bool IsValueInRange(float value)
	{
		return (value >= Min && value <= Max);
	}
}
