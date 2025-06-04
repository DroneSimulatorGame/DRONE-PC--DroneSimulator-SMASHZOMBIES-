using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "NewVideoPlayerDataScript", menuName = "Video/Video Player Data Script")]
public class VideoPlayerDataScript : ScriptableObject
{
    public List<VideoPlayerData> VideoPlayerDatas;
}

[Serializable]
public class VideoPlayerData
{
    [field: SerializeField]
    public int WaveID { get; private set; } // Integer wave ID
    [field: SerializeField]
    public VideoClip VideoClip { get; private set; } // VideoClip reference

    // Constructor to easily create new instances in code if needed
    public VideoPlayerData(int waveID, VideoClip videoClip)
    {
        WaveID = waveID;
        VideoClip = videoClip;
    }
}
