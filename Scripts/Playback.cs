using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;

public class Playback : MonoBehaviour
{
    private bool playback = false;
    private Robot target; 

    public UnityEvent OnFinishedPlayback;
    private Recorder.Recording data;

    private void Awake()
    {
        target = GetComponent<Robot>();
    }
    public void SetData(Recorder.Recording recording)
    {
        data = recording;
    }
    public void StartPlayback()
    {
        StopAllCoroutines();
        playback = true;
        StartCoroutine(PlaybackRecording());
    }


    public void RestartPlayback()
    {
        if (!playback)
        {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(PlaybackRecording());
    }

    IEnumerator PlaybackRecording()
    {
        int index = 1;
        transform.position = data.startPosition;
        Recorder.Motion toLoad = data.motions[0];
        target.LoadData(toLoad);
        Quaternion initialRot = transform.rotation;
        int nextAction = 0;
        float playbackTime = 0;
        float deltaTime = 0;
        float TimeUntilNext = data.motions[1].absoluteTime;
        yield return new WaitForFixedUpdate();
        while (playback)
        {
            playbackTime += Time.fixedDeltaTime;
            deltaTime += Time.fixedDeltaTime;
            Recorder.Motion midLoad = new Recorder.Motion();
            midLoad.movement = Vector3.Lerp(toLoad.movement, data.motions[index].movement, deltaTime / TimeUntilNext);
            midLoad.headRot = Quaternion.Lerp(toLoad.headRot, data.motions[index].headRot, deltaTime / TimeUntilNext);
            midLoad.totalRot = Quaternion.Lerp(toLoad.totalRot, data.motions[index].totalRot, deltaTime / TimeUntilNext);
            target.LoadData(midLoad);
            while(playbackTime >= data.motions[index].absoluteTime)
            {
                toLoad = data.motions[index];
                deltaTime = 0;
                index++;
                if(index >= data.motLength)
                {
                    OnFinishedPlayback.Invoke();
                    playback = false;
                    break;
                }
                TimeUntilNext = data.motions[index].absoluteTime - data.motions[index - 1].absoluteTime;
            }

            while(nextAction < data.actLength && playbackTime > data.actions[nextAction].absoluteTime)
            {
                //Do Action
                target.DoAction(data.actions[nextAction]);
                nextAction++;
            }
            yield return new WaitForFixedUpdate();
        }
        target.FinishedPlayback();
        Quaternion endRot = transform.rotation;
        Vector3 endPos = transform.position;
        float t = 0;
        while(t < 1f)
        {
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(endRot, initialRot, t);
            transform.position = Vector3.Lerp(endPos, data.startPosition, t);
        }
        yield return new WaitForSeconds(0.2f);
        GameManager.Instance.OnPlaybackFinished();
    }

}
