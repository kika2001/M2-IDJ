using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    public float time = 4;
    void Start()
    {
        StartCoroutine(Delete(4));
    }
    IEnumerator Delete(float tempo)
    {
        yield return new WaitForSeconds(tempo);
        Destroy(this.gameObject);
    }

}
