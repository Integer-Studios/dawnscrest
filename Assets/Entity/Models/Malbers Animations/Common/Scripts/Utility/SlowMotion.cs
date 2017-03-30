using UnityEngine;
using System.Collections;

namespace MalbersAnimations
{

    /// <summary>
    /// Going slow motion on user input
    /// </summary>
    public class SlowMotion : MonoBehaviour {
       
        [Header("Activate SlowMo with Right Click Mouse")]
        [Range(0.05f, 1)]
        [SerializeField] float slowMoTimeScale = 0.3f;
        [Range (0.1f,10)]
        [SerializeField] float slowMoSpeed =2f;
        bool canchange;


        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (Time.timeScale == 1.0F)
                {
                    StartCoroutine(SlowTime());
                }
                else
                {
                    StartCoroutine(RestartTime());
                }


                Time.fixedDeltaTime = 0.02F * Time.timeScale;
            }
        }

        IEnumerator SlowTime()
        {
            while (Time.timeScale > slowMoTimeScale)
            {
                Time.timeScale -= 1/slowMoSpeed * Time.unscaledDeltaTime;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
                yield return null;
            }
            Time.timeScale = slowMoTimeScale;
        }

        IEnumerator RestartTime()
        {
            while (Time.timeScale <1)
            {
                Time.timeScale += 1 / slowMoSpeed * Time.unscaledDeltaTime;
                yield return null;
            }
            Time.timeScale = 1;
        }
       
    }
}
