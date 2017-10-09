using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nucleon : MonoBehaviour {

    void Start()
    {
        if(transform.parent.parent.gameObject.GetComponent<Atom>().freezeNucleusMovementAfter5Seconds)
        {
            StartCoroutine(freezePos());
        }
    }

    void FixedUpdate()
    {
        gameObject.GetComponent<Rigidbody>().AddForce((transform.parent.parent.transform.position-transform.position).normalized * 1000 * Time.smoothDeltaTime);
    }

    IEnumerator freezePos()
    {
        yield return new WaitForSeconds(5f);

        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }
}
