﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class NetworkCapabilities : MonoBehaviour {

	private BotController bController;
	private UIManager uiManager;
	private GameManager gManager;

	// Use this for initialization
	void Start () {
		GameObject botGO = GameObject.Find ("Bot");
		if (botGO != null)
		{
			bController = botGO.GetComponent<BotController> ();
		}

		GameObject uiManGO = GameObject.Find ("UIManager");
		if (uiManGO != null)
		{
			uiManager = uiManGO.GetComponent<UIManager> ();
		}

		GameObject gManGO = GameObject.Find ("GameManager");
		if (gManGO != null)
		{
			gManager = gManGO.GetComponent<GameManager> ();
		}
	}

	public void getScoreList(string levelName)
	{
		StartCoroutine (GetHighScoreList (levelName));
	}

	public void SendScore(string username)
	{
		StartCoroutine (SendScoreAsync (username));
	}

	IEnumerator SendScoreAsync(string username)
	{
		int score = bController.LevelDef.maxScore;

		WWWForm data = new WWWForm ();
		data.AddField ("levelname", bController.LevelDef.name);
		data.AddField ("username", username);
		data.AddField ("score", bController.LevelDef.maxScore);

		WWW request = new WWW ("http://31.220.57.122/kirosWeb/lightbot_server/public/score/add", data);

		yield return request;

		var jsonResponse = JSON.Parse (request.text);

		if (jsonResponse != null)
		{
			string result = jsonResponse ["result"].Value;

			string message = "";
			int countDown = 3;

			if (result == "Ok")
			{
				message = "Score sent correctly";
			} else
			{
				message = "There has been a problem";
			}

			while (countDown > 0)
			{
				uiManager.SendScoreMessage (message + " ..." + countDown);
				countDown--;
				yield return new WaitForSeconds (1);
			}

			gManager.ChangeScene ("LevelList");
		}

		yield return null;
	}

	IEnumerator GetHighScoreList(string levelName)
	{
		WWW request = new WWW ("http://31.220.57.122/kirosWeb/lightbot_server/public/score/list?levelname=" + levelName);

		yield return request;

		var JSONResponse = JSON.Parse (request.text);

		if (JSONResponse != null)
		{
			string result = JSONResponse ["result"].Value;

			if (result == "Ok")
			{
				JSONArray list = JSONResponse ["data"].AsArray;
				List<string> scoreList = new List<string> ();

				for (int i = 0; i < list.Count; i++)
				{
					string scoreText = list [i] ["username"] + ": " + list [i] ["score"];
					scoreList.Add (scoreText);
				}

				uiManager.SetScoreList (scoreList);
			}
		}
		yield return null;
	}
}
