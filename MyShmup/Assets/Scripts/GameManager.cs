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

    public int _sectionIndex;
    [SerializeField]
    private int _waveIndex;
    public Section[] _sections;

    private bool _canStartNextSection = false;
    private BGObject _currentBG = null;
    private BGObject _previousBG = null;
    private int _previousBGIndex = 999;
    private IEnumerator _waitForWave;

    private void Start()
    {
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

        //Waits for message from background saying the new bg can spawn
        if(_canStartNextSection)
        {
            //DespawnEndWave
            StartSection();
            _canStartNextSection = false;
        }
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

        if(!FirstSpawn()) _previousBG = _currentBG;

        //If last background is different from the one being spawned spawn it
        if (SectionBGIsDifferentFromPrevious())
        {
            _currentBG = Instantiate(_sections[_sectionIndex]._backgroundSection._background);            
        }

        //Change scroll speed, TODO lerp the speed
        _currentBG._customSpeed = _sections[_sectionIndex]._backgroundSection._scrollSpeed;
        if(!FirstSpawn()) _previousBG._customSpeed = _sections[_sectionIndex]._backgroundSection._scrollSpeed;

        PlayerController player = FindObjectsOfType<PlayerController>()[0];
        if(player != null)
        {
            player.ChangeOrientation(_sections[_sectionIndex]._shipOrientation);
        }

        SpawnWave();
    }

    private void EndSection()
    {
        _previousBGIndex = _sectionIndex;
        _sectionIndex++;

        Debug.Log("Ending section");
        if (_sectionIndex < _sections.Length)
        {
            if(!SectionBGIsDifferentFromPrevious())
            {
                StartSection();
            }
            else
            {
                //Messages background to die and waits for it to tell when the next bg can be loaded
                _currentBG.KillBG();
                //SpawnEndWave
            }
        }
        /*
         si sectionIndex + 1 not null 
            si background anterior es el mismo que +1
                StartSection

            else
                dice a background que esta dying
                SpawnEndWave();
         TODO cuando un background este muriendo y otro haya aparecido hay que cambiar sus velocidades a lerp entre velocidad de anterior background y el nuevo
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

    private bool SectionBGIsDifferentFromPrevious()
    {
        if (_previousBGIndex == 999 || _sections[_previousBGIndex]._backgroundSection._background != _sections[_sectionIndex]._backgroundSection._background)
            return true;
        else
            return false;
    }

    private bool FirstSpawn()
    {
        if (_previousBGIndex == 999)
            return true;
        else
            return false;
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
