using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GeneralEvents : MonoBehaviour
{
    public GameObject audioPrefab;
    public GameObject footStepFX;
    public Transform footR;
    public Transform footL;
    public AudioClip footStepClip;

   public void FootR()
    {
        LeanPool.Spawn(footStepFX, footR.position, Quaternion.identity);
        GameObject audioCLone = LeanPool.Spawn(audioPrefab, footR.position, Quaternion.identity);
        AudioSource audioSourceClone = audioCLone.GetComponent<AudioSource>();
        audioSourceClone.clip = footStepClip;
        audioSourceClone.Play();    
    }
   
    public void FootL()
    {
        LeanPool.Spawn(footStepFX, footL.position, Quaternion.identity);
        GameObject audioCLone = LeanPool.Spawn(audioPrefab, footL.position, Quaternion.identity);
        AudioSource audioSourceClone = audioCLone.GetComponent<AudioSource>();
        audioSourceClone.clip = footStepClip;
        audioSourceClone.Play();
    }
}
