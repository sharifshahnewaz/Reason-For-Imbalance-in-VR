using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

public class RecordCOP : MonoBehaviour
{

    UIVA_Client_WiiFit theClient;
    string ipUIVAServer = "127.0.0.1";

    private double gravX = 0.0, gravY = 0.0, weight = 0.0, prevGravX = 0.0, prevGravY = 0.0, pathX = 0.0, pathY = 0.0, path = 0.0;
    private bool isFirstRow = true;
    private double tl = 0.0, tr = 0.0, bl = 0.0, br = 0.0;
    private string fitbutt = "";
    private double elapsedTime = 0.0f;
    private bool isWriting = false;
    private string displayMessage = null;
    private StringBuilder sb;

    public int sampleRate = 20;
    public String studyCondition;

    AudioPlayer audioPlayer;

    void Awake()
    {
        if (Application.platform == RuntimePlatform.WindowsWebPlayer ||
            Application.platform == RuntimePlatform.OSXWebPlayer)
        {
            if (Security.PrefetchSocketPolicy(ipUIVAServer, 843, 500))
            {
                Debug.Log("Got socket policy");
            }
            else
            {
                Debug.Log("Cannot get socket policy");
            }
        }

        audioPlayer = GetComponent<AudioPlayer>();
    }

    void Start()
    {
        try
        {
            theClient = new UIVA_Client_WiiFit(ipUIVAServer);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        displayMessage = "Press 'R' to start recording data";
        sb = new StringBuilder();
        sb.Append("System Time,Elapsed Time,Gravity X,Gravity Y, PathX, PathY, Path, Weight, Direction\n");
        StartCoroutine(WriteInFile());
    }

    IEnumerator WriteInFile()
    {
        //yield return new WaitForSeconds (0.001f);
        while (true)
        {
            try
            {
                theClient.GetWiiFitRawData(out tl, out tr, out bl, out br, out fitbutt);
                theClient.GetWiiFitGravityData(out weight, out gravX, out gravY, out fitbutt);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
            if (!isFirstRow)
            {
                pathX = Math.Abs(gravX - prevGravX);
                pathY = Math.Abs(gravY - prevGravY);
                path = Math.Sqrt(pathX * pathX + pathY * pathY);
            }
            if (isWriting)
            {
                sb.Append(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}\n",
                    System.DateTime.Now.ToString("hh.mm.ss.ffffff"), elapsedTime, gravX, gravY, pathX, pathY, path, weight, lookingDirection));
                prevGravX = gravX;
                prevGravY = gravY;
                isFirstRow = false;
                elapsedTime += (1.0f / sampleRate);
            }
            yield return new WaitForSeconds((1.0f / sampleRate));
        }

    }

    string lookingDirection = "front";

    string[] instructions = {
        // Start with normal navigation
        "left", "right", "top", "bottom", "front",
        "left", "right", "top", "bottom", "front",
        // Do a left right (horizontal) movement
        "left", "right", "left", "right",
        "left", "right", "left", "right", "front",
        // Do a top bottom (vertical) movement
        "top", "bottom", "top", "bottom",
        "top", "bottom", "top", "bottom", "front",
        // Do a clockwise movement
        "left", "top", "right", "bottom",
        "left", "top", "right", "bottom", "front",
        // Do a anti-clockwise movement
        "right", "top", "left", "bottom",
        "right", "top", "left", "bottom", "front",
        // Do a left right (horizontal) movement
        "left", "right", "left", "right",
        "left", "right", "left", "right", "front",
        // Do a top bottom (vertical) movement
        "top", "bottom", "top", "bottom",
        "top", "bottom", "top", "bottom", "front",
        // Do a clockwise movement
        "left", "top", "right", "bottom",
        "left", "top", "right", "bottom", "front",
        // Do a anti-clockwise movement
        "right", "top", "left", "bottom",
        "right", "top", "left", "bottom", "front",
        // End with normal navigation
        "left", "right", "top", "bottom", "front",
        "left", "right", "top", "bottom", "front",
    };

    public float delay = 1.5f;

    IEnumerator PlayInstructions()
    {
        audioPlayer.PlayAudio("readyConf");
        yield return new WaitForSeconds(2);
        audioPlayer.PlayAudio("three");
        yield return new WaitForSeconds(1);
        audioPlayer.PlayAudio("two");
        yield return new WaitForSeconds(1);
        audioPlayer.PlayAudio("one");
        yield return new WaitForSeconds(1);
        if (!studyCondition.ToLower().Contains("eyes"))
            audioPlayer.PlayAudio("lookCommand");
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < instructions.Length; i++)
        {
            lookingDirection = instructions[i];
            if (!studyCondition.ToLower().Contains("eyes"))
                audioPlayer.PlayAudio(lookingDirection);
            yield return new WaitForSeconds(delay);
        }
        isWriting = false;
        displayMessage = "Data recording is done";
        audioPlayer.PlayAudio("thankYou");
        yield return new WaitForSeconds(1);
        audioPlayer.PlayAudio("recordDone");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isWriting)
        {
            isWriting = true;
            displayMessage = "Data is recording...";
            StartCoroutine(PlayInstructions());
        }
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 30;

        GUI.color = Color.green;
        GUI.Label(new Rect(200, 100, Screen.width, Screen.height), displayMessage);

        GUI.color = Color.white;
        GUI.Label(new Rect(200, 200, Screen.width, Screen.height), "WiiFit Data:");
        GUI.Label(new Rect(200, 240, Screen.width, Screen.height), "Weight:\t\t" + weight.ToString());
        GUI.Label(new Rect(200, 280, Screen.width, Screen.height), "Grav X:\t\t" + gravX.ToString());
        GUI.Label(new Rect(200, 320, Screen.width, Screen.height), "Grav Y:\t\t" + gravY.ToString());

        GUI.color = Color.red;
        GUI.Label(new Rect(200, 400, Screen.width, Screen.height), "WiiFit Raw Data:");
        GUI.Label(new Rect(200, 440, Screen.width, Screen.height), "Top Left:\t\t" + tl.ToString());
        GUI.Label(new Rect(200, 480, Screen.width, Screen.height), "Bottom Left:\t" + bl.ToString());
        GUI.Label(new Rect(200, 520, Screen.width, Screen.height), "Top Right :\t" + tr.ToString());
        GUI.Label(new Rect(200, 560, Screen.width, Screen.height), "Bottom Right :\t" + br.ToString());

    }

    void OnApplicationQuit()
    {
        System.IO.File.AppendAllText("Data/" + studyCondition + "-balance-" + System.DateTime.Now.Ticks.ToString() + ".csv", sb.ToString());
    }
}




