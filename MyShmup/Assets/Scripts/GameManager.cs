using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Section
    {
        public Orientation _shipOrientation;
        public BackgroundSection _backgroundSection;
        public BGObject[] _backgroundObjects;
        public Wave[] _waves;
    }

    [System.Serializable]
    public class BackgroundSection
    {
        public float _scrollSpeed;
        public BGObject _background;
    }

    [System.Serializable]
    public class Wave
    {
        public int requisitosPorCumplirIndex = GameProperties._extraLevelRequirements[0][0];
        public float _duration = 1;
        public float _delayUntilNext = 0;
        //TODO lista de enemigos con atributos para asignar
    }

    [SerializeField]
    private int _sectionIndex;
    [SerializeField]
    private int _waveIndex;
    [SerializeField]
    private Section[] _sections;

    private bool _canStartNextSection = false;
    private int _previousbackgroundindex = 9999;

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
                Desactiva timer de EndWave
                prioritywave = false

        si canStartNextSection
            DespawnEndWave()
            StartSection()
            canStartNextSection = false
         */
    }

    private void StartSection()
    {
        /*
         si backgroundObjects no es null
            spawnea backgroundObjects
    
        si sectionIndex es distinto de 0
            previousBackgroundIndex = null

        si backgroundSection anterior es distinto del actual pone backgroundSection
        else setea la velocidad de scroll del backgroundSection

        pone orientacion del jugador si esta ha cambiado -> player.changeOrientation(orientation)

        SpawnWave()
        */
    }

    private void EndSection()
    {
        /*
         si sectionIndex + 1 not null 
            sectionIndex++

            si background anterior es el mismo que +1
                StartSection

            else
                dice a background que esta dying
                SpawnEndWave();
        */
    }

    private void SpawnWave()
    {
        /*
         si waveObject no es null
            spawnea waveobject
            pone un timer coroutine de EndWave basado en el waveObject

            cuenta enemigos de prioridad de la wave -> priorityEnemyAmount
            si priorityEnemyAmount es mayor que 0 -> es wave de enemigos de prioridad
                    setea priorityEnemiesLeft = priorityEnemyAmount
            prioritywave es true
        else
            EndWave()
        */
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

    private IEnumerator WaitForWave(float waveDuration)
    {
        yield return new WaitForSeconds(waveDuration);
        EndWave();
    }

    public void EndWave()
    {
        /*
        si waveObject no es null
            despawnea waveObject
        Invoke("GetNextWave", offset)
        */
    }

    public void GetNextWave()
    {
        /*
        int waveSelected = currentWveIndex

        nextWave = SelectNextWave(waveSelected)

        si nextWave es null EndSection()
        else
            currentWaveIndex = nextWave;
            SpawnWave();
        */
    }

    private int SelectNextWave(int selectedWave)
    {
        /*
        if waves[selectedWave + 1] == null
            return null;
        else if waves[selectedWave + 1].requisitos por cumplir == 0 usar persistent settings requirements[level-1][index]
            return selectedWave + 1;
        else
            return SelectNextWave(selectedWave + 1)*/
        return selectedWave; //quitar esto
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
