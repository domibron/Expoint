/* WebTextFileChecker.cs
This checks the text in a text file on the web - believe it!
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Net;
using UnityEditor;
using System;

public class ReadFromPasteBin : MonoBehaviour
{
	string text;

	public IEnumerator Start()
	{
		StartCoroutine(Check());
		if (String.IsNullOrEmpty(text)) yield return new WaitForSeconds(0.1f);

		if (text == "Failed")
		{
			Debug.LogError("Failed to contact Paste");
			// abort
		}
		else
		{

		}
	}

	private IEnumerator Check()
	{ // ? = unity's version https://docs.unity3d.com/Manual/UnityWebRequest-RetrievingTextBinaryData.html#:~:text=To%20retrieve%20simple%20data%20such,from%20which%20data%20is%20retrieved.
	  // dont care, unity is a pain anyway.
		WWW w = new WWW("https://pastebin.com/raw/G0BjPuKk");
		// ? UnityWebRequest wr = UnityWebRequest.Get("https://pastebin.com/raw/G0BjPuKk");
		yield return w;
		// ? yield return wr.SendWebRequest();
		if (w.error != null)
		{
			Debug.Log("Error .. " + w.error);
			// for example, often 'Error .. 404 Not Found'
			text = "Failed";
		}
		else
		{
			Debug.Log("Found ... ==>" + w.text + "<==");
			// don't forget to look in the 'bottom section'
			// of Unity console to see the full text of
			// multiline console messages.
			text = w.text;
		}

		// ? if (wr.result != UnityWebRequest.Result.Success) Debug.Log("Error .. " + wr.error.ToString());
		// ? else Debug.Log(wr.downloadHandler.text);

		/* example code to separate all that text in to lines:
		longStringFromFile = w.text
		List<string> lines = new List<string>(
			longStringFromFile
			.Split(new string[] { "\r","
" },
StringSplitOptions.RemoveEmptyEntries) );
// remove comment lines…
lines = lines
.Where(line => !(line.StartsWith(“//”)
|| line.StartsWith(“#”)))
.ToList();
*/

	}
}