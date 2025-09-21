using UnityEngine;

namespace Krisnat
{
    public class FloatUpAndDown : MonoBehaviour
    {
        [SerializeField] private float amp = 1f;
        [SerializeField] private float frequency = 1f;
        private Vector2 initPos;
        
        private void Start()
        {
            initPos = transform.position;
        }

        private void Update()
        {
            transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time * frequency) * amp + initPos.y, 0);
        }
    }
}
