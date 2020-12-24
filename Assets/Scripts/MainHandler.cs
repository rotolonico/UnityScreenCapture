using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
# if UNITY_EDITOR
using MS.Shell.Editor;
#endif

public class MainHandler : MonoBehaviour
{
    public static bool updateScreen;
    public static List<Color> screenPixels;

    public GameObject startPanel;
    public GameObject stopPanel;

    public TMP_InputField topInputField;
    public TMP_InputField leftInputField;
    public TMP_InputField widthInputField;
    public TMP_InputField heightInputField;
    public TMP_InputField monitorInputField;

    public RawImage image;
    private Texture2D texture;

    private ScreenRequester screenRequester;

    private Process test;

    private void Start()
    {
        topInputField.text = "0";
        leftInputField.text = "0";
        widthInputField.text = Screen.width.ToString();
        heightInputField.text = Screen.height.ToString();
        monitorInputField.text = "0";
    }

    public void StartScreenSharing()
    {
        screenRequester = new ScreenRequester();
        screenRequester.Start();

        if (int.TryParse(topInputField.text, out var top) && int.TryParse(leftInputField.text, out var left) &&
            int.TryParse(widthInputField.text, out var width) && int.TryParse(heightInputField.text, out var height) &&
            int.TryParse(monitorInputField.text, out var monitor))
        {
            StartServer(top, left, width, height, monitor);

            texture = new Texture2D(width, height);
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            image.texture = texture;
            image.enabled = true;

            startPanel.SetActive(false);
            stopPanel.SetActive(true);
        }
        else Debug.Log("Invalid input fields");
    }

    public void StopScreenSharing()
    {
        StopServer();

        screenRequester.Stop();
        image.enabled = false;

        startPanel.SetActive(true);
        stopPanel.SetActive(false);
    }

    private void Update()
    {
        if (!updateScreen) return;
        UpdateScreen();
    }

    private void UpdateScreen()
    {
        texture.SetPixels(screenPixels.ToArray());
        texture.Apply();
        updateScreen = false;
    }

    private void OnDestroy()
    {
        StopServer();

        screenRequester.Stop();
    }

    private void StartServer(int top, int left, int width, int height, int monitor)
    {
# if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            test = Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = true,
            Arguments = $"/C {Application.streamingAssetsPath}/venv-windows/Scripts/python.exe {Application.streamingAssetsPath}/server.py {top} {left} {width} {height} {monitor}"
        });
# else
        Process.Start(new ProcessStartInfo
        {
            FileName = "/bin/sh",
            UseShellExecute = true,
            RedirectStandardOutput = false,
            Arguments = $"{Application.streamingAssetsPath}/server.sh {top} {left} {width} {height} {monitor}"
        });
# endif
    }

    private void StopServer()
    {
# if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = true,
            Arguments = "/C taskkill /IM python.exe /F"
        });
# else
        Process.Start(new ProcessStartInfo
        {
            FileName = "/bin/sh",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            Arguments = $"{Application.streamingAssetsPath}/kill_server.sh"
        });
# endif
    }
}