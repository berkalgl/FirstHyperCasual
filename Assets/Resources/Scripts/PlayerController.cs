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
    private bool _isSpawningBridge;
    public GameObject bridgePiecePrefab;
    private BridgeSpawner _bridgeSpawner;
    private float _spawningBridgeTimer;
    private bool _finished;
    private float _scoreTimer;
    public AudioSource audioSource;
    public AudioClip gatherAudioClip, dropAudioClip, deadAudioClip, coinAudioClip, buyItemAudioClip, equipItemAudioClip, unequipItemAudioClip;
    [SerializeField] public Animator animator;
    private float _dropSoundTimer;
    public List<GameObject> wearablePlaces;
    public GameObject bodyPartOnCharacterPrefab;
    private class Constants
    {
        public const string bodyPartObstacleTag = "AddBodyPart";
        public const string bodyPartOnCharacter = "BodyPartOnCharacter";
        public const string syringeTag = "Syringe";
        public const string zombieChildName = "Zombie3";
        public const string attackAnimationName = "Z_Attack";
        public const string spawnBridgeStarterTag = "SpawnBridgeStarter";
        public const string spawnBridgeStopperTag = "SpawnBridgeStopper";
        public const string finishTag = "Finish";
        public const string coinTag = "Coin";
        public const string unTagged = "Untagged";
        public const string blackHole = "BlackHole";
        public const string platform = "Platform";
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
                _spawningBridgeTimer = 0.14f;
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
        GameObject createdBridgePiece = Instantiate(bridgePiecePrefab, this.transform);
        createdBridgePiece.transform.SetParent(null);
        PlayBridgeSound();
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
    private void PlayBridgeSound()
    {
        _dropSoundTimer -= Time.deltaTime;
        if(_dropSoundTimer < 0)
        {
            _dropSoundTimer = 0.15f;
            audioSource.PlayOneShot(dropAudioClip, 0.05f);
        }
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
                touchXDelta = 5 * (Input.GetTouch(0).position.x - _lastTouchedX) / Screen.width;
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
            other.tag = Constants.unTagged;
            audioSource.PlayOneShot(gatherAudioClip, 0.05f);
            animator.Play(Constants.attackAnimationName, 0, 0.0f);
            ChangeTheAmountOfBodyParts(true);
            Destroy(other.gameObject);
        }

        if (other.tag == Constants.syringeTag)
        {
            //meet the requirement when the character encounters a syringe.
            other.tag = Constants.unTagged;
            audioSource.PlayOneShot(dropAudioClip, 0.05f);
            ChangeTheAmountOfBodyParts(false);
            Destroy(other.gameObject);
        }
        
        if (other.tag == Constants.spawnBridgeStarterTag)
        {
            //meet the requirement when the character ends a platform.
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        
        if (other.tag == Constants.spawnBridgeStopperTag)
        {
            //meet the requirement when the character starts a platform.
            StopSpawningBridge();

            if (_finished)
                LevelController.Current.FinishGame();

        }
        
        if (other.tag == Constants.finishTag)
        {
            //finish
            _finished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        
        if (other.tag == Constants.coinTag)
        {
            other.tag = Constants.unTagged;
            audioSource.PlayOneShot(coinAudioClip, 0.05f);
            LevelController.Current.ChangeScore(10);
            Destroy(other.gameObject);
        }
        if (other.tag == Constants.blackHole)
        {
            Die();
        }

}
    public void ChangeTheAmountOfBodyParts(bool addOrRemove, GameObject bodyPart=null)
    {

        var createdBodyPart = transform.Find(Constants.bodyPartOnCharacter);

        if (addOrRemove)
        {
            if(createdBodyPart != null)
            {
                createdBodyPart.gameObject.transform.localScale = createdBodyPart.transform.localScale + new Vector3(0.1f,0.1f,0.1f);
            }
            else
            {
                var addedBodyPart = Instantiate(
                    bodyPartOnCharacterPrefab, 
                    new Vector3(
                        transform.localPosition.x, 
                        transform.localPosition.y + 0.25f, 
                        transform.localPosition.z - 0.3f
                        ),
                    new Quaternion(0,0,0,0),
                    transform);
                
                addedBodyPart.name = Constants.bodyPartOnCharacter;
            }
        }
        else
        {
            if(createdBodyPart != null)
                createdBodyPart.transform.localScale = createdBodyPart.transform.localScale - new Vector3(0.1f,0.1f,0.1f);

            if (createdBodyPart == null || createdBodyPart.transform.localScale == new Vector3(0.3f,0.3f,0.3f))
            {
                if (createdBodyPart != null)
                    Destroy(createdBodyPart.gameObject);
                //GameOver
                if (_finished)
                    LevelController.Current.FinishGame();
                else
                    Die();

                return;
            }

        }
    }
    public void Die()
    {
        animator.SetBool("Dead", true);
        audioSource.PlayOneShot(deadAudioClip, 0.05f);
        SetGameObjectsLayers();
        Camera.main.transform.SetParent(null);
        LevelController.Current.GameOver();
        
    }
    private void SetGameObjectsLayers()
    {

        //let the character fall when he died
        //CharacterDead layer = 6, hidden components 7 // Project Settings --> Pyshics --> Layer Matrix Character Dead and Hidden x

        gameObject.layer = 6;
        foreach (Transform trans in gameObject.transform.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = 6;
        }

        var platforms = GameObject.FindGameObjectsWithTag(Constants.platform);

        if (platforms != null)
        {
            foreach (GameObject platform in platforms)
            {
                platform.layer = 7;

                foreach (Transform trans in platform.transform.GetComponentsInChildren<Transform>(true))
                {
                    trans.gameObject.layer = 7;
                }

            }
        }
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
