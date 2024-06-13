using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;

public class DiscordController : MonoBehaviour
{
    public long applicationID;
    [Space]
    public string details = "DETAILS";
    public string state = "STATE";
    [Space]
    public string largeImage = "expoint";
    public string largeText = "LARGE TEXT";

    private long time;

    private static bool instanceExists;
    public Discord.Discord discord;
#if (!UNITY_EDITOR)
	void Awake()
	{
		if (!instanceExists)
		{
			instanceExists = true;
			DontDestroyOnLoad(gameObject);
		}
		else if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		discord = new Discord.Discord(applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);

		time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

		UpdateStatus();
	}

	// Update is called once per frame
	void Update()
	{
		try
		{
			discord.RunCallbacks();
		}
		catch
		{
			Destroy(gameObject);
		}
	}

	void LateUpdate()
	{
		UpdateStatus();
		
		
	}

	void UpdateStatus()
	{
		try
		{
			var activityManager = discord.GetActivityManager();
			var activity = new Discord.Activity
			{
				Details = details,
				State = state,
				Assets =
				{
					LargeImage = largeImage,
					LargeText = largeText
				},
				Timestamps =
				{
					Start = time
				}
			};

			activityManager.UpdateActivity(activity, (res) =>
			{
				if (res != Discord.Result.Ok) Debug.LogWarning("Failed connecting to Discord");
			});

		}
		catch
		{
			Destroy(gameObject);
		}
	}
	
	void OnApplicationQuit()
	{
		var acrivityManager = discord.GetActivityManager();
		acrivityManager.ClearActivity((result) =>
		{
			if (result == Discord.Result.Ok)
			{
				print("yes");
			}
			else
			{
				print("no");
			}
		});
	}
	
#endif
}

