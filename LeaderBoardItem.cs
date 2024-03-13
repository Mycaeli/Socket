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
    public RectTransform rectTransform;

    public void SetItem(UserJson user, int index)
    {
        TxtUsername.text = user.username;
        TxtScore.text = "" + user.data.score;

        rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top,100 * index, rectTransform.rect.height);


    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
