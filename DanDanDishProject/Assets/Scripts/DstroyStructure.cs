using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DstroyStructure : MonoBehaviour
{
    [SerializeField] GameObject structureParts;
    [SerializeField] float force = 10;
    [SerializeField] float fieldOfImpact = 2.1f;
    public LayerMask layerToHit;

    void Start()
    {
        DestroyIt();
    }

    public void DestroyIt()
    {
        Vector2 pos = new Vector2(transform.position.x, (transform.position.y - (UnityEngine.Random.Range(0f, 1f))));
        //Instantiate(blood, pos, Quaternion.identity);
        //Instantiate(structureParts, transform.position, structureParts.transform.rotation);//
        Collider2D[] objects = Physics2D.OverlapCircleAll(pos, fieldOfImpact, layerToHit);
        foreach (Collider2D obj in objects)
        {
            Vector2 direction = (Vector2)obj.transform.position - pos;
            obj.GetComponent<Rigidbody2D>().AddForce(direction * force * 100f);
        }
        structureParts.transform.SetParent(null);
    }
}
