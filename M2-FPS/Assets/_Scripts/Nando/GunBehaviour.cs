using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

public class GunBehaviour : MonoBehaviour
{

    //----------------------------------------
    public GameObject cam_go;
    private Camera cam;
    //----------------------------------------


    //----------------------------------------
    public GameObject bullet;
    public GameObject shotpoint;
    public GameObject player_With_Rigidbody;
    public bool WantsBump;
    public float BumpForce;
    public LayerMask layers;
    public string FireButton;
    public string RealoadButton;

    //----------------------------------------


    //----------------------------------------
    [HideInInspector]
    public bool disparar = false;
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
    public float dano;
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

    public float current_uprecoil = 0;
    public float current_rightrecoil = 0;
    public float max_uprecoil;
    public float max_rightrecoil;
    public float increment_uprecoil;
    public float increment_rightrecoil;
    public float decrement_recoil;

    //-----------Teste/trackingShit----------------------
    public Text ammoui_text;
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------


    private void Start()
    {
        cam = cam_go.GetComponent<Camera>();
        if (currentMagazineBullets > maxMagazineSize)
        {
            currentMagazineBullets = maxMagazineSize;
        }
    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Update()
    {
        if (Input.GetButton(FireButton) && podedisparar && !weapon_is_shooting && !reloading)
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
        else if (!Input.GetButton(FireButton))
        {
            disparar = false;
            if (wantsRecoil)
            {
                if (current_rightrecoil < 0)
                {
                    current_rightrecoil = 0;
                }
                else if (current_rightrecoil > 0)
                {
                    current_rightrecoil -= decrement_recoil * Time.deltaTime;
                }



                if (current_uprecoil < 0)
                {
                    current_uprecoil = 0;
                }
                else if (current_uprecoil > 0)
                {

                    current_uprecoil -= decrement_recoil * Time.deltaTime;
                }

            }

        }

        if (max_uprecoil < current_uprecoil)
            current_uprecoil = max_uprecoil;
        if (max_rightrecoil < current_rightrecoil)
            current_rightrecoil = max_rightrecoil;

        if (Input.GetKeyDown(KeyCode.R) && currentMagazineBullets < maxMagazineSize && currentSuppBullets > 0 && !disparar)
        {
            Debug.Log("Started Reloading");
            reloading = true;
            StartCoroutine(ReloadingCouldown(3f));
        }
        //-------------Teste----------------------
        ammoui_text.text = "Ammo:" + currentMagazineBullets.ToString() + "/" + maxMagazineSize + "\n Current SupBullets: " + currentSuppBullets;

    }
    private IEnumerator Burst(float time, int times = 1, int currentTime = 0, bool burst = false)
    {
        if (currentTime >= times)
        {
            if (burst)
                StartCoroutine(Waiter(burstTime));

            yield break;
        }
        Disparar(0f);

        if (currentMagazineBullets <= 0)
            yield break;
        yield return new WaitForSeconds(time);
        
        StartCoroutine(Burst(time, times, ++currentTime));
    }

    private IEnumerator Waiter(float time)
    {
        yield return new WaitForSeconds(time);
    }
    //public void Disparar(float tempo)
    //{

    //    RaycastHit hit;

    //    if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layers))
    //    {
    //        Vector3 direc = cam.transform.forward;
    //        direc = direc.normalized;
    //        Debug.Log(hit.transform.gameObject.name);
    //        var bala = Instantiate(bullet, shotpoint.transform.position, Quaternion.EulerAngles(direc));
    //        bala.GetComponent<Rigidbody>().AddForce(direc * potencia, ForceMode.Impulse);
    //        podedisparar = false;
    //        StartCoroutine(WaitToEnableFire(tempo));

    //    }
    //    else
    //    {
    //        var bala = Instantiate(bullet, shotpoint.transform.position, Quaternion.EulerAngles(cam.transform.forward));
    //        bala.GetComponent<Rigidbody>().AddForce(cam.transform.forward * potencia, ForceMode.Impulse);
    //        podedisparar = false;
    //        StartCoroutine(WaitToEnableFire(tempo));
    //    }
    //}
    public void Disparar(float tempo)
    {

        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layers))
        {
            Vector3 direc = cam.transform.forward;
            Debug.Log(hit.transform.gameObject.name);
            var bala = Instantiate(bullet, shotpoint.transform.position, Quaternion.EulerAngles(direc));
            bala.transform.LookAt(hit.point);
            bala.GetComponent<Rigidbody>().AddForce(bala.transform.forward * potencia, ForceMode.Impulse);
            bala.GetComponent<Projetil>().dano = dano;
            //Debug.DrawRay(bala.transform.position, bala.transform.forward, Color.red,10f);
            podedisparar = false;
            if (wantsRecoil)
                AddRecoil();
            if (WantsBump)
                player_With_Rigidbody.GetComponent<Rigidbody>().AddForce((player_With_Rigidbody.transform.forward) * BumpForce, ForceMode.Impulse);
            StartCoroutine(WaitToEnableFire(tempo));

        }
        else
        {
            var bala = Instantiate(bullet, shotpoint.transform.position, Quaternion.EulerAngles(cam.transform.forward));
            bala.GetComponent<Rigidbody>().AddForce(cam.transform.forward * potencia, ForceMode.Impulse);
            bala.GetComponent<Projetil>().dano = dano;
            podedisparar = false;
            if (wantsRecoil)
                AddRecoil();
            if (WantsBump)
                player_With_Rigidbody.GetComponent<Rigidbody>().AddForce((player_With_Rigidbody.transform.forward) * BumpForce, ForceMode.Impulse);
            StartCoroutine(WaitToEnableFire(tempo));
        }
    }

    public void AddRecoil()
    {
        current_uprecoil += increment_uprecoil;
        current_rightrecoil += increment_rightrecoil;
        ////----------------Delete this After---------------------------
        //increment_uprecoil = Random.Range(-0.5f, 0.5f);
        //increment_rightrecoil = Random.Range(-0.3f, 0.3f);
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
    [CustomEditor(typeof(GunBehaviour))]
    public class GunBehaviour_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            //public LayerMask layers;

            serializedObject.Update();
            GunBehaviour targetscript = (GunBehaviour)target;

            //---------------------------------------------------------------------------------------------------------------------------------------------
            EditorGUILayout.LabelField("Camera GameObject", EditorStyles.boldLabel);
            targetscript.cam_go = (GameObject)EditorGUILayout.ObjectField(targetscript.cam_go, typeof(GameObject), true);
            EditorGUILayout.HelpBox("In this field you put the camera which the player is using. This can not be empty", MessageType.Warning);
            //---------------------------------------------------------------------------------------------------------------------------------------------

            //---------------------------------------------------------------------------------------------------------------------------------------------
            EditorGUILayout.LabelField("Bullet GameObject", EditorStyles.boldLabel);
            targetscript.bullet = (GameObject)EditorGUILayout.ObjectField(targetscript.bullet, typeof(GameObject), true);
            EditorGUILayout.HelpBox("In this field you put the bullet that the weapon shoots", MessageType.Info);
            //---------------------------------------------------------------------------------------------------------------------------------------------


            //---------------------------------------------------------------------------------------------------------------------------------------------
            EditorGUILayout.LabelField("ShotPoint", EditorStyles.boldLabel);
            targetscript.shotpoint = (GameObject)EditorGUILayout.ObjectField(targetscript.shotpoint, typeof(GameObject), true);
            EditorGUILayout.HelpBox("In this field you put the spawnpoint for the bullet", MessageType.Info);
            //---------------------------------------------------------------------------------------------------------------------------------------------

            //---------------------------------------------------------------------------------------------------------------------------------------------            
            targetscript.WantsBump = EditorGUILayout.Toggle("Wants Bump", targetscript.WantsBump);

            if (targetscript.WantsBump == true)
            {
                targetscript.player_With_Rigidbody = (GameObject)EditorGUILayout.ObjectField("Player with RigidBody", targetscript.player_With_Rigidbody, typeof(GameObject), true);
                targetscript.BumpForce = EditorGUILayout.FloatField("Bump Force", targetscript.BumpForce);
            }

            //---------------------------------------------------------------------------------------------------------------------------------------------
            EditorGUILayout.LabelField("Inputs", EditorStyles.largeLabel);

            targetscript.FireButton = EditorGUILayout.TextField("Fire Button", targetscript.FireButton);
            targetscript.RealoadButton = EditorGUILayout.TextField("Reload Button", targetscript.RealoadButton);
            //------------------------------------------ERROOO---------------------------------------------------------------------------------------------------

            //EditorGUILayout.LabelField("Layers", EditorStyles.boldLabel);
            //targetscript.layers = EditorGUILayout.LayerField(targetscript.layers.value, targetscript.layers);


            //------------------------------------------ERROOO---------------------------------------------------------------------------------------------------

            EditorGUILayout.LabelField("Ammo Info", EditorStyles.largeLabel);

            targetscript.maxMagazineSize = EditorGUILayout.IntField("Max Magazine Size",targetscript.maxMagazineSize);
            targetscript.currentMagazineBullets = EditorGUILayout.IntField("Current Magazine Bullets", targetscript.currentMagazineBullets);
            targetscript.potencia = EditorGUILayout.FloatField("ShotForce", targetscript.potencia);
            targetscript.dano = EditorGUILayout.FloatField("Bullet Damage", targetscript.dano);
            targetscript.FireRate = EditorGUILayout.FloatField("Fire Rate", targetscript.FireRate);




            EditorGUILayout.LabelField("Weapon Info", EditorStyles.largeLabel);
            targetscript.WantsBurstFire = EditorGUILayout.Toggle("Wants Burst Fire", targetscript.WantsBurstFire);          
            if (targetscript.WantsBurstFire==true)
            {
                targetscript.burstTime = EditorGUILayout.FloatField("Time between bullets", targetscript.burstTime);
                targetscript.AmmountBurst = EditorGUILayout.IntField("Number of bullets", targetscript.AmmountBurst);
            }

            targetscript.WantsMaxSuppBullets = EditorGUILayout.Toggle("Wants Supp Bullets", targetscript.WantsMaxSuppBullets);


            if (targetscript.WantsMaxSuppBullets == true)
            {
                targetscript.maxSuppBullets = EditorGUILayout.IntField("Ammo Max Supply", targetscript.maxSuppBullets);
                targetscript.currentSuppBullets = EditorGUILayout.IntField("Ammo Current Supply", targetscript.currentSuppBullets);
            }
            targetscript.wantSpecificAmmo = EditorGUILayout.Toggle("Specific Ammo", targetscript.wantSpecificAmmo);
            if (targetscript.wantSpecificAmmo==true)
            {
                targetscript.ammo = (AmmoType)EditorGUILayout.ObjectField("Ammo Type", targetscript.ammo, typeof(AmmoType), true);
            }
            //---------------------------------------------------------------------------------------------------------------------------------------------

            //---------------------------------------------------------------------------------------------------------------------------------------------
            
            targetscript.wantsRecoil = EditorGUILayout.Toggle("Wants Recoil", targetscript.wantsRecoil);


            if (targetscript.wantsRecoil == true)
            {
                targetscript.max_uprecoil = EditorGUILayout.FloatField("Max Up Recoil", targetscript.max_uprecoil);
                targetscript.max_rightrecoil = EditorGUILayout.FloatField("Max Right Recoil", targetscript.max_rightrecoil);
                targetscript.increment_uprecoil = EditorGUILayout.FloatField("Increment Up Recoil", targetscript.increment_uprecoil);
                targetscript.increment_rightrecoil = EditorGUILayout.FloatField("Increment Right Recoil", targetscript.increment_rightrecoil);
                targetscript.decrement_recoil = EditorGUILayout.FloatField("Decrement All Recoil", targetscript.decrement_recoil);
            }
            //---------------------------------------------------------------------------------------------------------------------------------------------


            if (GUILayout.Button("Save"))
            {
                Debug.Log("Save Button Pressed");
                serializedObject.ApplyModifiedProperties();
            }
            //DrawDefaultInspector();
        }
    }

    //Primeiro antes de spawnar, tem de saber se ja existiram o maximo de balas antes (ou seja, checka a lista)
    //Caso ja exista, em vez de instaciar, ativa outravez
}

