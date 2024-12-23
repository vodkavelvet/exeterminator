using Lean.Pool; 
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public bool isAmmo;
    public bool isHealth;
    public GameObject audioPrefab; 
    public GameObject pickupFX;
    public AudioClip pickupClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isAmmo) 
                {
                    PlayerShoot playershoot = other.GetComponent<PlayerShoot>();
                    playershoot.weapon.currentMag += Random.Range(4, 5);
                }
            else if (isHealth)
            {
                HealthManager playerHealth = other.GetComponent<HealthManager>();
                playerHealth.CURRENTHEALTH += Random.Range(30, 100);
            }
            LeanPool.Spawn(pickupFX, transform.position, Quaternion.identity);
            GameObject audioCLone = LeanPool.Spawn(audioPrefab, transform.position, Quaternion.identity);
            AudioSource audioSourceClone = audioCLone.GetComponent<AudioSource>();
            audioSourceClone.clip = pickupClip;
            audioSourceClone.Play();
            Destroy(gameObject);
        }
    }
}
