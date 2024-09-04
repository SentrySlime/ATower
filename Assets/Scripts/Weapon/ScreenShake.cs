using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    //Rotations
    public Vector3 currentRotation;
    private Vector3 targetRotation;

    //Hipfire Recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    //Settings
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;


    void Start()
    {

    }

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void Screenshake(float screenShakeX, float screenShakeY, float screenShakeZ)
    {
        targetRotation += new Vector3(screenShakeX, Random.Range(-screenShakeY, screenShakeY), Random.Range(-screenShakeZ, screenShakeZ));
    }

    public void RecoilFire()
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    public void InitializeShake(float x, float y, float z)
    {
        recoilX = x;
        recoilY = y;
        recoilZ = z;
    }

}

