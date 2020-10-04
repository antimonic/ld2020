using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerRobot : MonoBehaviour
{
    [SerializeField]
    public Transform head;
    [SerializeField]
    private Transform AttachPoint;

    private Rigidbody rbody;

    public float Speed = 1;

    private float yAngle = 0;
    private float xAngle = 0;

    private Ball focusedBall;
    private bool holding;
    private float grabSpeed = 0.2f;
    private Recorder rec;
    private Vector3 StartPos;

    
    // Start is called before the first frame update
    void Start()
    {
        rec = GetComponent<Recorder>(); 
        rbody = GetComponent<Rigidbody>();
        rec.StartRecording();
        StartPos = transform.position;
        xAngle = transform.rotation.eulerAngles.y;
        
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

    private void FixedUpdate()
    {
        Vector3 delta = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
        rbody.velocity = delta * Speed;
    }

    // Update is called once per frame
    void Update()
    {
        yAngle -= Input.GetAxis("Mouse Y") * GameManager.Instance.MouseSensitivity;
        yAngle = Mathf.Clamp(yAngle, -90, 90);

        xAngle += Input.GetAxis("Mouse X") * GameManager.Instance.MouseSensitivity;
        xAngle = xAngle % 360;

        head.localRotation = Quaternion.Euler(yAngle, 0, 0);

        transform.localRotation = Quaternion.Euler(0, xAngle, 0);

        if(Vector3.Distance(transform.position, StartPos) < 1){
            if (!GameManager.Instance.finishText.enabled)
            {
                GameManager.Instance.finishText.enabled = true;
            }
            if (Input.GetButtonDown("Finish"))
            {
                CompleteCycle();
            }
        }else if (GameManager.Instance.finishText.enabled)
        {
            GameManager.Instance.finishText.enabled = false;
        }


        if (Input.GetMouseButton(1))
        {
            GameManager.Instance.ghost.Possess();
            GameManager.Instance.currentPlayer = null;
            if (focusedBall)
            {
                focusedBall.UnHighlight();
            }
            Destroy(gameObject);
        }

        RaycastHit hitInfo;
        if (!holding)
        {
            if (Physics.SphereCast(head.position, 0.5f, head.forward, out hitInfo, 4, (1 << 9)))
            {
                Ball hitBall = hitInfo.collider.gameObject.GetComponent<Ball>();
                if (hitBall != focusedBall)
                {
                    if (focusedBall)
                    {
                        focusedBall.UnHighlight();
                    }
                    focusedBall = hitBall;
                    focusedBall.Highlight();
                }
                if (Input.GetButtonDown("Interact"))
                {
                    focusedBall.Drop();
                    GetComponent<Recorder>().StoreAction();
                    holding = true;
                    focusedBall.UnHighlight();
                    StartCoroutine(GrabBall());
                    focusedBall.OnBallDropped.AddListener(DropBall);
                }
            }else if (focusedBall)
            {
                focusedBall.UnHighlight();
                focusedBall = null;
            }
        }
        else
        {
            if (Input.GetButtonDown("Interact"))
            {
                GetComponent<Recorder>().StoreAction();
                DropBall();
            }
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
        GameManager.Instance.finishText.enabled = false;
        GameManager.Instance.PlayerFinishedRecording();
    }
}
