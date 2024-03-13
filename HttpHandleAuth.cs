using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class HttpHandleAuth : MonoBehaviour
{
    string url = "https://sid-restapi.onrender.com";

    public string Token { get; private set; }
    public string Username { get; private set; }
    public UserJson AuthenticatedUser { get; private set; }

    public GameObject PanelAuth;
    public GameObject PanelMenu;
    Scene currentScene;

    // Start is called before the first frame update
    void Start()
    {
        currentScene = SceneManager.GetActiveScene();

        //PlayerPrefs.SetString("token", Token);
        Token = PlayerPrefs.GetString("token");
        if (string.IsNullOrEmpty(Token))
        {
            Debug.Log("No Token");
            PanelAuth.SetActive(true);
        }
        else
        {
            Username= PlayerPrefs.GetString("username");
            StartCoroutine(GetProfile());
        }
    }

    public UserJson GetAuthenticatedUser()
    {
        return new UserJson
        {
            _id = PlayerPrefs.GetString("user_id"),
            username = PlayerPrefs.GetString("username"),
            data = new UserData
            {
                score = PlayerPrefs.GetInt("score", 0)
            }
        };
    }

    public void SendRegister()
    {
        AuthData authData = new();
        authData.username = GameObject.Find("InputField USERNAME").GetComponent<TMP_InputField>().text;
        authData.password = GameObject.Find("InputField PASSWORD").GetComponent<TMP_InputField>().text;

        StartCoroutine(Register(JsonUtility.ToJson(authData)));
    }

    public void SendLogIn()
    {
        AuthData authData = new();
        authData.username = GameObject.Find("InputField USERNAME").GetComponent<TMP_InputField>().text;
        authData.password = GameObject.Find("InputField PASSWORD").GetComponent<TMP_InputField>().text;

        StartCoroutine(LogIn(JsonUtility.ToJson(authData)));
    }

    IEnumerator Register(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(url+"/api/usuarios", json);
        request.method = "POST";
        request.SetRequestHeader("Content-Type","application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                Debug.Log("Register Succes");
                StartCoroutine(LogIn(json));
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }

    }

    IEnumerator LogIn(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(url + "/api/auth/login", json);
        request.method = "POST";
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                AuthData authData = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                Token = authData.token;
                Username = authData.usuario.username;

                PlayerPrefs.SetString("token", Token);
                PlayerPrefs.SetString("username", Username);

                Debug.Log(authData.token);
                PanelAuth.SetActive(false);
                PanelMenu.SetActive(true);

            }
            else
            {
                PanelAuth.SetActive(false);
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }

    }

    IEnumerator GetProfile()
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "/api/usuarios/" + Username);
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
                AuthData authData = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                AuthenticatedUser = authData.usuario;
                Debug.Log("Username: " + authData.usuario.username + " is authenticate | user score is: " + authData.usuario.data.score);

                if (currentScene.name == "_play")
                {
                    Debug.Log("Start Scene");
                    PanelAuth.SetActive(false);
                    PanelMenu.SetActive(true);
                }
                else if (currentScene.name == "GameScene")
                {
                    Debug.Log("Game Scene");
                }
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    public void LogOut()
    {
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.DeleteKey("user_id");
        PlayerPrefs.DeleteKey("score");

        Token = null;
        Username = null;
        AuthenticatedUser = null;

        SceneManager.LoadScene("_play"); // Replace "LoginScene" with the name of your login scene.
    }

    [System.Serializable]
    public class AuthData
    {
        public string username;
        public string password;
        public UserJson usuario;
        public string token;
    }
    [System.Serializable]
    public class UserJson
    {
        public string _id;
        public string username;
        public UserData data;
    }
    [System.Serializable]
    public class UserData
    {
        public int score;
    }

    [System.Serializable]
    public class UserJsonList 
    {
        public UserJson[] usuarios;
    }
}

