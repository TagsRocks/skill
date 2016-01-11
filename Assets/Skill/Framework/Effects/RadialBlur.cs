using UnityEngine;
using System.Collections;
namespace Skill.Framework.Effects
{
    public class RadialBlur : MonoBehaviour
    {
        public float Time = 0.5f;// Blur time    

        void OnEnable()
        {
            if (Global.Instance != null)
            {
                if (!Global.Instance.Settings.Quality.RadialBlur)
                    return;
            }
            if (RadialBlurCameraEffect.Instance != null)
            {
                RadialBlurCameraEffect.Instance.Blur(transform.position, Time);
            }
        }
    }
}
