using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;
    public float maximumX;
    public float runningSpeed;
    public float xSpeed;
    private float _currentRunningSpeed;
    public GameObject runningOnBodyPartPrefab;
    public List<RunningOnBodyPart> bodyParts;
    private string bodyPartObstacleTag = "AddBodyPart";
    private string zombieChildName = "Zombie3";
    private string attackAnimationName = "Z_Attack";

    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        Current = this;
        _currentRunningSpeed=runningSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = new Vector3(GetXMovement(), transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
        transform.position = newPosition;
    }

    private float GetXMovement()
    {
        float newX = 0;
        float touchXDelta = 0;

        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            touchXDelta = Input.GetTouch(0).deltaPosition.x / Screen.width;

        }else if(Input.GetMouseButton(0))
        {
            touchXDelta = Input.GetAxis("Mouse X");
        }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -maximumX, maximumX);

        return newX;
    }

    private void OnTriggerEnter(Collider other)
    {
        animator = transform.Find(zombieChildName).GetComponent<Animator>();

        if(other.tag == bodyPartObstacleTag)
        {
            animator.Play(attackAnimationName,0, 1.0f);
            IncrementBodyPartVolume(0.2f);
            Destroy(other.gameObject);
        }
    }

    public void IncrementBodyPartVolume(float incrementValue)
    {
        if(bodyParts.Count == 0)
        {
            if(incrementValue > 0)
            {
                CreateBodyPart(incrementValue);
            }
            else
            {
                //GameOver
            }
        }else
        {
            bodyParts[bodyParts.Count -1].IncrementBodyPartSize(incrementValue);
        }
    }

    public void CreateBodyPart(float value)
    {
        RunningOnBodyPart createdRunningOnBodyPart = Instantiate(runningOnBodyPartPrefab, transform).GetComponent<RunningOnBodyPart>();
        bodyParts.Add(createdRunningOnBodyPart);
        createdRunningOnBodyPart.IncrementBodyPartSize(value);
    }

    public void DestroyBodyPart(RunningOnBodyPart bodyPart)
    {   
        bodyParts.Remove(bodyPart);
        Destroy(bodyPart.gameObject);
    }
}
