using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class GunBehaviour : MonoBehaviour
{
    public Camera cam;
    public GameObject bullet;
    public GameObject shotpoint;
    public LayerMask layers;
    private bool disparar = false;
    private bool podedisparar = true;
    public float uprecoil;
    public float sideRecoil;
    public float speedRecoil;
    private float current_upRecoil;
    private float current_sideRecoil;
    public float Current_upRecoil 
    {
        get { return current_upRecoil; }
        set 
        {
            if (value<0)
            {
                current_upRecoil = 0;
            }
        }
    }
    public float Current_sideRecoil
    {
        get { return current_sideRecoil; }
        set
        {
            if (value < 0)
            {
                current_sideRecoil = 0;
            }
        }
    }
    private void Update()
    {
        if (Input.GetMouseButton(0) && podedisparar)
        {
            Disparar(0.1f);
        }
        
    }

    public void Recoil()
    {
        Current_upRecoil = uprecoil;
        Current_sideRecoil = sideRecoil;
        Current_upRecoil -= speedRecoil * Time.deltaTime;
        Current_sideRecoil -= speedRecoil * Time.deltaTime;
    }
    public void Disparar(float tempo)
    {

        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layers))
        {

            Vector3 direc = hit.point - shotpoint.transform.position;
            direc = direc.normalized;
            Debug.Log("Did Hit");
            Debug.Log(hit.transform.gameObject.name);
            var bala = Instantiate(bullet, shotpoint.transform.position, Quaternion.EulerAngles(direc));
            bala.GetComponent<Rigidbody>().AddForce(direc * 20, ForceMode.Impulse);
            podedisparar = false;
            StartCoroutine(WaitToEnableFire(tempo));

        }
        else
        {
            Debug.Log("Else");
            var bala = Instantiate(bullet, shotpoint.transform.position, Quaternion.EulerAngles(cam.transform.forward));
            bala.GetComponent<Rigidbody>().AddForce(cam.transform.forward * 20, ForceMode.Impulse);
            podedisparar = false;
            StartCoroutine(WaitToEnableFire(tempo));
        }
    }
    IEnumerator WaitToEnableFire(float tempo)
    {
        Recoil();
        yield return new WaitForSeconds(tempo);
        podedisparar = true;
    }

    //Primeiro antes de spawnar, tem de saber se ja existiram o maximo de balas antes (ou seja, checka a lista)
    //Caso ja exista, em vez de instaciar, ativa outravez
}
