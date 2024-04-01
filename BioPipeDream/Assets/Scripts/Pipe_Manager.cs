using System;
using UnityEngine;
using System.Collections.Generic;

public sealed class Pipe_Manager : MonoBehaviour
{
    public static event Action PuzzleCompleted;

    [SerializeField] private List<GameObject> _pipePrefabs;
    [SerializeField] private Pipe_Ctrl _startPipe;
    [SerializeField] private Pipe_Ctrl _endPipe;

    [SerializeField] private List<GameObject> _slotList = new List<GameObject>();
    private List<Pipe_Ctrl> _pipeList = new List<Pipe_Ctrl>();

    void Awake()
    {
        CreatePipes();
        _startPipe.IsFilling = true;
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
                Pipe_Ctrl newPipe = Instantiate(geratedPipeList[i], _slotList[i].transform.position, Quaternion.identity, _slotList[i].transform).GetComponent<Pipe_Ctrl>();
                newPipe.SetInitialValues(3, true);
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
            foreach (var pipe in _pipeList)
            {
                pipe.ActiveFastMode();
            }
        }
    }

    private void OnPuzzleCompleted()
    {
        PuzzleCompleted?.Invoke();
    }
}

public enum PipeType 
{ 
    Horizontal,
    Vertical,
    RightToUp,
    RightToDown,
    LeftToUp,
    LeftToDown
}

