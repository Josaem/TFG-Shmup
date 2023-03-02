using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Section
    {
        public Orientation _shipOrientation;
        public BGSection _backgroundSection;
        public BGObject[] _backgroundObjects;
        public Wave[] _waves;
    }

    [System.Serializable]
    public class BGSection
    {
        public float _scrollSpeed;
        public BGObject _background;
    }

    [System.Serializable]
    public class Wave
    {
        public int _requirementsIndex = GameProperties._extraLevelRequirements[0][0];
        public float _duration = 1;
        public float _delayUntilNext = 0;
        public bool[] _enemyWave; //TODO lista de enemigos con atributos para asignar
    }

    [SerializeField]
    private int _sectionIndex;
    [SerializeField]
    private int _waveIndex;
    [SerializeField]
    private Section[] _sections;

    private bool _canStartNextSection = false;
    private BGObject _currentBG = null;
    private BGObject _prevBackground = null; //check if this works
    private IEnumerator _waitForWave;

    private void Start()
    {
        if(_sectionIndex != 0)
        {
            _currentBG = _sections[_sectionIndex]._backgroundSection._background;
        }

        StartSection();
    }

    private void Update()
    {
        /*
        si prioritywave es true
            si priorityEnemiesLeft == 0
                EndWave()
                Desactiva waitfornextwave
                prioritywave = false

        si canStartNextSection
            DespawnEndWave()
            StartSection()
            canStartNextSection = false
         */
    }

    private void StartSection()
    {
        //Spawn background objects (meteorites, etc)
        if (_sections[_sectionIndex]._backgroundObjects != null)
        {
            foreach(BGObject bGObject in _sections[_sectionIndex]._backgroundObjects)
            {
                Instantiate(bGObject);
            }
        }

        //If last background is different from the one being spawned
        if(_prevBackground != _sections[_sectionIndex]._backgroundSection._background)
        {
            _currentBG = Instantiate(_sections[_sectionIndex]._backgroundSection._background);
        }     

        //Change scroll speed, TODO lerp the speed
        _currentBG._customSpeed = _sections[_sectionIndex]._backgroundSection._scrollSpeed;

        PlayerController player = FindObjectsOfType<PlayerController>()[0];
        if(player != null)
        {
            player.ChangeOrientation(_sections[_sectionIndex]._shipOrientation);
        }

        SpawnWave();
    }

    private void EndSection()
    {
        _sectionIndex++;

        Debug.Log("Ending section");
        if (_sectionIndex < _sections.Length)
        {

        }
        /*
         si sectionIndex + 1 not null 
            si background anterior es el mismo que +1
                StartSection

            else
                dice a background que esta dying
                SpawnEndWave();
        */
    }

    public void AllowStartNextSection()
    {
        _canStartNextSection = true;
    }

    private void SpawnWave()
    {
        Debug.Log("Spawning wave: " + _waveIndex);
        //spawnea enemyWave si hay 

        //Set timer to forcibly end wave
        _waitForWave = WaitForWave(_sections[_sectionIndex]._waves[_waveIndex]._duration);
        StartCoroutine(_waitForWave);

        //si hay enemyWave
        //cuenta enemigos de prioridad de la enemyWave -> priorityEnemyAmount
        //si priorityEnemyAmount es mayor que 0->es wave de enemigos de prioridad
        //        setea priorityEnemiesLeft = priorityEnemyAmount
        //prioritywave es true                  
    }

    private IEnumerator WaitForWave(float waveDuration)
    {
        yield return new WaitForSeconds(waveDuration);
        EndWave();
    }

    public void EndWave()
    {
        /*
        si enemyWave no es null
            despawnea enemyWave
        */

        //Search next wave after delay
        Invoke(nameof(GetNextWave), _sections[_sectionIndex]._waves[_waveIndex]._delayUntilNext);
    }

    public void GetNextWave()
    {
        //Look for next wave
        int? nextWave = SelectNextWave(_waveIndex + 1);

        if(nextWave == null)
        {
            //No next wave, end section
            _waveIndex = 0;
            EndSection();
        }
        else
        {
            _waveIndex = (int)nextWave;
            SpawnWave();
        }
    }

    private int? SelectNextWave(int selectedWave)
    {
        if (selectedWave >= _sections[_sectionIndex]._waves.Length)
        {
            return null;
        }
        else if (GameProperties._extraLevelRequirements[GameProperties._currentLevel]
                [_sections[_sectionIndex]._waves[selectedWave]._requirementsIndex] == 0)
        {
            return selectedWave;
        }
        else return SelectNextWave(selectedWave + 1);
    }

    private void SpawnEndWave()
    {
        /*
         si endwave no es null
                Spawnea endwave
        */
    }

    private void DespawnEndWave()
    {
        /*
         si endwave no es null
            despawnea endwave
        */
    }

    public void BackgroundDead()
    {
        _canStartNextSection = true;
    }

    /*        
    Player
        change orientation(orientation)
            rotation = orientation


    Enemies (clase base):
        si son prioritarios
        transform de entrada
        behavior
        path
        waypoints
        transform salida
        offset hasta primer ataque timing entre ataques por ataque
        array de Guns


    Gun -> puede o no tener sprite
        offset entre ataque
        cuanto tarda en atacar de primeras
        si apunta a jugador
        direccion donde apuntar si no apunta a jugador
        ataque para hacer -> puede ser null


    Attack Generator
        poner las propias balas


    Bullet prefabs
         */
}
