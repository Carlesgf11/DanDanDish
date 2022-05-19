using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public enum PlayerState { CHOOSE, ACTION, MOVE, DIE };
    public PlayerState state;
    public List<Transform> checkPoints;
    public int currentCheckpoint;
    public Transform cameraTarget;
    public int cameraX;


    private void Start()
    {
        currentCheckpoint = 5;
    }

    private void Update()
    {
        switch (state)
        {
            case PlayerState.CHOOSE:
                break;
            case PlayerState.ACTION:
                break;
            case PlayerState.MOVE:
                MoveUpdate();
                break;
            case PlayerState.DIE:
                break;
        }
    }

    public void MoveUpdate()
    {
        Vector2 finalPos = new Vector2(checkPoints[currentCheckpoint].position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, finalPos, 10 * Time.deltaTime);
        float distance = Vector2.Distance(finalPos, transform.position);
        if (distance <= 0.02f)
        {
            transform.position = finalPos;
            state = PlayerState.CHOOSE;
        }
    }

    public void Win()
    {
        currentCheckpoint++;
        cameraTarget.parent = gameObject.transform;
        cameraTarget.transform.localPosition = new Vector3(cameraX, 3, -10);
        state = PlayerState.MOVE;
    }

    public void Lose()
    {
        currentCheckpoint--;
        Vector2 finalPos = new Vector2(checkPoints[currentCheckpoint].position.x, transform.position.y);
        transform.position = finalPos;
        state = PlayerState.CHOOSE;
    }
}
