using UnityEngine;

public class Parallaxing : MonoBehaviour
{
    public Transform[] backgrounds;  //all the back and foregrounds we want to be parallaxed
    private float[] parallaxScale;

    public float smoothing = 1f;

    private Transform cam;
    private Vector3 previousCamPos; //stores the position of the camera in the previous frame

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    private void Start()
    {
        previousCamPos = cam.position;

        parallaxScale = new float[backgrounds.Length];

        for(int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScale[i] = backgrounds[i].position.z * -1;
        }
    }
    private void Update()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float parallaxX = (previousCamPos.x - cam.position.x) * parallaxScale[i];
            float parallaxY = (previousCamPos.y - cam.position.y) * parallaxScale[i];

            float backgroundTargetPosX = backgrounds[i].position.x + parallaxX;
            float backgroundTargetPosY = backgrounds[i].position.x + parallaxY;

            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime); // lerp moves between 2 points
        }
        previousCamPos = cam.position;
    }
}

