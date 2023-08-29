using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tesst : MonoBehaviour
{
#if (UNITY_SERVER)
	void Start()
	{
		LogConsoleError("Error");
		Debug.Log("Other");
	}

	public static void LogConsoleError(string text)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Debug.LogError(text);
		Console.ForegroundColor = ConsoleColor.White;
	}
#endif
}
