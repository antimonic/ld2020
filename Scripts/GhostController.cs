using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    public bool isPossessed = false;
    public GameObject spawnEffect;
    public LayerMask spawnableLayers;
    public float Speed;

    private Camera cam;
    private float yLookAngle;
    private float xLookAngle;
    private bool useMouse;

    private void Start()
    {
        EnableMouse();
        cam = GetComponent<Camera>();
        spawnEffect.SetActive(false);
        isPossessed = false;
        cam.enabled = false;
        Possess();
    }

    public void Possess()
    {
        isPossessed = true;
        cam.enabled = true;
    }
    public void UnPossess()
    {
        isPossessed = false;
        cam.enabled = false;
    }

    public void DisableMouse()
    {
        useMouse = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void EnableMouse()
    {
        useMouse = true;
        Cursor.lockState = CursorLockMode.Locked; 
    }

    // Update is called once per frame
    void Update()
    {
        if (isPossessed && useMouse)
        {
            yLookAngle -= Input.GetAxis("Mouse Y");
            yLookAngle = Mathf.Clamp(yLookAngle, -90, 90);

            xLookAngle += Input.GetAxis("Mouse X");
            xLookAngle = xLookAngle % 360;

            transform.localRotation = Quaternion.Euler(yLookAngle, xLookAngle, 0);

            transform.position += Speed * Time.deltaTime * (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal") + Vector3.up * Input.GetAxis("CamVert"));

            RaycastHit hitInfo;
            if(Physics.Raycast(transform.position, transform.forward, out hitInfo, 4, spawnableLayers))
            {
                if (!spawnEffect.activeSelf)
                {
                    spawnEffect.SetActive(true);
                }
                spawnEffect.transform.position = hitInfo.point;
                spawnEffect.transform.rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.forward);
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject Robot = GameObject.Instantiate(GameManager.Instance.playerRobotPrefab);
                    Robot.transform.position = hitInfo.point;
                    Robot.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up), Vector3.up);
                    GameManager.Instance.currentPlayer = Robot.GetComponent<PlayerRobot>();
                    GameManager.Instance.PlaySimRound();
                    //spawnEffect.SetActive(false);
                    UnPossess();
                }
            }else if (spawnEffect.activeSelf)
            {
                spawnEffect.SetActive(false);
            }
        }
    }

}
