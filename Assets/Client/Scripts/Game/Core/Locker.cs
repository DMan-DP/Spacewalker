using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class Locker : MonoBehaviour
    {
        [SerializeField] private bool isLocked = false;
        [SerializeField, ColorUsage(true, true)] private Color openColor;
        [SerializeField, ColorUsage(true, true)] private Color closeColor;
        [Space]
        [SerializeField] private Rigidbody lockerRigidbody;
        [SerializeField] private GameObject lockerBody;
            
        private Material material;
        private static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");

        public void SetLock(bool value)
        {
            isLocked = value;
            
            if (isLocked)
            {
                material.SetColor(emissionColor, closeColor);
                lockerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                material.SetColor(emissionColor, openColor);
                lockerRigidbody.constraints = RigidbodyConstraints.None;
            }
        }
        
        #region MonoBehaviour Methods
        private void Start()
        {
            material = lockerBody.GetComponent<MeshRenderer>().material;
            SetLock(isLocked);
        }
        #endregion
    }
}