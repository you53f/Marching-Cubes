using UnityEngine;

public class SpringImplementationFloat : MonoBehaviour
{
    SpringUtils.tDampedSpringMotionParams springParams;
    [SerializeField] GameObject target;

    public float frequency = 15f;
    public float dampingRatio = 0.5f;

    private float targetPos;
    private float currentPos;
    float vel;

    private void Awake()
    {
        springParams = new SpringUtils.tDampedSpringMotionParams();
    }
    private void Update()
    {
        targetPos = target.transform.position.y;
        SpringUtils.CalcDampedSpringMotionParams(ref springParams, Time.deltaTime, frequency, dampingRatio);
        SpringUtils.UpdateDampedSpringMotion(ref currentPos, ref vel, targetPos, in springParams);
        transform.position = new Vector3(currentPos, transform.position.y, transform.position.z);
    }
}