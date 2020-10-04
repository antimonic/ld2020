using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [SerializeField]
    public Transform head;
    [SerializeField]
    private Transform AttachPoint;

    public Transform ballBone;

    private Rigidbody rbody;

    public float Speed = 1;

    private float yAngle = 0;
    private float xAngle = 0;

    private Ball focusedBall;
    private bool holding;
    private float grabSpeed = 0.2f;
    private Recorder rec;
    private Vector2 moveInput;

    public void FinishedPlayback()
    {
        moveInput = Vector2.zero;
    }
    public void LoadData(Recorder.Motion mot)
    {
        transform.rotation = mot.totalRot;
        head.localRotation = mot.headRot;
        moveInput = mot.movement;
    }
    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
    }

    public void DropBall()
    {
        if (holding)
        {
        focusedBall.GetComponent<Rigidbody>().useGravity = true;
            focusedBall.transform.SetParent(null);
            holding = false;
            focusedBall = null;
        }
    }

    private void Update()
    {
        Debug.DrawRay(ballBone.position, Vector3.Cross(rbody.velocity.normalized, Vector3.up));
        ballBone.Rotate(Vector3.Cross(rbody.velocity.normalized, Vector3.up), rbody.velocity.magnitude * Time.deltaTime * -360, Space.World);
    }

    private void FixedUpdate()
    {
        Vector3 delta = transform.forward * moveInput.y + transform.right * moveInput.x;
        rbody.velocity = delta * Speed;
    }

    public void DoAction(Recorder.Action act)
    {
        if (!holding)
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(head.position, 0.5f, head.forward, out hitInfo, 4, (1 << 9)))
            {
                Ball hitBall = hitInfo.collider.gameObject.GetComponent<Ball>();
                hitBall.Drop();

                holding = true;
                focusedBall = hitBall;
                StartCoroutine(GrabBall());
                focusedBall.OnBallDropped.AddListener(DropBall);
            }
            else
            {
                Debug.Log("MISSED");
            }
        }
        else
        {
            DropBall();
        }
    }

    IEnumerator GrabBall()
    {
        float t = 0;
        Vector3 startPos = focusedBall.transform.position;
        focusedBall.GetComponent<Rigidbody>().useGravity = false;
        while(t < grabSpeed)
        {
            t += Time.deltaTime;
            focusedBall.transform.position = Vector3.Lerp(startPos, AttachPoint.position, t / grabSpeed);
            yield return new WaitForEndOfFrame();
        }
        focusedBall.transform.SetParent(AttachPoint);
        focusedBall.transform.localPosition = Vector3.zero;
        while (holding)
        {
            Vector3 diff = AttachPoint.position - focusedBall.transform.position;
            focusedBall.GetComponent<Rigidbody>().velocity = diff.normalized * 20 * Mathf.Min(diff.magnitude, 0.4f);
            if (diff.magnitude > 2f)
            {
                focusedBall.transform.SetParent(null);
                holding = false;
                focusedBall = null;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void CompleteCycle()
    {
        rec.StopRecording();
    }
}
