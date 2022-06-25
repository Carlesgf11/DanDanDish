using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private GameObject AndarPs;

    public void AndarParticle()
    {
        Instantiate(AndarPs, new Vector3(transform.position.x , (transform.position.y +1.4f), -10f), Quaternion.identity);
    }
}
