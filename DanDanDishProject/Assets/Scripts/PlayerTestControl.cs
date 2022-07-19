using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestControl : MonoBehaviour
{
    public GameObject ps, blood;
    public float fieldOfImpact, force;
    public LayerMask layerToHit;
    void Start()
    {
        
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            Die();
        }
    }
    public void Die()
    {
        Vector2 pos = new Vector2(transform.position.x, (transform.position.y - (Random.Range(0f, 1f))));
        Instantiate(blood, pos, Quaternion.identity);
        Instantiate(ps, transform.position, ps.transform.rotation);
        Collider2D[] objects = Physics2D.OverlapCircleAll(pos, fieldOfImpact, layerToHit);
        foreach(Collider2D obj in objects)
        {
            Vector2 direction = (Vector2)obj.transform.position - pos;
            obj.GetComponent<Rigidbody2D>().AddForce(direction * force * 100f);

        }
        ps.transform.SetParent(null);
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fieldOfImpact);
    }
}
