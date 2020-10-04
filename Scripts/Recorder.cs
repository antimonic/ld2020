using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Recorder : MonoBehaviour
{
    public class Recording {
        public Vector3 startPosition;
        public Motion[] motions;
        public Action[] actions;
        public int motLength;
        public int actLength;
        public Recording()
        {
            motions = new Motion[1024];
            motLength = 0;
            actions = new Action[32];
        }
    }

    public float Frequency = 10;

    private bool isRecording = false;
    private Recording recording;
    private int rindex;
    private int aindex;


    public Recording GetRecording()
    {
        return recording;
    }
    
    //can add detail later
    public void StoreAction()
    {
        if (!isRecording)
        {
            Debug.LogError("Tried to record while not recording");
            return;
        }
        Action act = new Action();
        act.absoluteTime = GameManager.Instance.timeSinceStart;
        recording.actions[aindex] = act;
        aindex++;
    }

    public class Motion
    {
        public float absoluteTime;
        public Vector3 movement;
        public Quaternion totalRot;
        public Quaternion headRot;
    }

    public class Action
    {
        public enum ActionType { 
            Drop,
            Throw
        }

        public float absoluteTime;
        //Something about action
    }

    public virtual void StartRecording()
    {
        recording = new Recording();
        recording.startPosition = transform.position;
        rindex = 0;
        aindex = 0;
        isRecording = true;
        StartCoroutine(RecordingCoroutine());
    }

    IEnumerator RecordingCoroutine()
    {
        float recordTime = 1 / Frequency;
        StoreTransform();
        float timeSinceLast = 0;
        while (isRecording)
        {
            timeSinceLast += Time.fixedDeltaTime;
            if (timeSinceLast >= recordTime)
            {
                timeSinceLast = 0;
                StoreTransform();
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public virtual void StopRecording()
    {
        StoreTransform();
        recording.motLength = rindex;
        recording.actLength = aindex;
        isRecording = false;
    }


    private void StoreTransform()
    {
        Motion act = new Motion();
        act.movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        act.totalRot = transform.rotation;
        act.headRot = GetComponent<PlayerRobot>().head.transform.localRotation;
        act.absoluteTime = GameManager.Instance.timeSinceStart;
        if(rindex >= 1024)
        {
            Debug.LogError("Recording Overflow");
        }
        recording.motions[rindex] = act;
        rindex++;
    }


}
