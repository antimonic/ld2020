using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField]
    private Transform ballSpawn;

    private GameObject ball;

    public Vector3 GetBallPos()
    {
        return ball.transform.position;
    }

    public void DestroyBall()
    {
        if (ball)
        {
            Destroy(ball);
            ball = null;
        }
    }
    public void StartRound()
    {
        GameObject Ball = GameObject.Instantiate(GameManager.Instance.ballPrefab);
        ball = Ball;
        Ball.transform.position = ballSpawn.position;
    }

}
