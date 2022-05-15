using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public InputField nameField;
    public InputField passwordField;
    public Button submitButton;

    private static readonly HttpClient client = new HttpClient();

    public void GetData ()
    {
        var request = (HttpWebRequest)WebRequest.Create("https://localhost:7191/api/Users");

        var response = (HttpWebResponse)request.GetResponse();

        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

        Debug.Log(responseString);
    }
}
