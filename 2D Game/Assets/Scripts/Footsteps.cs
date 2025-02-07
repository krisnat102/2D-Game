using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Krisnat
{
    public class Footsteps : MonoBehaviour
    {
        [SerializeField] private AudioSource footstep;

        public void PlayFootstep()
        {
            SurfaceSelect();
            footstep.Play();
        }

        private FSMaterial SurfaceSelect()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f);
            return FSMaterial.None;
        }
    }

    enum FSMaterial
    {
        Ground,
        Stone,
        None
    }
}
