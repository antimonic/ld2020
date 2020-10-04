using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalAcceptor : MonoBehaviour
{
    public Transform acceptPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ball")
        {
            collision.gameObject.GetComponent<Collider>().enabled = false;
            collision.gameObject.GetComponent<Ball>().Drop();
            StartCoroutine(GrabBall(collision.gameObject.transform));            
        }
    }

    IEnumerator GrabBall(Transform ball)
    {
        ball.GetComponent<Collider>().enabled = false;
        ball.GetComponent<Rigidbody>().isKinematic = true;
        float t = 0;
        Vector3 initial = ball.transform.position;
        while(t < 0.5f)
        {
            t += Time.deltaTime;
            ball.transform.position = Vector3.Lerp(initial, acceptPoint.position, t / 0.5f);
            yield return new WaitForEndOfFrame();
        }
        t = 0;
        Vector3 end = acceptPoint.position - acceptPoint.up * 1;
        Vector3 initScale = ball.transform.localScale;
        while(t < 1)
        {
            t += Time.deltaTime;
            if (!ball)
            {
                continue;
            }
            ball.transform.position = Vector3.Lerp(acceptPoint.position, end, t);
            ball.transform.localScale = Vector3.Lerp(initScale, Vector3.zero, t);
            yield return new WaitForEndOfFrame();
        }
        Destroy(ball.gameObject);
        GameManager.Instance.GoalComplete();
    }
}
