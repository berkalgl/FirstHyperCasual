using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;
    public float maximumX;
    private float _lastTouchedX;
    public float runningSpeed;
    public float xSpeed;
    private float _currentRunningSpeed;
    public List<GameObject> addedBodyParts;

    private bool _isSpawningBridge;
    public GameObject bridgePiecePrefab;

    private BridgeSpawner _bridgeSpawner;
    private float _spawningBridgeTimer;

    private bool _finished;
    private float _scoreTimer;

    [SerializeField] public Animator animator;

    private class Constants
    {
        public const string bodyPartObstacleTag = "AddBodyPart";
        public const string syringeTag = "Syringe";
        public const string zombieChildName = "Zombie3";
        public const string attackAnimationName = "Z_Attack";
        public const string spawnBridgeStarterTag = "SpawnBridgeStarter";
        public const string spawnBridgeStopperTag = "SpawnBridgeStopper";
        public const string finishTag = "Finish";
    } 

    // Start is called before the first frame update
    void Start()
    {
        Current = this;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if level started and game active
        if (LevelController.Current == null || !LevelController.Current.gameActive)
        {
            return;
        }

        Vector3 newPosition = new Vector3(GetXMovement(), transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
        transform.position = newPosition;
        
        if (_isSpawningBridge)
        {
            _spawningBridgeTimer -= Time.deltaTime;

            if (_spawningBridgeTimer < 0)
            {
                _spawningBridgeTimer = 0.15f;
                CreateBridgePieces();

                //if finished, increment the score
                if (_finished)
                {
                    _scoreTimer -= Time.deltaTime;
                    if (_scoreTimer < 0)
                    {
                        _scoreTimer = 0.01f;
                        LevelController.Current.ChangeScore(1);
                    }
                }
            }
        }
    }

    private void CreateBridgePieces()
    {
        //Create prefab piece on the bridge
        GameObject createdBridgePiece = Instantiate(bridgePiecePrefab);
        //Destroy the body part
        ChangeTheAmountOfBodyParts(false);

        //arrange the direction of piece of the bridge
        Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position;
        float distance = direction.magnitude;
        direction = direction.normalized;
        createdBridgePiece.transform.forward = direction;

        //Check where the character is between start and end reference, make boundary
        float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z;
        characterDistance = Mathf.Clamp(characterDistance, 0, distance);

        //Arrange the position of new piece of the bridge
        Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance;
        newPiecePosition.x = transform.position.x;
        createdBridgePiece.transform.position = newPiecePosition;
    }

    private float GetXMovement()
    {
        //in the x dimension, arrange the character move
        float newX = 0;
        float touchXDelta = 0;

        if(Input.touchCount > 0 )
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _lastTouchedX = Input.GetTouch(0).position.x;

            }else if(Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                touchXDelta = 5 * (_lastTouchedX - Input.GetTouch(0).position.x) / Screen.width;
                _lastTouchedX = Input.GetTouch(0).position.x;
            }

        }else if(Input.GetMouseButton(0))
        {
            touchXDelta = Input.GetAxis("Mouse X");
        }

        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -maximumX, maximumX);

        return newX;
    }
    public void ChangeSpeed(float value) 
    {
        _currentRunningSpeed = value;
    }
    private void OnTriggerEnter(Collider other)
    {
        animator = transform.Find(Constants.zombieChildName).GetComponent<Animator>();

        if (other.tag == Constants.bodyPartObstacleTag)
        {
            //meet the requirement when the character encounters a body part.
            animator.Play(Constants.attackAnimationName, 0, 0.0f);
            ChangeTheAmountOfBodyParts(true, other.gameObject);
            Destroy(other.gameObject);
        }
        else if (other.tag == Constants.syringeTag)
        {
            //meet the requirement when the character encounters a syringe.
            ChangeTheAmountOfBodyParts(false);
            Destroy(other.gameObject);
        }
        else if (other.tag == Constants.spawnBridgeStarterTag)
        {
            //meet the requirement when the character ends a platform.
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag == Constants.spawnBridgeStopperTag)
        {
            //meet the requirement when the character starts a platform.
            StopSpawningBridge();

            if (_finished)
                LevelController.Current.FinishGame();

        }
        else if (other.tag == Constants.finishTag)
        {
            //finish
            _finished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }

    }

    public void ChangeTheAmountOfBodyParts(bool addOrRemove, GameObject bodyPart=null)
    {

        if (addOrRemove)
        {
            CreateBodyPart(bodyPart);
        }
        else
        {

            if (addedBodyParts.Count == 0) 
            {
                //GameOver
                if (_finished)
                    LevelController.Current.FinishGame();
                else
                    Die();

                return;
            }

            bodyPart = addedBodyParts[addedBodyParts.Count - 1].gameObject;
            DestroyBodyPart(bodyPart);
        }
    }
    public void Die()
    {
        animator.SetBool("Dead", true);
        //let the character fall when he died
        //CharacterDead layer = 6, hidden components 7 // Project Settings --> Pyshics --> Layer Matrix Character Dead and Hidden x
        gameObject.layer = 6;

        Camera.main.transform.SetParent(null);

        LevelController.Current.GameOver();
        
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

    public void StartSpawningBridge(BridgeSpawner spawner)
    {
        _bridgeSpawner = spawner;
        _isSpawningBridge = true;
    }
    public void StopSpawningBridge()
    {
        _isSpawningBridge = false;
    }
}
