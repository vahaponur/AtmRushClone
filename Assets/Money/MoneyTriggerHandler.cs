using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyTriggerHandler : MonoBehaviour
{
    #region Serialized Fields
    #endregion

    #region Private Fields

    private StackHandler _stackHandler;
    #endregion

    #region Public Properties
    #endregion

    #region MonoBehaveMethods

    private void Start()
    {
        _stackHandler = FindObjectOfType<StackHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
      HandleTrap(other);
    }

    #endregion
    
    #region PublicMethods
    #endregion
    
    #region PrivateMethods
    /// <summary>
    /// Handles the trap trigger events
    /// </summary>
    /// <param name="other">Trap or other</param>
    void HandleTrap(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            Money money = GetComponent<Money>();
            
            _stackHandler.TrapPoppingMoney(money);
           
        } 
    }
    #endregion
}
