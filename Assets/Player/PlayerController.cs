using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    #region Serialized Fields
    [Range(0,10)]
    [SerializeField] private float _xClamp;

     [SerializeField] private float _horizontalSpeed;
     [SerializeField] private float _forwardSpeed;
    #endregion

    #region Private Fields
    #endregion

    #region Public Properties
    #endregion

    #region MonoBehaveMethods
    void Awake()
    {
	
    }

    void Start()
    {
        
    }

   
    void Update()
    {
        MovePlayer();
    }
    #endregion
    
    #region PublicMethods
    #endregion
    
    #region PrivateMethods

    void MovePlayer()
    {
        ProcessHorizontalMovement();
        ProcessForwardMovement();
    }
    void ProcessHorizontalMovement()
    {
        var startV = transform.position;
        var input = Input.GetAxis("Horizontal");
        var movementVector = Vector3.right * input * Time.deltaTime*_horizontalSpeed;
        var xChange = transform.position.x + movementVector.x;
        xChange = Mathf.Clamp(xChange, -_xClamp, _xClamp);
        transform.position = new Vector3(xChange, startV.y, startV.z);
    }

    void ProcessForwardMovement()
    {
        transform.Translate(Vector3.forward * _forwardSpeed * Time.deltaTime);
    }
    #endregion
}
