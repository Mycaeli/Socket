using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class HttpHandle : MonoBehaviour
{
    public RawImage[] images;
    public TextMeshProUGUI[] labels;
    public TextMeshProUGUI usertxt;
    public TMP_Dropdown userDropdown;
    string my_url = "https://my-json-server.typicode.com/Mycaeli/Socket";
    string url = "https://rickandmortyapi.com/api";
    Coroutine sendRequest;
    public void Handle()
    {
        int selectedUserId = userDropdown.value + 1;
        Debug.Log(selectedUserId);
        if (sendRequest != null)
        {
            StopCoroutine(sendRequest);
            sendRequest = null;
        }
        sendRequest = StartCoroutine(GetUser(selectedUserId));
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get(url + "/character");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if(www.responseCode== 200)
            {
                // Show results as text
                //Debug.Log(www.downloadHandler.text);
                CharacterList characters = JsonUtility.FromJson<CharacterList>(www.downloadHandler.text);

                foreach(Character character in characters.results)
                {
                    Debug.Log(character.name + " is a " + character.species);
                }               
            }
            else
            {
                Debug.Log(www.responseCode + "|" + www.error);
            }
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }

    IEnumerator GetUser(int uid)
    {
        UnityWebRequest www = UnityWebRequest.Get(my_url + "/users/" + uid);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                UserData user = JsonUtility.FromJson<UserData>(www.downloadHandler.text);
                usertxt.text = "Player: ";
                usertxt.text = usertxt.text + user.username;

                int index = 0;
                foreach (int cardId in user.deck)
                {
                    StartCoroutine(GetCharacter(cardId, index));
                    index++;
                }
            }
            else
            {
                Debug.Log(www.responseCode + "|" + www.error);
            }
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }

    IEnumerator GetCharacter(int id, int index)
    {
        UnityWebRequest www = UnityWebRequest.Get(url + "/character/" + id);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                // Show results as text
                //Debug.Log(www.downloadHandler.text);
                Character character = JsonUtility.FromJson<Character>(www.downloadHandler.text);
                //Debug.Log(character.name + " is a " + character.species);

                labels[index].text= character.name;

                StartCoroutine(DonwloadImage(character.image, index));

            }
            else
            {
                Debug.Log(www.responseCode + "|" + www.error);
            }
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }

    IEnumerator DonwloadImage(string url, int index)
    {
        UnityWebRequest request= UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
            images[index].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }
}

public class UserData
{
    public int id;
    public string username;
    public int[] deck; 
}

[System.Serializable]
public class CharacterList
{
    public Character[] results;
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string species;
    public string image;
}
