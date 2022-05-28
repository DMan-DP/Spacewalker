using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Client
{
    public class TextWriter : MonoBehaviour
    {
        [Min(0.001f)] public float timePerCharacter;
        private string textToWrite = "";
        private TextMeshProUGUI textMesh;
        private float timer = 0f;
        private int characterIndex;

        #region MonoBehaviour Methods
        private void Start()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
            textToWrite = textMesh.text;
            characterIndex = 0;
            StartCoroutine(WriteText());
        }
    
        private IEnumerator WriteText()
        {
            if (textMesh != null && textToWrite != "")
            {
                while (characterIndex < textToWrite.Length)
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0f)
                    {
                        timer += timePerCharacter;
                        characterIndex++;
                        textMesh.text = textToWrite.Substring(0, characterIndex);
                    }
                    
                    yield return null;
                }
            }
        }
        #endregion
    }
}