using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;

namespace Tests
{
    public class StaminaTests
    {
        GameObject canvas;
        
        [SetUp]
        public void PrepareScene()
        {
            canvas = MonoBehaviour.Instantiate(Resources.Load<GameObject>("UniversalAssets/Canvas"));
        }

        [TearDown]
        public void CleanUp()
        {
            MonoBehaviour.Destroy(canvas);
            canvas = null;
        }

        [UnityTest]
        public IEnumerator StaminaLoss_test()
        {
            //Set up
            GameObject mockObject = new GameObject();
            mockObject.AddComponent<GameManager>();

            yield return 0;

            //Call Unit
            int curStam = mockObject.GetComponent<GameManager>().GetStamina();
            mockObject.GetComponent<GameManager>().DrainStamina(10);

            //Test values
            Assert.AreEqual(curStam-10, mockObject.GetComponent<GameManager>().GetStamina());

            //Clean up
            MonoBehaviour.Destroy(mockObject);

            yield break;
        }

        [UnityTest]
        public IEnumerator StaminaRegain_test()
        {
            //Set up
            GameObject mockObject = new GameObject();
            mockObject.AddComponent<GameManager>();

            yield return 0;

            //Call Unit
            mockObject.GetComponent<GameManager>().DrainStamina(20);
            int curStam = mockObject.GetComponent<GameManager>().GetStamina();
            mockObject.GetComponent<GameManager>().RegenStamina(10);

            //Test values
            Assert.AreEqual(curStam+10, mockObject.GetComponent<GameManager>().GetStamina());

            //Clean up
            MonoBehaviour.Destroy(mockObject);

            yield break;
        }

        [UnityTest]
        public IEnumerator StaminaMaxDrop_test()
        {
            //Set up
            GameObject mockObject = new GameObject();
            mockObject.AddComponent<GameManager>();

            yield return 0;

            //Call Unit
            int curStam = mockObject.GetComponent<GameManager>().GetStamina();
            yield return new WaitForSeconds(10);
            int newStam = mockObject.GetComponent<GameManager>().GetStamina();

            //Test values
            Debug.Log("Beginning Stamina=" + curStam);
            Debug.Log("Stamina after 10 seconds=" + newStam);
            Assert.That(curStam, Is.GreaterThan(newStam));

            //Clean up
            MonoBehaviour.Destroy(mockObject);

            yield break;
        }

        [UnityTest]
        public IEnumerator StaminaBarMovement_test()
        {
            //Set up
            GameObject mockObject = new GameObject();
            mockObject.AddComponent<GameManager>();

            yield return 0;

            GameObject sliderBarObject = GameObject.FindWithTag("StaminaBar");
            RectTransform staminaBarObject = (RectTransform) sliderBarObject.GetComponent<RectTransform>().parent;

            //Call Unit
            int CAP_DECAY_TIME = 10;
            int curStam = mockObject.GetComponent<GameManager>().GetStamina();
            float curBarLevel = staminaBarObject.sizeDelta.x;
            mockObject.GetComponent<GameManager>().DrainCap(20);
            yield return new WaitForSeconds(CAP_DECAY_TIME);
            int newStam = mockObject.GetComponent<GameManager>().GetStamina();
            float newBarLevel = staminaBarObject.sizeDelta.x;

            //Test values
            Debug.Log("Beginning Stamina=" + curStam);
            Debug.Log("Stamina after=" + newStam);
            Debug.Log("Beginning Stamina Bar X=" + curBarLevel);
            Debug.Log("Stamina Bar X afterwards=" + newBarLevel);
            Assert.That(curStam-20, Is.GreaterThan(newStam));
            Assert.That(newStam, Is.GreaterThan(curStam-20-CAP_DECAY_TIME));
            Assert.That(curBarLevel-1, Is.GreaterThan(newBarLevel));

            //Clean up
            MonoBehaviour.Destroy(mockObject);

            yield break;
        }
    }
}
