using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class WaveSystemController : MonoBehaviour
{
    private enum SpawnState {SPAWNING,WAITING,COUNTING };

    [System.Serializable]
    public class Wave
    {
        public string Name; //Nome da onda
        public GameObject[] Enemys; // inimigos que podem ser instanciados
        public int EnemyCount; // quantidade de inimigos da onda
        public float SpawnRate; // tempo entre cada inimigo instanciado
    }
    [SerializeField] private Wave[] Waves;
    [SerializeField] private Transform[] SpawnPoints;

    private int NextWave = 0; //Index da proxima onda na Array
    [SerializeField] private float TimeBetweenWaves; // Tempo de espera entre cada onda
    [SerializeField] private float WaveCountDown; // Contagem regressiva para proxima onda
    private float TimeVerification; // tempo entre cada verificação para ver se a inimigos vivos

    [SerializeField]private SpawnState State = SpawnState.COUNTING;

    void Start()
    {
        WaveCountDown = TimeBetweenWaves;

        GMC = GameObject.FindGameObjectWithTag("GameController");
    }

    void Update()
    {
        if (State == SpawnState.WAITING)
        {
            if (!EnemysIsAlive())
            {
                WaveAsCompleted();
            }
            else
            {
                return;
            }
        }

        if (WaveCountDown <= 0)
        {
            if (State != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(Waves[NextWave]));
            }
        }
        else
        {
            WaveCountDown -= Time.deltaTime;
        }
    }

    private IEnumerator SpawnWave(Wave _wave)
    {
        State = SpawnState.SPAWNING;

        for (int I = 0; I < _wave.EnemyCount; I++)
        {
            int Rand = Random.Range(0, _wave.Enemys.Length);
            SpawnEnemy(_wave.Enemys[Rand]);
            yield return new WaitForSeconds(1f/_wave.SpawnRate);
        }

        State = SpawnState.WAITING;
        yield break;
    }
    private void SpawnEnemy(GameObject _enemy)
    {
        int Rand = Random.Range(0, SpawnPoints.Length);
        Instantiate(_enemy, SpawnPoints[Rand].position, Quaternion.identity);
    }

    private void WaveAsCompleted()
    {
        State = SpawnState.COUNTING;
        WaveCountDown = TimeBetweenWaves;

        if (NextWave + 1 > Waves.Length - 1)
        {
            Debug.Log("Room Completed");
        }
        else
        {
            NextWave++;
        }
    }

    private bool EnemysIsAlive()
    {
        //Verificar se a inimigos vivos
        TimeVerification -= Time.deltaTime;
        if (TimeVerification <= 0f)
        {
            TimeVerification = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }
}
