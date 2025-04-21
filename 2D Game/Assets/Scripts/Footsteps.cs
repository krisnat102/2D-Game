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

        [Header("Audio Settings")]
        [SerializeField] private float minVolume = 0.07f;
        [SerializeField] private float maxVolume = 0.14f;
        [SerializeField] private float minPitch = 0.8f;
        [SerializeField] private float maxPitch = 1.2f;

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
                    Debug.Log(1);
                    break;
            }

            if (clip != null)
            {
                footstep.clip = clip;
                footstep.volume = Random.Range(minVolume, maxVolume);
                footstep.pitch = Random.Range(minPitch, maxPitch);
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
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.down, 1.5f, groundLayer | rockLayer | grassLayer);

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

        private void OnDrawGizmosSelected()
        {
            Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.down * 1.5f, Color.yellow);
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
