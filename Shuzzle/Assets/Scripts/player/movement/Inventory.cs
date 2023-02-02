using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Current Held Item")]
    [SerializeField] private GameObject _currentObject;

    [Header("Item Place Holder")] 
    [SerializeField] private Transform _itemPosHolder;

    [Header("LayerMask References")]
    [SerializeField] private LayerMask _isInteractable;
    
    
    // The tools to determine an interactable object is infront of us
    private Camera _cam;
    private Ray _rayToObject;
    
    // store our hit data on a Raycast
    private RaycastHit _hit;
    
    // Data to use on our newly acquired item
    private Vector3 _reduceSize;
    [SerializeField] private Vector3 _sizeOrigin;

    // Start is called before the first frame update
    void Start()
    {
        _cam = Camera.main;
        _reduceSize = new Vector3(0.25f, 0.25f, 0.25f);
        
    }

    // Update is called once per frame
    void Update()
    {
        _rayToObject = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Input.GetKeyDown(KeyCode.E))
        {
            // if an object is front of you
                // attempt to pick it up
                // or drop your current 
            if (Physics.Raycast(_rayToObject, out _hit, 2, _isInteractable) && _currentObject == null)
            {
                // Debug.Log(hit.transform);
                
                _currentObject = _hit.transform.gameObject;
                _sizeOrigin = _currentObject.transform.localScale;
                _currentObject.transform.localScale = _reduceSize;
                
                _hit.transform.SetParent(_itemPosHolder);
                _currentObject.transform.position = _itemPosHolder.position;
                _currentObject.transform.forward = _itemPosHolder.forward;
            }
            else if(_currentObject != null)
            {
                // drop item from hands
                // Debug.Log("dropping item");
                
                _currentObject.transform.SetParent(null);
                _currentObject.transform.position += _currentObject.transform.forward * 2;
                
                _currentObject.transform.localScale = _sizeOrigin;
                _currentObject = null;
            }
        }
        
    }
}
