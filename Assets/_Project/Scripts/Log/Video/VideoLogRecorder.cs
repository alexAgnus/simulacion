using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VideoLogRecorder : MonoBehaviour
{
    [Range(1f, 144f)] public float framerate;
    private string _logCreationDate;
    private string _logOutputPath;
    private Coroutine _recordingCoroutine;
    private const string WinFfmpegPath = @"Assets\_Project\Scripts\Log\Video\FFMPEG\Windows\ffmpeg-7.0-full_build\bin\ffmpeg.exe";

    private void Start()
    {
        _logCreationDate = Log.logCreationDate;
        _logOutputPath = Log.logOutputPath;
        GenerateVideoData();
    }

    private void GenerateVideoData()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass configClass = new AndroidJavaClass("com.arthenica.ffmpegkit.FFmpegKitConfig");
        AndroidJavaObject paramVal = new AndroidJavaClass("com.arthenica.ffmpegkit.Signal").GetStatic<AndroidJavaObject>("SIGXCPU");
        configClass.CallStatic("ignoreSignal", new object[] { paramVal });
        _recordingCoroutine = StartCoroutine(Recording());
#elif UNITY_EDITOR
        if (File.Exists(WinFfmpegPath))
        {
            _recordingCoroutine = StartCoroutine(Recording());
        }
#endif
    }

    private IEnumerator Recording()
    {
        string framesPath = Path.Combine(_logOutputPath, "Frames");
        if (!Directory.Exists(framesPath))
        {
            Directory.CreateDirectory(framesPath);
        }
        float frameInterval = 1 / framerate;
        int frameCount = 0;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(Path.Combine(framesPath, $"Frame_{frameCount:D08}.png"));
            frameCount++;
            yield return new WaitForSeconds(frameInterval);
        }
    }

    public static void RunFFmpegCommand(string ffmpegCommand)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass ffmpegKit = new AndroidJavaClass("com.arthenica.ffmpegkit.FFmpegKit");
        AndroidJavaObject session = ffmpegKit.CallStatic<AndroidJavaObject>("execute", ffmpegCommand);
        AndroidJavaObject returnCode = session.Call<AndroidJavaObject>("getReturnCode");
        int rc = returnCode.Call<int>("getValue");

        if (rc == 0)
        {
            Debug.Log("Video created successfully!");
        }
        else
        {
            Debug.LogError($"FFmpegKit failed with return code {rc}");
        }
#elif UNITY_EDITOR
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {WinFfmpegPath} {ffmpegCommand}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            Debug.Log("Video created successfully!");
        }
        else
        {
            Debug.LogError("FFmpeg failed to create video.");
        }
#endif
    }

    private void CreateRecording()
    {
        string ffmpegCommand = $"-framerate {framerate} -i \"{_logOutputPath}\\Frames\\Frame_%08d.png\" -c:v libx264 -pix_fmt yuv420p \"{_logOutputPath}\\Log_{_logCreationDate}.mp4\"";
        RunFFmpegCommand(ffmpegCommand);
        Directory.Delete($"{_logOutputPath}/Frames", true);
    }

    private void OnDestroy()
    {
        if (_recordingCoroutine != null)
        {
            StopCoroutine(_recordingCoroutine);
        }
        CreateRecording();
    }
}