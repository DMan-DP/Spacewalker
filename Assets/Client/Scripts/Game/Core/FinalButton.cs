using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Client
{
    public class FinalButton : MonoBehaviour
    {
        public SpriteRenderer[] ButtonSprite;
        [Space]
        public Material ButtonPlaneMaterial;
        public Material ButtonRingMaterial;
        [Space]
        public TextMeshPro TextMeshPro;
        public Material TextMaterial;
        [Space]
        public AudioSource AudioSource;
        public AudioClip InteracableClip;
        public AudioClip NonInteracableClip;
        [Space] 
        public AudioSource DoorAudioSource;
        
        private bool isLocked;
        
        public void OpenDoor()
        {
            isLocked = true;
            
            ButtonSprite[0].material = ButtonPlaneMaterial;
            for (int i = 1; i < ButtonSprite.Length; i++)
            {
                ButtonSprite[i].material = ButtonRingMaterial;
            }

            TextMeshPro.renderer.material = TextMaterial;
            TextMeshPro.text = "ОТКРЫТЬ";
        }

        public void Click()
        {
            if (isLocked)
            {
                AudioSource.PlayOneShot(NonInteracableClip);
            }
            else
            {
                AudioSource.PlayOneShot(InteracableClip);
                DoorAudioSource.Play();
                SceneLoader.SwitchToScene("EndTitle", true);
            }
        }
    }
}