using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class WaveGun : MonoBehaviour
{
    [SerializeField]
    private float _timeBetweenWaves;
    [SerializeField]
    private float _waveSpeed;
    [SerializeField]
    private WaveSelector _waveToDie;
    [SerializeField]
    private GameObject _waveObject;

    private float _waveTime = 0;
    private GameManager _myGM;

    [System.Serializable]
    private class WaveSelector
    {
        public int _section;
        public int _wave;
    }

    private void Start()
    {
        _myGM = GetComponentInParent<GameManager>();
    }

    private void Update()
    {
        //If cooldown between shots has passed
        if (Time.time > _waveTime)
        {
            _waveTime = Time.time + _timeBetweenWaves;

            //Shoot a shot
            GameObject wave = Instantiate(_waveObject,
                transform.position, Quaternion.identity,
                transform);
            wave.GetComponent<ExpandingWaveBehavior>()._speed = _waveSpeed;
        }

        if (_myGM != null && (_myGM._sectionIndex == _waveToDie._section && _myGM._waveIndex == _waveToDie._wave))
        {
            Destroy(gameObject);
        }
    }
}
