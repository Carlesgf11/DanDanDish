using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowControl : MonoBehaviour
{
    public float speed;
    public Vector3 arrowDir;

    // Update is called once per frame
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
        if (other.tag == "Player" || other.tag == "Arrow")
        {
            Destroy(gameObject);
        }
    }
}
