using UnityEngine;

public class ViewController : MonoBehaviour
{
    [SerializeField] Camera sight;
    [SerializeField] PlayerController pc;

    public static Vector3 aimVec;

    public float sensitivity = 0.5f;

    float rotationX = 0f;
    float rotationY = 0f;

    float maxY = 90f;
    float minY = -90f;

    Vector3 forwardDirection;

    private void Update()
    {
        if (!GameManager.instance.gameOver && !GameManager.instance.putEsc)
        {
            AimVec();

            rotationX -= Input.GetAxis("Mouse X") * sensitivity;
            rotationY += Input.GetAxis("Mouse Y") * sensitivity;
            rotationY = Mathf.Clamp(rotationY, minY, maxY);

            forwardDirection.x = Mathf.Cos(rotationX * Mathf.Deg2Rad) * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * rotationY));
            forwardDirection.z = Mathf.Sin(rotationX * Mathf.Deg2Rad) * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * rotationY));
            forwardDirection.y = 0;

            transform.forward = forwardDirection;

            forwardDirection.y = Mathf.Sin(Mathf.Deg2Rad * rotationY);

            sight.transform.LookAt(forwardDirection + sight.transform.position);
        }
    }

    public void AimVec()
    {
        RaycastHit hit;
        Ray ray = new Ray(sight.transform.position, sight.transform.forward);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            aimVec = hit.point;
        }
        else
        {
            aimVec = ray.GetPoint(200f);
        }
    }
}
