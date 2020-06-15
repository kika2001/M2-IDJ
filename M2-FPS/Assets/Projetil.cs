using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float speed;
    public bool hasBulletHole;
    public GameObject bullethole;
    public float time = 4;
    public float damage;
    public float velo;
    public float reducer_damage;
    public Vector3 lastpos = new Vector3(0,0,0);
    private float distance;
    void Start()
    {
        StartCoroutine(Delete(10));
    }
    IEnumerator Delete(float tempo)
    {
        yield return new WaitForSeconds(tempo);
        Destroy(this.gameObject);
    }
    private void LateUpdate()
    {
        distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - lastpos.x, 2) + Mathf.Pow(transform.position.y - lastpos.y, 2) + Mathf.Pow(transform.position.z - lastpos.z, 2));
        velo = distance / Time.deltaTime;
        Debug.Log("Current velocity: " + GetComponent<Rigidbody>().velocity.magnitude);
        lastpos = transform.position;
    }
    private void Update()
    {
        if (damage<0)
        {
            damage = 0;
        }
        if (damage>0)
        {
            damage -= reducer_damage * Time.deltaTime;
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player" && hasBulletHole)
        {
            var normal = collision.GetContact(0).normal;
            var bullethole_go = Instantiate(bullethole, collision.GetContact(0).point, Quaternion.identity);
            bullethole_go.transform.forward = normal;
            Destroy(this.gameObject);
        }
    }
}
