using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float speed;
    public bool hasBulletHole;
    public GameObject bullethole;
    public float time = 4;
    void Start()
    {
        StartCoroutine(Delete(10));
    }
    IEnumerator Delete(float tempo)
    {
        yield return new WaitForSeconds(tempo);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag!="Player" && hasBulletHole)
        {
            var normal = collision.GetContact(0).normal;
            var bullethole_go = Instantiate(bullethole, collision.GetContact(0).point, Quaternion.identity);
            bullethole_go.transform.forward = normal;
            Destroy(this.gameObject);
        }
    }
}
