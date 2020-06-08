using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float speed;
    public bool hasBulletHole;
    //public GameObject bullethole;
    //public GameObject hole_pos;
    // Start is called before the first frame update
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
            //var direction = transform.position - collision.transform.position;
            //var bullethole_go = Instantiate(bullethole, transform.position, Quaternion.LookRotation(direction));
            //bullethole_go.transform.LookAt(direction);
            Destroy(this.gameObject);
        }
    }
}
