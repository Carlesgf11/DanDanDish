using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowControl : MonoBehaviour
{
    public float speed;
    public Vector3 arrowDir;
    public AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        ShootArrow(arrowDir);
    }

    public void ShootArrow(Vector3 _dir)
    {
        transform.Translate(_dir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            ArrowEvent("ArrowImpact_Body");
        }
        if(other.tag == "Arrow")
        {
            ArrowEvent("ArrowVsArrow");
        }
        if (other.tag == "Shield")
        {
            ArrowEvent("ArrowImpact_Wood");
        }
    }

    private void ArrowEvent(string _sound)
    {
        audioManager.PlaySound(_sound);
        Destroy(gameObject);
    }
}
