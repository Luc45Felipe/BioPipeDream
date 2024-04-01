using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class Pipe_Ctrl : MonoBehaviour
{
    public static event Action ImpossibleContinue;
    public static event Action NewPipeFilling;

    [SerializeField] private PipeType _type;
    [SerializeField] private Image _liquidImage;

    private bool _isContedByScore;
    private float _fillTime = 3;

    [Space(2)]
    [SerializeField] private InspectPoint _inspectPoint, _inspectPoint1;

    private bool _isFilling;
    public bool IsFilling
    { 
        get => _isFilling;
        set 
        {
            if(value)
            {
                InspectPoint comeFrom = GetPointWithFullPipe();

                if(comeFrom == null)
                {
                    comeFrom = GetStartablePoint();

                    if(comeFrom == null)
                    {
                        print("impossible start");
                    }
                    else
                    {
                        _isFilling = value;
                        SetFillDirection(comeFrom);

                        if(_isContedByScore)
                        {
                            OnNewPipeFilling();
                        }
                    }
                }
                else
                {
                    SetFillDirection(comeFrom);
                    _isFilling = value;

                    if(_isContedByScore)
                    {
                        OnNewPipeFilling();
                    }
                }
            }
            else
            {
                _isFilling = false;
            }
        }
    }

    private float _fillValue = 0;
    public float FillValue
    {
        get => _fillValue;
        private set 
        {
            _fillValue = value;

            if(_fillValue >= 1)
            {
                InspectPoint toInspect;

                if(IsCurvedPipe(this))
                {
                    toInspect = (_liquidImage.fillClockwise == _inspectPoint.flowConfig) ? _inspectPoint1 : _inspectPoint;
                }
                else
                {
                    toInspect = (_liquidImage.fillOrigin == Convert.ToInt32(_inspectPoint.flowConfig)) ? _inspectPoint1 : _inspectPoint;
                }

                Pipe_Ctrl pipeFinded = GetPipeAtPoint(toInspect);

                if(pipeFinded != null)
                {
                    if(IsCompatible(pipeFinded._type, toInspect))
                    {
                        pipeFinded.IsFilling = true;
                    }
                    else
                    {
                        print("Next pipe is not compatible");
                        OnImpossibleContinue();
                    }
                }
                else
                {
                    print("Pipe not found");
                    OnImpossibleContinue();
                }
            }
        }
    }

    void Update()
    {
        if(IsFilling && FillValue < 1)
        {
            Fill();
        }
    }

    public void SetInitialValues (float fillTime, bool isContedByScore)
    {
        _fillTime = fillTime;
        _isContedByScore = isContedByScore;
    }

    private InspectPoint GetPointWithFullPipe()
    {
        Pipe_Ctrl pipeFinded = GetPipeAtPoint(_inspectPoint);
        Pipe_Ctrl otherPipeFinded = GetPipeAtPoint(_inspectPoint1);

        if(pipeFinded != null && IsCompatible(pipeFinded._type, _inspectPoint) && pipeFinded._liquidImage.fillAmount > .9f)
        {
            return _inspectPoint;
        }
        else if(otherPipeFinded != null && IsCompatible(otherPipeFinded._type, _inspectPoint1) && otherPipeFinded._liquidImage.fillAmount > .9f)
        {
            return _inspectPoint1;
        }
        else
        {
            return null;
        }
    }

    private Pipe_Ctrl GetPipeAtPoint(InspectPoint point)
    {
        Collider2D pipeDetect = Physics2D.OverlapCircle(point.point.position, 10f);

        if(pipeDetect != null)
        {
            Pipe_Ctrl detectFillCtrl = pipeDetect.GetComponent<Pipe_Ctrl>();
            return detectFillCtrl;
        }

        return null;
    }

    private bool IsCompatible(PipeType type, InspectPoint inspectPoint)
    {
        bool isCompatible;

        switch ((int)type)
        {
            case 0:
                isCompatible = inspectPoint.horizontal;
            break;

            case 1:
                isCompatible = inspectPoint.vertical;
            break;

            case 2:
                isCompatible = inspectPoint.rightUp;
            break;

            case 3:
                isCompatible = inspectPoint.rightDown;
            break;

            case 4:
                isCompatible = inspectPoint.leftUp;
            break;

            case 5:
                isCompatible = inspectPoint.leftDown;
            break;

            default:
                isCompatible = false;
            break;
        }

        return isCompatible;
    }

    private InspectPoint GetStartablePoint()
    {
        Pipe_Ctrl pipeFinded = GetPipeAtPoint(_inspectPoint);
        Pipe_Ctrl otherPipeFinded = GetPipeAtPoint(_inspectPoint1);

        if(pipeFinded == null && otherPipeFinded != null)
        {
            return _inspectPoint;
        }
        else if(pipeFinded != null && otherPipeFinded == null)
        {
            return _inspectPoint1;
        }
        else
        {
            return null;
        }
    }

    private void SetFillDirection(InspectPoint comeFrom)
    {
        if(IsCurvedPipe(this))
        {
            _liquidImage.fillClockwise = comeFrom.flowConfig;
        }
        else
        {
            _liquidImage.fillOrigin = Convert.ToInt32(comeFrom.flowConfig);
        }
    }

    private bool IsCurvedPipe(Pipe_Ctrl pipe)
    {
        if(pipe._liquidImage.fillMethod == Image.FillMethod.Radial90)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Fill()
    {
        FillValue += Time.deltaTime / _fillTime;
        _liquidImage.fillAmount = Mathf.Lerp(0,1, FillValue);
    }

    public void ActiveFastMode()
    {
        _fillTime = 0.1f;
    }

    private void OnImpossibleContinue()
    {
        ImpossibleContinue?.Invoke();
    }

    private void OnNewPipeFilling()
    {
        NewPipeFilling?.Invoke();
    }

    [System.Serializable]
    public class InspectPoint
    {
        public Transform point;
        public bool flowConfig;
        [Space(5)]
        public bool horizontal;
        public bool vertical;
        public bool rightUp;
        public bool rightDown;
        public bool leftUp;
        public bool leftDown;
    }
}
