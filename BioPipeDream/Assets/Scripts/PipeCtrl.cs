using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public sealed class PipeCtrl : MonoBehaviour
{
    public static event Action ImpossibleContinue;
    public static event Action AnotherPipeFilling;

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

                if(comeFrom != null)
                {
                    SetFillDirection(comeFrom);
                    _isFilling = value;

                    if(_isContedByScore)
                    {
                        OnAnotherPipeFilling();
                    }
                }
                else
                {
                    comeFrom = GetStartablePoint();

                    if(comeFrom != null)
                    {
                        _isFilling = value;
                        SetFillDirection(comeFrom);

                        if(_isContedByScore)
                        {
                            OnAnotherPipeFilling();
                        }
                    }
                    else
                    {
                        print("impossible start");
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

                PipeCtrl pipeFinded = GetPipeAtPoint(toInspect);

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
        PipeCtrl pipeFinded = GetPipeAtPoint(_inspectPoint);
        PipeCtrl otherPipeFinded = GetPipeAtPoint(_inspectPoint1);

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

    private PipeCtrl GetPipeAtPoint(InspectPoint point)
    {
        Collider2D pipeDetect = Physics2D.OverlapCircle(point.point.position, 10f);

        if(pipeDetect != null)
        {
            PipeCtrl detectFillCtrl = pipeDetect.GetComponent<PipeCtrl>();
            return detectFillCtrl;
        }

        return null;
    }

    private bool IsCompatible(PipeType type, InspectPoint inspectPoint)
    {
        return inspectPoint.Compatibility.HasFlag(TypeToCompatibility(type)) ? true : false;
    }

    private CompatibilityPipes TypeToCompatibility(PipeType type)
    {
        int num = 1 << (int)type;
        return (CompatibilityPipes)num;
    }

    private InspectPoint GetStartablePoint()
    {
        PipeCtrl pipeFinded = GetPipeAtPoint(_inspectPoint);
        PipeCtrl otherPipeFinded = GetPipeAtPoint(_inspectPoint1);

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

    private bool IsCurvedPipe(PipeCtrl pipe)
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

    public void ActiveQuickMode()
    {
        _fillTime = 0.1f;
    }

    private void OnImpossibleContinue()
    {
        ImpossibleContinue?.Invoke();
    }

    private void OnAnotherPipeFilling()
    {
        AnotherPipeFilling?.Invoke();
    }

    [System.Serializable]
    public class InspectPoint
    {
        public Transform point;
        public CompatibilityPipes Compatibility;
        public bool flowConfig;
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

    [Flags]
    public enum CompatibilityPipes
    {
        Horizontal = 1 << 0,
        Vertical = 1 << 1,
        RightUp = 1 << 2,
        RightDown = 1 << 3,
        LeftUp = 1 << 4,
        LeftDown = 1 << 5
    }
}



