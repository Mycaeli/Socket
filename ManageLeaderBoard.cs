using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using static HttpHandleAuth;

public class ManageLeaderBoard : MonoBehaviour
{
    string url = "https://sid-restapi.onrender.com";

    private UserJson user;
    private string Token;
    public GameObject panelLeaderBoard;
    public GameObject itemLeaderBoardPrefab;

    private List<GameObject> itemLeaderBoardList;
    

    void Start()
    {
        Token = PlayerPrefs.GetString("token");

        itemLeaderBoardList = new List<GameObject>();

        
    }

    public void CosultLeader()
    {
        StartCoroutine("GetLeaderBoard");
    }

    IEnumerator GetLeaderBoard()
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "/api/usuarios/" );
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
                UserJsonList list = JsonUtility.FromJson<UserJsonList>(request.downloadHandler.text);

                var leaderboard = list.userList = list.userList.OrderByDescending(user => user.data.score).Take(5).ToArray();
                ShowLeaderBoard(leaderboard);
            }
            else
            {

                Debug.Log(request.responseCode + "|" + request.error);
            }
        }

    }

    void ShowLeaderBoard(UserJson[] list )
    {
        itemLeaderBoardList.Clear();
        foreach(UserJson user in list)
        {
            GameObject item = GameObject.Instantiate(itemLeaderBoardPrefab, panelLeaderBoard.transform) as GameObject;
            ManageLeaderBoardItem(item, user);
        }
    }

    void ManageLeaderBoardItem(GameObject item, UserJson user)
    {
        itemLeaderBoardList.Add(item);
        LeaderBoardItem boardItem = item.GetComponent<LeaderBoardItem>();
        boardItem.SetItem(user,itemLeaderBoardList.Count);
    }
}
