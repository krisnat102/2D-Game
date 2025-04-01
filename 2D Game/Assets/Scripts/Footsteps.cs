using UnityEngine;

namespace Krisnat
{
    public class Footsteps : MonoBehaviour
    {
        [Header("Footstep audios")]
        [SerializeField] private AudioSource footstep;
        [SerializeField] private AudioClip groundFS, grassFS, rockFS;

        [Header("Layers")]
        [SerializeField] private LayerMask groundLayer, grassLayer, rockLayer;

        public void PlayFootstep()
        {
            AudioClip clip = null;

            FSMaterial material = SurfaceSelect();

            switch (material)
            {
                case FSMaterial.Ground:
                    clip = groundFS;
                    break;
                case FSMaterial.Rock:
                    clip = rockFS;
                    break;
                case FSMaterial.Grass:
                    clip = grassFS;
                    break;
                default:
                    break;
            }

            if (clip != null)
            {
                footstep.clip = clip;
                footstep.volume = Random.Range(0.07f, 0.14f);
                footstep.pitch = Random.Range(0.8f, 1.2f);
                footstep.Play();
            }
        }

        public void PlayCrouchFootstep()
        {
            AudioClip clip = null;

            FSMaterial material = SurfaceSelect();

            switch (material)
            {
                case FSMaterial.Ground:
                    clip = groundFS;
                    break;
                case FSMaterial.Rock:
                    clip = rockFS;
                    break;
                case FSMaterial.Grass:
                    clip = grassFS;
                    break;
                default:
                    break;
            }

            if (clip != null)
            {
                footstep.clip = clip;
                footstep.volume = Random.Range(0.04f, 0.08f);
                footstep.pitch = Random.Range(0.7f, 1f);
                footstep.Play();
            }
        }

        public void PlayWallClimb()
        {
            footstep.clip = groundFS;
            footstep.volume = Random.Range(0.08f, 0.16f);
            footstep.pitch = Random.Range(0.5f, 0.8f);
            footstep.Play();
            
        }


        private FSMaterial SurfaceSelect()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer | rockLayer | grassLayer);
            Debug.DrawRay(transform.position, Vector2.down * 1f, Color.red, 0.5f); 

            if (hit.collider != null)
            {
                int hitLayer = hit.collider.gameObject.layer;

                if (((1 << hitLayer) & groundLayer) != 0)
                {
                    return FSMaterial.Ground;
                }
                else if (((1 << hitLayer) & grassLayer) != 0)
                {
                    return FSMaterial.Grass;
                }
                else if (((1 << hitLayer) & rockLayer) != 0)
                {
                    return FSMaterial.Rock;
                }
            }

            return FSMaterial.None;
        }
    }

    enum FSMaterial
    {
        Ground,
        Grass,
        Rock,
        None
    }
}
