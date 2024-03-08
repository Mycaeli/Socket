using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using static HttpHandleAuth;
using static GameCore.Score;
using GameCore;

public class ManageScore : MonoBehaviour
{
    string url = "https://sid-restapi.onrender.com";

    private UserJson user;
    private string Token;

    private Score score;

    void Start()
    {
        Token = PlayerPrefs.GetString("token");
        StartCoroutine(UpdateScore());
    }


    IEnumerator UpdateScore()
    {

        int newScore = GetCurrentScore();

        string jsonData = "{\"score\":" + newScore.ToString() + "}";

        UnityWebRequest request = UnityWebRequest.Put(url + "/api/usuarios/", jsonData);
        request.method = "PATCH";
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-token", Token);

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                user.data.score = GetCurrentScore();
            }
            else
            {

                Debug.Log(request.responseCode + "|" + request.error);
            }
        }

    }

    private int GetCurrentScore()
    {
        return score.score;
    }
}
