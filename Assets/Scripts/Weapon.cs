using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using Lean.Transition;
using Unity.VisualScripting;

public class Weapon : MonoBehaviour
{
    [Header("RANGE STATS")]
    public bool isFullAuto;
    public float shootDelay;
    public GameObject bulletShell;
    public GameObject muzzleFlashFX;
    public GameObject impactFX;
    public Transform muzzleFlashSpawnPoint;
    public Transform bulletShellSpawnPoint;
    public float range;
    public float horizontalRecoilRadius;
    public float verticalRecoilRadius;
    public Transform cameraTransform;
    public Transform camLookAt;
    public int maxAmmo;
    public int setMag;
    public float reloadTime;
    public AudioClip reloadClip;

    [Header("GENERAL STATS")]
    public float damage;
    public GameObject audioPrefab;
    public AudioClip shotClip;
    public AudioClip impactClip;
    public string targetAttack;
    public Animator userAnimator;
    public RuntimeAnimatorController animatorController;

    [Header("MELLE STATS")]
    public Collider triggerColider;


    [Header("DEBUG OUTPUT")]
    public Vector3 defaultCamLookAtLocalPosition;
    public bool isShoot; // variable bool untuk check box yes/no
    public bool isReload;
    public float currentMag;
    public float currentAmmo;

    private void Start()
    {
        if (triggerColider != null)
        {
            triggerColider.enabled = false;
        }
        if (camLookAt != null)
        {
            defaultCamLookAtLocalPosition = camLookAt.localPosition;
        }
        currentAmmo = maxAmmo;
        currentMag = setMag;
        userAnimator.runtimeAnimatorController = animatorController;
    }

    public void Shoot()
    {
        if (currentAmmo >=1 && !isReload)
        {
            if (isFullAuto)
            {
                if (!isShoot)
                {
                    StartCoroutine(IEShootDelay());
                    IEnumerator IEShootDelay()
                    {
                        ShootRay();
                        isShoot = true;
                        yield return new WaitForSeconds(shootDelay);
                        isShoot = false;
                    }
                }
            }
            else
            {
                if (!isShoot)
                {
                    ShootRay();
                    isShoot = true;
                }
            }
        }
        else
        {
            StartCoroutine(Reload());
        }
    }

    private void Update()
    {
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * range, Color.red);
    }

    Quaternion RandomQuaternion()
    {    
        return new Quaternion(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
    }

    void ShootRay()
    {
        RaycastHit hit;

        LeanPool.Spawn(muzzleFlashFX, muzzleFlashSpawnPoint.position, muzzleFlashSpawnPoint.rotation);
        LeanPool.Spawn(bulletShell, bulletShellSpawnPoint.position, RandomQuaternion());
        GameObject audioCLone = LeanPool.Spawn(audioPrefab, muzzleFlashSpawnPoint.position, Quaternion.identity);
        AudioSource audioSourceClone = audioCLone.GetComponent<AudioSource>();
        audioSourceClone.clip = shotClip;
        audioSourceClone.Play();
        Recoil();
        currentAmmo -= 1;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, range))
        {
            LeanPool.Spawn(impactFX, hit.point, RandomQuaternion());
            audioCLone = LeanPool.Spawn(audioPrefab, hit.point, Quaternion.identity);
            audioSourceClone = audioCLone.GetComponent<AudioSource>();
            audioSourceClone.clip = impactClip;
            audioSourceClone.Play();

            if (hit.transform.tag == "Enemy")
            { 
                HealthManager hitHealthManager = hit.transform.GetComponent<HealthManager>();
                hitHealthManager.TakeDamage(damage);
                hitHealthManager.isCostumBloodSplat = true;
                hitHealthManager.costumBloodSplatPoint = hit.point;
            }
        }
    }

    public void StopShoot()
    {
        isShoot = false;
    }

    void Recoil()
    {
        float randomHorizontalRecoil = Random.Range(-horizontalRecoilRadius, horizontalRecoilRadius);
        float randomVerticalRecoil = Random.Range(-verticalRecoilRadius, verticalRecoilRadius);
        Vector3 randomRecoiLocalPosition = new Vector3(randomHorizontalRecoil, randomVerticalRecoil, 0);
        Vector3 recoilLocalPosition = camLookAt.localPosition + randomRecoiLocalPosition;

        camLookAt.localPositionTransition(recoilLocalPosition, shootDelay).JoinTransition().
            localPositionTransition(defaultCamLookAtLocalPosition, 0.01f);
    }

    public void MelleAttack()
    {
        GameObject audioCLone = LeanPool.Spawn(audioPrefab, transform.position, Quaternion.identity);
        AudioSource audioSourceClone = audioCLone.GetComponent<AudioSource>();
        audioSourceClone.clip = shotClip;
        audioSourceClone.Play();
        triggerColider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject audioCLone = LeanPool.Spawn(audioPrefab, transform.position, Quaternion.identity);
        AudioSource audioSourceClone = audioCLone.GetComponent<AudioSource>();
        audioSourceClone.clip = impactClip;
        audioSourceClone.Play();
        if (other.CompareTag(targetAttack))
        {
            HealthManager hitHealthManager = other.GetComponent<HealthManager>();
            hitHealthManager.TakeDamage(damage);
            hitHealthManager.isCostumBloodSplat = false;    
        }
        triggerColider.enabled = false;
    }

    public IEnumerator Reload()
    {
        if (currentMag >= 1 && currentAmmo < maxAmmo && !isReload)
        {
            GameObject audioCLone = LeanPool.Spawn(audioPrefab, transform.position, Quaternion.identity);
            AudioSource audioSourceClone = audioCLone.GetComponent<AudioSource>();
            audioSourceClone.clip = reloadClip;
            audioSourceClone.Play();

            userAnimator.Play("Reload", 1);
            isReload = true;
            yield return new WaitForSeconds(reloadTime);
            currentAmmo = maxAmmo;
            currentMag -= 1;
            isReload = false;
        }
    }

}
