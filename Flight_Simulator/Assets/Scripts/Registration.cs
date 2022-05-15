using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Registration : MonoBehaviour
{
    public InputField nameField;
    public InputField passwordField;
    public Dropdown countryField;
    public Button submitButton;

    private static readonly HttpClient client = new HttpClient();

    public void SendData()
    {
        var request = (HttpWebRequest)WebRequest.Create("http://fsapi.thetomcatproject.org/api/Users");

        var str = string.Format("\"username\": \"{0}\",\"password\": \"{1}\",\"country\": \"{2}\"", nameField.text, passwordField.text, countryField.options[countryField.value].text);
        var data = Encoding.ASCII.GetBytes("{" + str + "}");

        request.Method = "POST";
        request.ContentType = "application/json";
        request.ContentLength = data.Length;

        using (var stream = request.GetRequestStream())
        {
            stream.Write(data, 0, data.Length);
        }

        var response = (HttpWebResponse)request.GetResponse();

        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

        //Dictionary<string, string> data = new Dictionary<string, string>();
        //data.Add("Username", nameField.text);
        //data.Add("Password", passwordField.text);
        //data.Add("Country", countryField.options[countryField.value].text);

        //var content = new FormUrlEncodedContent(data);
        //var resonse = await client.PostAsync("https://localhost:7191/api/Users", content);
        //var str = await resonse.Content.ReadAsStringAsync();
    }
}
