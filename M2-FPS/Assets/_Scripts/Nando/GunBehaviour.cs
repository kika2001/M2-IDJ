using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Reflection;

public class GunBehaviour : MonoBehaviour
{
    //----------------------------------------
    public GameObject cam_go;
    private Camera cam;
    //----------------------------------------


    //----------------------------------------
    public GameObject bullet;
    public GameObject shotpoint;
    public LayerMask layers;
    //----------------------------------------

    //----------------------------------------
    private bool disparar = false;
    private bool weapon_is_shooting = false;
    private bool podedisparar = true;
    private bool reloading = false;

    
    //----------------------------------------

    //----------------------------------------
    public bool WantsMaxSuppBullets;
    //[HideInInspector]
    public int maxSuppBullets;
    //[HideInInspector]
    public int currentSuppBullets;
    //----------------------------------------
    public int maxMagazineSize;
    public int currentMagazineBullets;
    public float potencia;
    public float FireRate;
    public bool WantsBurstFire;
    public float burstTime;
    public int AmmountBurst;

    //----------------------------------------
    public bool wantSpecificAmmo;
    public AmmoType ammo;
    //----------------------------------------

    //--------------------------------
    public bool wantsRecoil;


    //-----------Teste/trackingShit----------------------

    public Text ammoui_text;
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
    public int CurrentSuppBullets
    {
        get { return currentSuppBullets; }
        set { currentSuppBullets = value; }
    }
    public int CurrentMagazineBullets
    {
        get { return currentMagazineBullets; }
        set { currentMagazineBullets = value; }
    }

