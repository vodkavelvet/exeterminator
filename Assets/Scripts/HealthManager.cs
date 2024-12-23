using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;


public class HealthManager : MonoBehaviour
{
    public float setHealth;
    public GameObject bloodSplatFX;
    public GameObject audioPrefab;
    public Slider healthBarSlider;
    public Canvas canvas;
    public TMP_Text healthText;
    public AudioClip bloodSplatClip;
    public Animator animator;
    public Camera mainCamera;
    public Component[] componentToDisable;
    public bool isHavingAimLayer;
    public bool isWorldCanvas;
    public bool isHealthPart;
    public float partDamageMultiplier;
    public HealthManager mainHealthManager;
    public AudioClip[] hurtClips; 

    [Header("DEBUG OUTPUT")]
    public bool isCostumBloodSplat;
    public Vector3 costumBloodSplatPoint;
    float currentHealth;


    public float CURRENTHEALTH
    {
        get { return currentHealth; }
        set
        {
            if (currentHealth != value)
            {
                currentHealth = value;
                onCurrentHealthChanged();
            }
        }
    }
    private void Start()
    {
        currentHealth = setHealth;
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = setHealth;
            healthBarSlider.value = CURRENTHEALTH;
            healthText.text = CURRENTHEALTH.ToString() + "/" + setHealth;
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isHealthPart)
        {
            damage *= partDamageMultiplier;
            mainHealthManager.TakeDamage(damage);
        }
        else
        {
            CURRENTHEALTH -= damage;   
        }
    }

    public void onCurrentHealthChanged()
    {
        Vector3 bloodSplatPosition;
        if (isCostumBloodSplat)
        {
            bloodSplatPosition = costumBloodSplatPoint;
        }
        else
        {
            bloodSplatPosition = new Vector3(transform.position.x, 0.5f, transform.position.z);
        }
        LeanPool.Spawn(bloodSplatFX, bloodSplatPosition, Quaternion.identity);

        GameObject audioCLone = LeanPool.Spawn(audioPrefab, bloodSplatPosition, Quaternion.identity);
        AudioSource audioSourceClone = audioCLone.GetComponent<AudioSource>();
        audioSourceClone.clip = bloodSplatClip;
        audioSourceClone.Play();

        audioCLone = LeanPool.Spawn(audioPrefab, bloodSplatPosition, Quaternion.identity);
        audioSourceClone = audioCLone.GetComponent<AudioSource>();
        audioSourceClone.clip = hurtClips[Random.Range(0, hurtClips.Length)];
        audioSourceClone.Play();
        if (healthBarSlider != null)
        {
            healthBarSlider.value = CURRENTHEALTH;
            healthText.text = CURRENTHEALTH.ToString() + "/" + setHealth;
        }
        if (CURRENTHEALTH <= 0)
        {
            animator.Play("Die");
            foreach (var component in componentToDisable)
            {
                Destroy(component);
            }
            if (isHavingAimLayer)
            {
                animator.SetLayerWeight(1, 0);
            }
        }
        if (CURRENTHEALTH > setHealth)
        {
            CURRENTHEALTH = setHealth;
        }
    }


    private void Update()
    {
        if (isWorldCanvas && canvas != null)
        {
            canvas.transform.LookAt(mainCamera.transform.position); 
        }
    }
}
