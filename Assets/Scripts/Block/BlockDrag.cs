using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Vector3 _screenPoint;
    private Vector3 _offset;
    private bool _isDragging = false;
    private int _originalLayer;
    public string DragLayerName = "Ignore Raycast";
    public float DragSpeed = 10f;

    public float DragDistance = 0f;

    private Vector3 _targetPosition;

    public void OnPointerDown(PointerEventData eventData)
    {
        BeginDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EndDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragging)
        {
            Vector3 curScreenPoint = new Vector3(eventData.position.x, eventData.position.y, _screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;

            _targetPosition = curPosition;
        }
    }


    private void BeginDrag(PointerEventData eventData)
    {
        _isDragging = true;

        _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, _screenPoint.z));

        if (DragDistance <= 0f)
        {
            DragDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        }

        _originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer(DragLayerName);
    }

    private void EndDrag()
    {
        _isDragging = false;
        gameObject.layer = _originalLayer;
        transform.localPosition = Vector3.zero;
    }


    private void Start()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        if (string.IsNullOrEmpty(DragLayerName))
        {
            Debug.LogError("DragLayerName is not set! Please set it in the Inspector.");
        }

        if(LayerMask.NameToLayer(DragLayerName) == -1)
        {
          Debug.LogError($"Layer '{DragLayerName}' does not exist.  Please create it in Edit > Project Settings > Tags and Layers.");
        }
    }

    private void Update()
    {
        if (_isDragging)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, DragSpeed * Time.deltaTime);
        }
    }
}