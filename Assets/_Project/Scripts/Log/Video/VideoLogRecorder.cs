using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VideoLogRecorder : MonoBehaviour
{
    public Camera recordingCamera;
    private string _logCreationDate;
    private string _logOutputPath;
    private Coroutine _recordingCoroutine;
    private int _height;
    private int _width;
    private float _framerate;

    private string _win_ffmpeg_path;
    
    private void Awake()
    {
        GenerateVideoData();
    }

    private void Start()
    {
        _logCreationDate = Log.logCreationDate;
        _logOutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            $"My Games/Bike AR/Logs/Log_{_logCreationDate}");
        _recordingCoroutine = StartCoroutine(Recording());
    }

    private void GenerateVideoData()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
            // FFMPEG ANdroid
        #elif UNITY_EDITOR && UNITY_STANDALONE
            _win_ffmpeg_path = Path.Combine(Application.dataPath, "_Project/Scripts/Log/Video/Windows/ffmpeg-7.0-full_build/bin/ffmpeg.exe");
#endif
    }

    private IEnumerator Recording()
    {
        yield return new WaitForSeconds(1/_framerate);
    }

    private void CreateRecording()
    {
    }

    private void OnDestroy()
    {
        CreateRecording();
    }

    private void OnApplicationQuit()
    {
        CreateRecording();
    }
}