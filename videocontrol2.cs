using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Linq;


public class videocontrol2 : MonoBehaviour
{

    //Video To Play [Assign from the Editor]
    public VideoClip videoToPlay;
    public RenderTexture renderTexture;

    private VideoPlayer videoPlayer;
    private VideoSource videoSource;

    //Set from the Editor
    public List<VideoClip> videoClipList;

    private List<VideoPlayer> videoPlayerList;
    private int videoIndex = 0;
    Coroutine m_MyCoroutineReference;

    bool notPrepared;


    // Use this for initialization
    void Start()
    {
        Application.runInBackground = true;

        m_MyCoroutineReference = StartCoroutine(playVideo());
    }



    IEnumerator playVideo(bool firstRun = true)
    {
        if (videoClipList == null || videoClipList.Count <= 0)
        {
            Debug.Log("error1b");

            yield break;

        }

        notPrepared = true;

        if (firstRun)
        {
            var numberList = Enumerable.Range(0, videoClipList.Count).ToList();

            videoPlayerList = new List<VideoPlayer>();
            for (int i = 0; i < videoClipList.Count; i++)
            {
                System.Random rnd = new System.Random();
                int remove = rnd.Next(numberList.Count - 1);


                //Create new Object to hold the Video and the sound then make it a child of this object
                GameObject vidHolder = new GameObject("VP" + i);
                vidHolder.transform.SetParent(transform);


                //Add VideoPlayer to the GameObject
                VideoPlayer videoPlayer = vidHolder.AddComponent<VideoPlayer>();
                videoPlayerList.Add(videoPlayer);

                //Disable Play on Awake for both Video and Audio
                videoPlayer.playOnAwake = false;

                videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;

                videoPlayer.targetTexture = renderTexture;

                videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

                //We want to play from video clip not from url
                videoPlayer.source = VideoSource.VideoClip;

                //Set video Clip To Play 
                videoPlayer.clip = videoClipList[numberList[remove]];

                numberList.RemoveAt(remove);

            }

            //Prepare video
            videoPlayerList[videoIndex].Prepare();
        }

        //Make sure that the NEXT VideoPlayer index is valid
        if (videoIndex >= videoPlayerList.Count)
        {
            Debug.Log("error2b");
            yield break;

        }





        //Wait until this video is prepared
        while (!videoPlayerList[videoIndex].isPrepared)
        {
            Debug.Log("waitingb");
            yield return null;
        }


        //Play first video
        videoPlayerList[videoIndex].Play();

        //Wait while the current video is playing
        bool reachedHalfWay = false;
        int nextIndex = (videoIndex + 1);
        while (videoPlayerList[videoIndex].isPlaying)
        {


            //(Check if we have reached half way)
            if (!reachedHalfWay && videoPlayerList[videoIndex].time >= (videoPlayerList[videoIndex].clip.length / 2))
            {
                reachedHalfWay = true; //Set to true so that we don't evaluate this again

                //Make sure that the NEXT VideoPlayer index is valid. Othereise Exit since this is the end
                if (nextIndex >= videoPlayerList.Count)
                {


                    Destroy(videoPlayerList[videoIndex]);
                    videoIndex = 0;
                    Debug.Log("restartB");
                    StartCoroutine(playVideo());
                    yield break;

                }

                //Prepare the NEXT video
                notPrepared = false;
                videoPlayerList[nextIndex].Prepare();
            }
            yield return null;
        }

        Destroy(videoPlayerList[videoIndex]);
        //Wait until NEXT video is prepared
        Debug.Log("waiting2b");
        Debug.Log((videoPlayerList[nextIndex]).clip);
        while (!videoPlayerList[nextIndex].isPrepared)
        {

            if (notPrepared)
            {

                videoPlayerList[nextIndex].Prepare();
                notPrepared = false;
            }

            yield return null;
        }



        //Increment Video index
        videoIndex++;


        //Play next prepared video. Pass false to it so that some codes are not executed at-all
        StartCoroutine(playVideo(false));

    }







}
