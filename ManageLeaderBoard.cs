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

    private string Token;
    public GameObject panelLeaderBoard;
    public GameObject itemLeaderBoardPrefab;

    private List<GameObject> itemLeaderBoardList;
    private UserJson[] userList;


    void Start()
    {
        Token = PlayerPrefs.GetString("token");

        itemLeaderBoardList = new List<GameObject>();
        userList = null;
    }

    public void CosultLeader()
    {
        StartCoroutine(GetLeaderBoard());
    }

    IEnumerator GetLeaderBoard()
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "/api/usuarios");
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
                if (list != null && list.usuarios != null && list.usuarios.Any())
                {
                    userList = list.usuarios;
                    var leaderboard = userList
                        .OrderByDescending(user => user.data?.score ?? 0) // Order by score (or use 0 if score is missing)
                        .Take(4) // Take top 4 users
                        .ToArray();
                    ShowLeaderBoard(leaderboard);
                }
                else
                {
                    // Handle the case when the list is null or empty
                    Debug.LogWarning("User list is null or empty.");
                    Debug.Log(request.downloadHandler.text);

                }
            }
            else
            {

                Debug.Log(request.responseCode + "|" + request.error);
            }
        }

    }

    void ShowLeaderBoard(UserJson[] list)
    {
        // Deactivate the original item if it exists
        GameObject originalItem = panelLeaderBoard.transform.GetChild(0).gameObject;
        if (originalItem != null)
        {
            originalItem.SetActive(true);
        }

        DeactivateLeaderBoardItems();

        foreach (UserJson user in list)
        {
            GameObject item = GameObject.Instantiate(itemLeaderBoardPrefab, panelLeaderBoard.transform) as GameObject;
            ManageLeaderBoardItem(item, user);
        }

        if (userList != null)
        {
            Array.Copy(list, userList, Math.Min(list.Length, userList.Length));
        }

        if (originalItem != null)
        {
            originalItem.SetActive(false);
        }
    }

    void DeactivateLeaderBoardItems()
    {
        foreach (var item in itemLeaderBoardList)
        {
            LeaderBoardItem boardItem = item.GetComponent<LeaderBoardItem>();
            if (boardItem != null)
            {
                boardItem.Deactivate();
                GameObject.Destroy(item); // Destroy the clone
            }
        }
        itemLeaderBoardList.Clear(); // Clear the list after destroying clones
    }

    void ManageLeaderBoardItem(GameObject item, UserJson user)
    {
        if (item == null)
        {
            Debug.LogError("Item GameObject is null.");
            return;
        }

        LeaderBoardItem boardItem = item.GetComponent<LeaderBoardItem>();
        if (boardItem == null)
        {
            Debug.LogError("LeaderBoardItem component not found on the item GameObject.");
            return;
        }

        itemLeaderBoardList.Add(item);
        Debug.Log(itemLeaderBoardList.Count);
        boardItem.SetItem(user, itemLeaderBoardList.Count);
    }
}
