using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackHandler : MonoBehaviour
{
    #region Serialized Fields

    [Tooltip("Money forward scale unit")]
    [SerializeField] private float _moneyZScale = 1f;
    #endregion

    #region Private Fields

    private Stack<Money> _moniesOnPlayer = new Stack<Money>();
    
    /// <summary>
    /// Last Z Position according to stack size
    /// </summary>
    private float _nextZ;
    
    /// <summary>
    /// Player main collider
    /// </summary>
    private Transform _playerCollider;
    
    /// <summary>
    /// Player rigidbody
    /// </summary>
    private Rigidbody _rb;
    
    /// <summary>
    /// Default money local scale to use on animation reset
    /// </summary>
    private Vector3 _defaultMoneyScale;
    
    /// <summary>
    /// A bool to sure get default money scale once
    /// </summary>
    private bool isScaleCalculated;
   
    #endregion

    #region Public Properties

    public float NextZ
    {
        get => _nextZ;
    }
    #endregion

    #region MonoBehaveMethods

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _playerCollider = transform.GetChildWithTag("PlayerCollider");
        
    }

    private void OnCollisionEnter(Collision other)
    {
        foreach (Money money in _moniesOnPlayer)
        {
            StartCoroutine(AnimateMoney(money));
        }
        
        ProcessMoneyCollision(other);
    }

    private void Update()
    {
        ControlMoneyMovement();
    }

    #endregion
    
    #region PublicMethods
    
    /// <summary>
    /// Pop money stack until this money (including this)
    /// Used on money trigger handler to handle stack decreasing
    /// </summary>
    /// <param name="money">Money to reach</param>
    public void TrapPoppingMoney(Money money)
    {
        int countToThis = 0;
        foreach (Money mny in _moniesOnPlayer)
        {
            countToThis++;
           if (money == mny)
           {
               break;
           }
        }

        for (int i = 0; i < countToThis; i++)
        {
            _moniesOnPlayer.Pop();
            MakeSmallerCollider();
        }
        
    }
    #endregion
    
    #region PrivateMethods
  
    /// <summary>
    /// Adds money info to player money stack
    /// </summary>
    /// <param name="other"></param>
    void ProcessMoneyCollision(Collision other)
    {
        var money = other.gameObject.GetComponent<Money>();
        bool isMoney = money  != null;
        if (isMoney)
        {
            MakeBiggerCollider();
            ArrangeDefaultScale(money.transform.localScale);
            
            money.GetComponent<BoxCollider>().isTrigger = true;
            _moniesOnPlayer.Push(money);
            
            money.transform.position = NewEnterancePosition();
            ControlMoneyMovement();
            
        }
        
        void ArrangeDefaultScale(Vector3 defScale)
        {
            if (isScaleCalculated)
            {
                return;
            }
            _defaultMoneyScale = defScale;
            isScaleCalculated = true;
        }
    }
    
    /// <summary>
    /// Decide new money enterance position
    /// </summary>
    /// <returns>New position for money</returns>
    Vector3 NewEnterancePosition()
    {
        //Get last money but 
        Money lastMoney = _moniesOnPlayer.Count>0 ? _moniesOnPlayer.Peek():null;
            

        return lastMoney == null? new Vector3(transform.position.x, 0.75f, transform.position.z + _nextZ):
            new Vector3(lastMoney.transform.position.x, 0.75f, transform.position.z + _nextZ);

    }
    
    /// <summary>
    /// Controls movement of the stack in the scene
    /// </summary>
    void ControlMoneyMovement()
    {
        //Decide total Z to carry stack
        _nextZ = _moniesOnPlayer.Count * _moneyZScale;
        
        //Node counter for horizontal movement
        int node = _moniesOnPlayer.Count+1;
        
        foreach (Money money in _moniesOnPlayer)
        {
            //Arrange forward position
            money.transform.position = new Vector3(money.transform.position.x, 0.75f, transform.position.z + _nextZ);
            
            //Arrange Horizontal Position
            node--;
            MoveMoneyHorizontally(money,node);
            
            //Calculate next z position on stack
            _nextZ -= _moneyZScale;
        }
    }
    
    /// <summary>
    /// Called when stack enhanced, collider enhanced
    /// </summary>
    void MakeBiggerCollider()
    {
        //Make collider move to front
        _playerCollider.localPosition = new Vector3(_playerCollider.localPosition.x, _playerCollider.localPosition.y,
            _nextZ+_moneyZScale);
        
        //Get Collider component
        BoxCollider playerCollider = _playerCollider.GetComponent<BoxCollider>();
        
        //Make collider bigger with money scale
        playerCollider.size = new Vector3(playerCollider.size.x, playerCollider.size.y, playerCollider.size.z + _moneyZScale);
        
        //Change collider center to backward to make equal player size
        playerCollider.center = new Vector3(playerCollider.center.x, playerCollider.center.y,
            playerCollider.center.z + (_moneyZScale / 2f));
    }   
    
    /// <summary>
    /// Called when stack decreased, collider gets small
    /// </summary>
    void MakeSmallerCollider()
    {
        //Make collider move to back
        _playerCollider.localPosition = new Vector3(_playerCollider.localPosition.x, _playerCollider.localPosition.y,
            _nextZ-_moneyZScale);
        
        //Get Collider component
        BoxCollider playerCollider = _playerCollider.GetComponent<BoxCollider>();
        
        //Make collider smaller with money scale
        playerCollider.size = new Vector3(playerCollider.size.x, playerCollider.size.y, playerCollider.size.z - _moneyZScale);
        
        //Change collider center to forward to make equal player size
        playerCollider.center = new Vector3(playerCollider.center.x, playerCollider.center.y,
            playerCollider.center.z - (_moneyZScale / 4f));
    }
    
    /// <summary>
    /// Animates money by lerping them according to stack position
    /// </summary>
    /// <param name="money">Money to move</param>
    /// <param name="node">Money node on stack</param>
    void MoveMoneyHorizontally(Money money,int node)
    {
        float diffX = transform.position.x - money.transform.position.x;
        
        //Arrange xMovement according to stack pos, last slow first fast
        float xMovement = (Time.deltaTime * 20 * diffX)/(node*0.1f);
        money.transform.position += new Vector3(xMovement, 0, 0);
    }
    
    /// <summary>
    /// Animate money to tween scale
    /// </summary>
    /// <param name="money">Money to scale</param>
    /// <returns>End of frame</returns>
    IEnumerator AnimateMoney(Money money)
    {
       
        var dTime = 0f;
        while (dTime<0.2f)
        {
            dTime += Time.deltaTime;
            money.transform.localScale += new Vector3(1, 0, 1) * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        dTime = 0f;
        while (dTime<0.2f)
        {
            dTime += Time.deltaTime;
            money.transform.localScale -= new Vector3(1, 0, 1) * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        money.transform.localScale = _defaultMoneyScale;
    }
    
    
    #endregion
}
