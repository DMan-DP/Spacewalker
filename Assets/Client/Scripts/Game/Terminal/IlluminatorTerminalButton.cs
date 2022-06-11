using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Client.Gameplay
{
    public class IlluminatorTerminalButton : TerminalHandButtonBase
    {
        public Animator IlluminatorAnimator;
        public AudioSource IlluminatorAudioSource;
        public AudioSource TerminalAudioSource;
        [Space]
        public AudioClip InteractableClip;
        public AudioClip NonInteractableClip;
        [Space]
        public Material InteractableMaterial;
        [Space]
        public Material NonInteractableMaterial;

        public UnityEvent OnIlluminatorOpening;

        private bool isLocked = true;
        private static readonly int open = Animator.StringToHash("Open");

        private void Start()
        {
            SetLocked(true);
        }

        protected override void PressButton()
        {
            base.PressButton();
            if (isLocked)
            {
                TerminalAudioSource.clip = NonInteractableClip;
                TerminalAudioSource.Play();
            }
            else
            {
                TerminalAudioSource.clip = InteractableClip;
                TerminalAudioSource.Play();
                IlluminatorAnimator.SetTrigger(open);
                IlluminatorAudioSource.Play();
                SetLocked(true);
                OnIlluminatorOpening.Invoke();
            }
        }

        public void SetLocked(bool value)
        {
            isLocked = value;
            var text = GetComponentInChildren<TextMeshProUGUI>();
            
            if (isLocked)
            {
                GetComponent<Image>().material = NonInteractableMaterial;
                
                text.text = "Недоступно";
                text.fontSize = 2;
            }
            else
            {
                GetComponent<Image>().material = InteractableMaterial;
                
                text.text = "Открыть";
                text.fontSize = 2.8f;
            }
        }
    }
}