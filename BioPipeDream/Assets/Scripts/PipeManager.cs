using System;
using UnityEngine;
using System.Collections.Generic;

public sealed class PipeManager : MonoBehaviour
{
    public static event Action PuzzleCompleted;

    [SerializeField] private int _fillTime;
    private const int _fillTimeDefault = 3;

    [SerializeField] private List<GameObject> _pipePrefabs;
    [SerializeField] private PipeCtrl _startPipe;
    [SerializeField] private PipeCtrl _endPipe;

    [SerializeField] private List<GameObject> _slotList = new List<GameObject>();
    private List<PipeCtrl> _pipeList = new List<PipeCtrl>();

    void Awake()
    {
        _startPipe.IsFilling = true;
        CreatePipes();
    }

    private void CreatePipes()
    {
        List<GameObject> geratedPipeList = new List<GameObject>();

        for (int i = 0; i < _pipePrefabs.Count; i++)
        {
            geratedPipeList.Add(_pipePrefabs[i]);
            geratedPipeList.Add(_pipePrefabs[i]);
        }

        for (int i = geratedPipeList.Count; i < _slotList.Count; i++)
        {
            geratedPipeList.Add(_pipePrefabs[UnityEngine.Random.Range(0, _pipePrefabs.Count)]);
        }

        geratedPipeList = ShufflePrefabs(geratedPipeList);


        if(_slotList.Count > 0)
        {
            for (int i = 0; i < _slotList.Count; i++)
            {
                PipeCtrl newPipe = Instantiate(geratedPipeList[i], _slotList[i].transform.position, Quaternion.identity, _slotList[i].transform).GetComponent<PipeCtrl>();
                newPipe.SetInitialValues(_fillTime > 0 ? _fillTime : _fillTimeDefault, true);
                _pipeList.Add(newPipe);
            }
        }
    }

    private List<GameObject> ShufflePrefabs(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            GameObject temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        return list;
    } 

    private void Update()
    {
        if(_endPipe.IsFilling)
        {
            _endPipe.IsFilling = false;
            OnPuzzleCompleted();
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            print(_fillTime);
            foreach (var pipe in _pipeList)
            {
                pipe.ActiveQuickMode();
            }
        }
    }

    private void OnPuzzleCompleted()
    {
        PuzzleCompleted?.Invoke();
    }
}

