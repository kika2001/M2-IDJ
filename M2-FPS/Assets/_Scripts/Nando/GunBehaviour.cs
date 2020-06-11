using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Reflection;

public class GunBehaviour : MonoBehaviour
{
    public GameObject cam_go;
    private Camera cam;
    //----------------------------------------
    public GameObject bullet;
    public GameObject shotpoint;
    public LayerMask layers;
    //----------------------------------------

    //----------------------------------------
    private bool disparar = false;
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
    //----------------------------------------
    public bool wantSpecificAmmo;
    public AmmoType ammo;
    //----------------------------------------

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
        if (Input.GetMouseButton(0) && podedisparar)
        {
            disparar = true;
        }
        else
        {
            disparar = false;
        }
        if (Input.GetKeyDown(KeyCode.R) && currentMagazineBullets < maxMagazineSize && currentSuppBullets > 0)
        {

            Debug.Log("Started Reloading");
            reloading = true;
        }
        //-------------Teste----------------------
        ammoui_text.text = "Ammo:" + currentMagazineBullets.ToString() + "/" + maxMagazineSize + "\n Current SupBullets: " + currentSuppBullets;

    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
    private void FixedUpdate()
    {
        if (disparar == true && currentMagazineBullets > 0)
        {
            Disparar(FireRate);
        }
        if (reloading == true)
        {
            StartCoroutine(ReloadingCouldown(3f));
        }
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
    public void Disparar(float tempo)
    {

        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layers))
        {
            Vector3 direc = cam.transform.forward;
            direc = direc.normalized;
            Debug.Log("Did Hit");
            Debug.Log(hit.transform.gameObject.name);
            var bala = Instantiate(bullet, shotpoint.transform.position, Quaternion.EulerAngles(direc));
            bala.GetComponent<Rigidbody>().AddForce(direc * potencia, ForceMode.Impulse);
            podedisparar = false;
            StartCoroutine(WaitToEnableFire(tempo));

        }
        else
        {
            Debug.Log("Else");
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
        yield return new WaitForSeconds(tempo);
        currentMagazineBullets--;
        Debug.Log("Current bullets: " + currentMagazineBullets);
        podedisparar = true;
    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator ReloadingCouldown(float tempo)
    {
        //Recoil();
        reloading = false;
        yield return new WaitForSeconds(tempo);
        var sobra = maxMagazineSize - currentMagazineBullets;
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

