using Borodar.FarlandSkies.Core.Helpers;
using UnityEngine;

namespace Borodar.FarlandSkies.LowPoly
{
    [HelpURL("http://www.borodar.com/stuff/farlandskies/lowpoly/docs/QuickStart_v2.3.pdf")]
    public class SkyboxCycleManager : Singleton<SkyboxCycleManager>
    {
        public float CycleDuration = 10f;
        public float CycleProgress;
        public bool Paused;

        private SkyboxDayNightCycle _dayNightCycle;

        //---------------------------------------------------------------------
        // Messages
        //---------------------------------------------------------------------

        protected void Start()
        {
            _dayNightCycle = SkyboxDayNightCycle.Instance;
        }

        protected void Update()
        {
            if (!Paused)
            {
                CycleProgress += (Time.deltaTime / CycleDuration) * 100f;
                CycleProgress %= 100f;
            }

            if (_dayNightCycle != null)
                _dayNightCycle.TimeOfDay = CycleProgress;
        }
    }
}