    private void Start()
    {
        cam = cam_go.GetComponent<Camera>();
        if (currentMagazineBullets>maxMagazineSize)
        {
            currentMagazineBullets = maxMagazineSize;
        }
    }
    //public float uprecoil;
    //public float sideRecoil;
    //public float speedRecoil;
    //private float current_upRecoil;
    //private float current_sideRecoil;
    //public float Current_upRecoil 
    //{
    //    get { return current_upRecoil; }
    //    set 
    //    {
    //        if (value<0)
    //        {
    //            current_upRecoil = 0;
    //        }
    //    }
    //}
    //public float Current_sideRecoil
    //{
    //    get { return current_sideRecoil; }
    //    set
    //    {
    //        if (value < 0)
    //        {
    //            current_sideRecoil = 0;
    //        }
    //    }
    //}
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Update()
    {   
        if (Input.GetMouseButton(0) && podedisparar && !weapon_is_shooting)
        {
            disparar = true;
            if (currentMagazineBullets > 0)
            {
                if (!WantsBurstFire)
                {
                    weapon_is_shooting = true;
                    Disparar(FireRate);
                    weapon_is_shooting = false;
                }
                else
                {
                    weapon_is_shooting = true;
                    //Couroutine de disparo burst
                    StartCoroutine(Burst(burstTime, AmmountBurst, burst: true));
                    //Couroutine que faz com que nao possa disparar logo de seguida
                    StartCoroutine(WaitToEnableFireCouldown(FireRate));
                    
                }

            }
        }
        else
        {
            disparar = false;
        }
        if (Input.GetKeyDown(KeyCode.R) && currentMagazineBullets < maxMagazineSize && currentSuppBullets > 0)
        {
            Debug.Log("Started Reloading");
            reloading = true;
            StartCoroutine(ReloadingCouldown(3f));
        }
        //-------------Teste----------------------
        ammoui_text.text = "Ammo:" + currentMagazineBullets.ToString() + "/" + maxMagazineSize + "\n Current SupBullets: " + currentSuppBullets;
    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
    //public void Recoil()
    //{
    //    Current_upRecoil = uprecoil;
    //    Current_sideRecoil = sideRecoil;
    //    Current_upRecoil -= speedRecoil * Time.deltaTime;
    //    Current_sideRecoil -= speedRecoil * Time.deltaTime;
    //}
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------




    private IEnumerator Burst(float time, int times = 1, int currentTime = 0, bool burst = false)
    {
        if (currentTime >= times)
        {
            if (burst)
                StartCoroutine(Waiter(burstTime));

            yield break;
        }

        Disparar(0f);
        if(currentMagazineBullets==0)
            yield break;
        yield return new WaitForSeconds(time);

        StartCoroutine(Burst(time, times, ++currentTime));
    }

    private IEnumerator Waiter(float time)
    {
        yield return new WaitForSeconds(time);
    }   
    public void Disparar(float tempo)
    {

        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layers))
        {
            Vector3 direc = cam.transform.forward;
            direc = direc.normalized;
            Debug.Log(hit.transform.gameObject.name);
            var bala = Instantiate(bullet, shotpoint.transform.position, Quaternion.EulerAngles(direc));
            bala.GetComponent<Rigidbody>().AddForce(direc * potencia, ForceMode.Impulse);
            podedisparar = false;
            StartCoroutine(WaitToEnableFire(tempo));

        }
        else
        {
            var bala = Instantiate(bullet, shotpoint.transform.position, Quaternion.EulerAngles(cam.transform.forward));
            bala.GetComponent<Rigidbody>().AddForce(cam.transform.forward * potencia, ForceMode.Impulse);
            podedisparar = false;
            StartCoroutine(WaitToEnableFire(tempo));
        }
    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator WaitToEnableFire(float tempo)
    {
        //Recoil();
        currentMagazineBullets--;
        yield return new WaitForSeconds(tempo);        
        Debug.Log("Current bullets: " + currentMagazineBullets);
        podedisparar = true;
    }
    IEnumerator WaitToEnableFireCouldown(float tempo)
    {
        //Recoil();
        yield return new WaitForSeconds(tempo);
        podedisparar = true;
        weapon_is_shooting = false;
    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator ReloadingCouldown(float tempo)
    {
        
        yield return new WaitForSeconds(tempo);
        var sobra = maxMagazineSize - currentMagazineBullets;
        if (WantsMaxSuppBullets)
        {
            if (currentSuppBullets >= sobra)
            {
                currentMagazineBullets = maxMagazineSize;
                currentSuppBullets -= sobra;
            }
            else if (currentSuppBullets < sobra)
            {
                currentMagazineBullets += currentSuppBullets;
                currentSuppBullets = 0;
            }
        }
        else
        {
            currentMagazineBullets = maxMagazineSize;
        }
        reloading = false;

        Debug.Log("Done Reloading");



    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
    //[CustomEditor(typeof(GunBehaviour))]
    //public class GunBehaviour_Editor : Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        //public LayerMask layers;
    //        //public float potencia;
    //        serializedObject.Update();
    //        GunBehaviour targetscript = (GunBehaviour)target;


    //        EditorGUILayout.LabelField("Camera GameObject", EditorStyles.boldLabel);
    //        targetscript.cam_go = (GameObject)EditorGUILayout.ObjectField(targetscript.cam_go, typeof(GameObject), true);
    //        EditorGUILayout.HelpBox("In this field you put the camera which the player is using", MessageType.Info);


    //        EditorGUILayout.LabelField("Bullet GameObject", EditorStyles.boldLabel);
    //        targetscript.bullet = (GameObject)EditorGUILayout.ObjectField(targetscript.bullet, typeof(GameObject), true);
    //        EditorGUILayout.HelpBox("In this field you put the bullet that the weapon shoots", MessageType.Info);

    //        EditorGUILayout.LabelField("ShotPoint", EditorStyles.boldLabel);
    //        targetscript.shotpoint = (GameObject)EditorGUILayout.ObjectField(targetscript.shotpoint, typeof(GameObject), true);
    //        EditorGUILayout.HelpBox("In this field you put the spawnpoint for the bullet", MessageType.Info);

    //        //EditorGUILayout.LabelField("Layers", EditorStyles.boldLabel);
    //        //targetscript.layers = EditorGUILayout.LayerField(targetscript.layers,targetscript.layers.value);
    //        //EditorGUILayout.HelpBox("In this field you put the spawnpoint for the bullet", MessageType.Info);

    //        EditorGUILayout.LabelField("Bullet Info", EditorStyles.boldLabel);
    //        targetscript.WantsMaxSuppBullets = EditorGUILayout.Toggle("WantsSuppBullets", targetscript.WantsMaxSuppBullets);


    //        if (targetscript.WantsMaxSuppBullets == true)
    //        {
    //            EditorGUILayout.LabelField("Ammo Max Supply", EditorStyles.label);
    //            targetscript.maxSuppBullets = EditorGUILayout.IntField(targetscript.maxSuppBullets);
    //            EditorGUILayout.LabelField("Ammo Current Supply", EditorStyles.label);
    //            targetscript.currentSuppBullets = EditorGUILayout.IntField(targetscript.currentSuppBullets);
    //        }


    //        if (GUILayout.Button("Gerar Random"))
    //        {
    //            Debug.Log("Botao Teste Premido");
    //        }
    //        serializedObject.ApplyModifiedProperties();
    //        //DrawDefaultInspector();
    //    }
    //}

    //Primeiro antes de spawnar, tem de saber se ja existiram o maximo de balas antes (ou seja, checka a lista)
    //Caso ja exista, em vez de instaciar, ativa outravez
}

