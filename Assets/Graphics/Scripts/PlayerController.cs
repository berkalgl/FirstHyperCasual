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
    public List<GameObject> addedBodyParts;

    private class Constants
    {
        public const string bodyPartObstacleTag = "AddBodyPart";
        public const string syringeTag = "Syringe";
        public const string zombieChildName = "Zombie3";
        public const string attackAnimationName = "Z_Attack";
        
    } 

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
        animator = transform.Find(Constants.zombieChildName).GetComponent<Animator>();

        if (other.tag == Constants.bodyPartObstacleTag)
        {
            animator.Play(Constants.attackAnimationName, 0, 0.0f);
            ChangeTheAmountOfBodyParts(true, other.gameObject);
            Destroy(other.gameObject);
        } else if (other.tag == Constants.syringeTag) 
        {
            ChangeTheAmountOfBodyParts(false, addedBodyParts[addedBodyParts.Count - 1].gameObject);
            Destroy(other.gameObject);
        }

    }

    public void ChangeTheAmountOfBodyParts(bool addOrRemove, GameObject bodyPart)
    {
        if(addedBodyParts.Count < 0)
        {
                //GameOver
        }else
        {
            if(addOrRemove)
            {
                CreateBodyPart(bodyPart);
            }
            else
            {
                DestroyBodyPart(bodyPart);
            }
        }
    }

    public void CreateBodyPart(GameObject bodyPart)
    {
        var addedBodyPart = Instantiate(
            bodyPart, 
            new Vector3(
                transform.position.x, 
                transform.position.y, 
                addedBodyParts.Count == 0 ? (transform.position.z- 0.5f) : addedBodyParts[addedBodyParts.Count-1].transform.position.z-0.5f
                ),
            new Quaternion(0,0,0,0),
            transform);

        addedBodyParts.Add(addedBodyPart);
    }

    public void DestroyBodyPart(GameObject bodyPart)
    {   
        addedBodyParts.Remove(bodyPart);
        Destroy(bodyPart);
    }
}
