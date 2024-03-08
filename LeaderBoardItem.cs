using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static HttpHandleAuth;

public class LeaderBoardItem : MonoBehaviour
{
    public string Username { get; private set; }
    public int Score { get; private set; }
    public int Position { get; private set; }

    public TMP_Text TxtUsername;
    public TMP_Text TxtScore;

    public void SetItem(UserJson user, int position)
    {
        TxtUsername.text = user.username;
        TxtScore.text = "" + user.data.score;

        transform.position = new Vector3(transform.position.x, 100 - (position * 100), 0);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
