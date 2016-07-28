using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class Login : MonoBehaviour, Service {
    public InputField usernameText;
    public InputField pwdText;
    public Text msgText;
    private string username;
    private string pwd;
    public Client sockClient;

    private const ushort CID_LOGIN       = 0x1001;
    private const ushort CID_CREATE_USER = 0x1002;

    void Start () {
        sockClient.Register(SID.MSG_SC_LOGIN, this);
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (usernameText.isFocused)
            {
                pwdText.Select();
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Process(int len, BinaryReader reader)
    {
        throw new NotImplementedException();
    }

    public void DoLogin()
    {
        username = usernameText.text;
        pwd = pwdText.text;
        if (username.Length == 0 || pwd.Length == 0)
        {
            StartCoroutine(ShowMessage("fill form", 2));
        }

        List<System.Object> values = new List<System.Object>();
        values.Add(SID.MSG_CS_LOGIN);
        values.Add(CID_LOGIN);
        values.Add(username);
        values.Add(pwd);
        sockClient.TryWriter(values);
    }

    IEnumerator ShowMessage(string message, float delay)
    {
        msgText.text = message;
        msgText.enabled = true;
        yield return new WaitForSeconds(delay);
        msgText.enabled = false;
    }
}
