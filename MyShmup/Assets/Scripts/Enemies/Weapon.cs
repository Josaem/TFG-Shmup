using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private float _delayUntilShooting = 0;

    private Transform _weaponPivot;
    private Transform _originalTransform;
    private bool _isEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        _originalTransform = transform;
        _weaponPivot = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableWeapon() // -> enables its guns too
    {
        Invoke(nameof(StartWeapon), _delayUntilShooting);
    }

    private void StartWeapon()
    {
        _isEnabled = true;
    }

    public void DisableWeapon() // -> disables it and its guns
    {
        _isEnabled = false;
    }

    /*
    -gun[]
    -delayUntilShooting
    -weaponBehavior[]
        -enabled
        -rotative
        -pointatplayer
        -duration
        -offset
     */
}
