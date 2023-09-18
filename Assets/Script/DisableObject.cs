using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour
{
    // Start is called before the first frame update

    private Animator amt;
    void Start()
    {
        amt = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        StartCoroutine(WaitForAnimationEnd());
    }
    private IEnumerator WaitForAnimationEnd()
    {
        Debug.Log("Nhonn");
        yield return new WaitForSeconds(amt.GetCurrentAnimatorStateInfo(0).length);
        gameObject.SetActive(false);
    }
}
