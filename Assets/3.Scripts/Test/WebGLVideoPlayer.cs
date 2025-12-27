using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLVideoPlayer : MonoBehaviour
{
    public void PlayOverlayVideo(string fileName)
    {
        var videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        videoPlayer.Play();
    }

    public void HideOverlayVideo()
    {

    }
}