using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using static HttpHandleAuth;

public class ManageScore : MonoBehaviour
{
    string url = "https://sid-restapi.onrender.com";

    public HttpHandleAuth httpHandleAuth;
    private UserJson user;
    private string Token;

    private bool isUserInitialized = false;

    void Start()
    {
        Token = PlayerPrefs.GetString("token");
        StartCoroutine(WaitForUserInitialization());
    }

    IEnumerator WaitForUserInitialization()
    {
        while (user == null)
        {
            user = httpHandleAuth.AuthenticatedUser;
            yield return null; // Wait for next frame to recheck
        }

        isUserInitialized = true; // Set flag to true when user is initialized
        Debug.Log("Authenticated user data retrieved successfully.");
        Debug.Log(user.username + " " + user.data.score);
    }

    public void GetScore(int score)
    {
        StartCoroutine(GetScoreCoroutine(score));
    }

    IEnumerator GetScoreCoroutine(int score)
    {
        while (!isUserInitialized)
        {
            yield return null; // Wait until user data is initialized
        }

        StartCoroutine(UpdateScore(score));
    }


    IEnumerator UpdateScore(int score)
    {
        //yield return new WaitUntil(() => user != null); // Wait until user is not null

        if (user.data == null)
        {
            Debug.LogError("User or user data is null.");
            yield break; // exit the coroutine early
        }

        user.data.score = score;
        Debug.Log(user.username + " " + user.data.score);
        string jsonData = JsonUtility.ToJson(user.data);

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
                Debug.Log("Score updated successfully.");
            }
            else
            {

                Debug.Log(request.responseCode + "|" + request.error);
            }
        }        
    }
}
