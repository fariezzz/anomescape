using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject flashlight;
    public bool isFlashlightActive;
    public AudioSource flashlightSFX;
    public AudioClip flashlightSFXClip;
    public CapsuleMovement capsuleMovement;

    // Start is called before the first frame update
    void Start()
    {
        flashlight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
            //capsuleMovement.MakeNoise();
        }
    }

    public void ToggleFlashlight()
    {
        isFlashlightActive = !isFlashlightActive;
        flashlight.SetActive(isFlashlightActive);

        if (flashlightSFX != null && flashlightSFXClip != null)
        {
            flashlightSFX.PlayOneShot(flashlightSFXClip);
        }
    }
}
