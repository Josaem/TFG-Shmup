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
        public GameObject _endWave;
        public Transform _playerStartingPosition;
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
        public int _spawnRequirementsIndex = GameProperties._extraLevelRequirements[0][0];
        public float _duration = 1;
        public float _delayUntilNext = 0;
        public GameObject _enemyWave;
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
    private bool _priorityWave = false;
    [HideInInspector]
    public int _priorityEnemiesLeft = 0;
    private GameObject _currentWave;

    private void Start()
    {
        StartSection();
    }

    private void Update()
    {
        if(_priorityWave)
        {
            if (_priorityEnemiesLeft <= 0)
            {
                EndWave();
                if(_waitForWave != null)
                    StopCoroutine(_waitForWave);
                _priorityWave = false;
            }
        }

        //Waits for message from background saying the new bg can spawn
        if(_canStartNextSection)
        {
            if(IsThereEndWave())
            {
                DespawnEndWave();
            }
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

        //Change scroll speed, TODOVISUALS lerp the speed
        _currentBG._customSpeed = _sections[_sectionIndex]._backgroundSection._scrollSpeed;
        if(!FirstSpawn()) _previousBG._customSpeed = _sections[_sectionIndex]._backgroundSection._scrollSpeed;

        PlayerController player = FindObjectsOfType<PlayerController>()[0];
        if(player != null)
        {
            player.ChangeOrientation(_sections[_sectionIndex]._shipOrientation);
            if(_sections[_sectionIndex]._playerStartingPosition != null)
            {
                player.transform.position = _sections[_sectionIndex]._playerStartingPosition.position;
            }
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
                SpawnEndWave();
            }
        }
    }

    public void AllowStartNextSection()
    {
        _canStartNextSection = true;
    }

    private void SpawnWave()
    {
        Debug.Log("Spawning wave: " + _waveIndex);

        //spawnea enemyWave si hay
        if (IsThereEnemyWave())
        {
            _currentWave = Instantiate(_sections[_sectionIndex]._waves[_waveIndex]._enemyWave);
        }

        //if wave is not endless set timer
        if(_sections[_sectionIndex]._waves[_waveIndex]._duration != 0)
        {
            //Set timer to forcibly end wave
            _waitForWave = WaitForWave(_sections[_sectionIndex]._waves[_waveIndex]._duration);
            StartCoroutine(_waitForWave);
        }

        if(IsThereEnemyWave())
        {
            _currentWave.GetComponent<WaveObject>().SetPriorityEnemies();
            if (_priorityEnemiesLeft > 0)
            {
                _priorityWave = true;
            }
        }
    }

    private IEnumerator WaitForWave(float waveDuration)
    {
        yield return new WaitForSeconds(waveDuration);
        EndWave();
    }

    public void EndWave()
    {
        if (IsThereEnemyWave())
        {
            DespawnWave();
        }

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
                [_sections[_sectionIndex]._waves[selectedWave]._spawnRequirementsIndex] == 0)
        {
            return selectedWave;
        }
        else return SelectNextWave(selectedWave + 1);
    }

    private bool IsThereEnemyWave()
    {
        if (_sections[_sectionIndex]._waves[_waveIndex]._enemyWave != null)
        {
            return true;
        }
        else
            return false;
    }

    private void SpawnEndWave()
    {
        if(IsThereEndWave())
        {
            Debug.Log("Spawning EndWave");
            _currentWave = Instantiate(_sections[_sectionIndex-1]._endWave);
        }
    }

    private void DespawnWave()
    {
        Debug.Log("Despawning Wave");
        if(_currentWave != null)
            _currentWave.GetComponent<WaveObject>().DespawnWave();
    }

    private void DespawnEndWave()
    {
        Debug.Log("Despawning EndWave");
        if(_currentWave != null)
            _currentWave.GetComponent<WaveObject>().DespawnWave();
    }

    private bool IsThereEndWave()
    {
        if (_sections[_sectionIndex-1]._endWave != null)
        {
            return true;
        }
        else
            return false;
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
}
