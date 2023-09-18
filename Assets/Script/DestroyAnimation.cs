using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAnimation : MonoBehaviour
{
    private Animator amt;
    // Start is called before the first frame update
    void Start()
    {
        amt = GetComponent<Animator>();
        Destroy(gameObject, amt.GetCurrentAnimatorStateInfo(0).length);
    }

}
