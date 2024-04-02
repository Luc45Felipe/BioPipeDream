using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class PipeMov : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
{
    private PipeCtrl _pipeCtrl;
    private Transform _currentParent;
    private Vector3 _previousSlotPoint;
    private Collider2D _collider;
    private bool _isDragging;
    private float _timer;

    private Image[] _images;

    private void Start()
    {
        _pipeCtrl = GetComponent<PipeCtrl>();
        _collider = GetComponent<Collider2D>();
        _images = GetComponentsInChildren<Image>();

        _currentParent = transform.parent;
        transform.localPosition = Vector3.zero;
        UpdateSlot();
    }

    private void Update()
    {
        if(_isDragging == false)
        {
            if(Vector3.Distance(transform.position, _currentParent.position) > 1)
            {
                _timer += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(_previousSlotPoint, Vector3.zero, _timer * 5);
            }
            else
            {
                _timer = 0;
                transform.localPosition = Vector3.zero;
            }
        }
    }

    private void UpdateSlot()
    {
        _previousSlotPoint = transform.localPosition;
    }

    private void SetRaycastTarget(bool isDetectable)
    {
        _collider.enabled = isDetectable;
        foreach (var item in _images)
        {
            item.raycastTarget = isDetectable;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(_pipeCtrl.IsFilling == false)
        {
            _currentParent = transform.parent;
            _isDragging = true;

            transform.SetParent(transform.root.root);
            transform.SetAsLastSibling();

            SetRaycastTarget(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(_isDragging) { transform.position = Input.mousePosition; }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if(_isDragging)
        {
            _isDragging = false;
            transform.SetParent(_currentParent);
            UpdateSlot();
            SetRaycastTarget(true);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(_pipeCtrl.IsFilling == false)
        {
            GameObject other = eventData.pointerDrag;
            PipeMov otherComponent = other.GetComponent<PipeMov>();

            if(otherComponent._pipeCtrl.IsFilling == false)
            {
                Transform otherParent = otherComponent._currentParent;

                otherComponent.SetNewParent(_currentParent);
                SetNewParent(otherParent);

                UpdateSlot();
            }
        }
    }

    private void SetNewParent(Transform newParent)
    {
        _currentParent = newParent;
        transform.SetParent(newParent);
    }
}
