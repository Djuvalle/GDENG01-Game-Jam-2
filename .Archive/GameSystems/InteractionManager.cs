using System;
using GameEnum;
using Unity.VisualScripting;
using UnityEngine;
public class InteractionManager : MonoBehaviour
{
    private const float DISTANCE_FROM_CAMERA = 2f;
    public static InteractionManager Instance { get; private set; }
    private Camera cam;
    [SerializeField] private InputManager input;
    private GameObject currentGrabable;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        cam = Camera.main;
    }

    private void Update()
    {
        if (currentGrabable != null)
        {
            currentGrabable.transform.position = cam.transform.position + cam.transform.forward * DISTANCE_FROM_CAMERA;
        }
    }

    private void OnEnable()
    {
        input.OnInputDown += HandleClickDown;
        input.OnInputUp += HandleClickUp;
        input.OnPlayerInteract += HandleInteract;
    }

    private void OnDisable()
    {
        input.OnInputDown -= HandleClickDown;
        input.OnInputUp -= HandleClickUp;
        input.OnPlayerInteract -= HandleInteract;
    }

    private void HandleClickDown(Vector2 mousePos)
    {
        if (currentGrabable != null) return;
        Ray ray = cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Clickable clickable = hit.transform.GetComponent<Clickable>();
            clickable?.OnClicked();
        }
    }

    private void HandleClickUp(Vector2 mousePos)
    {
        if (currentGrabable == null) return;
        Clickable clickable = currentGrabable.transform.GetComponent<Clickable>();
        clickable?.OnClickRelease();
    }

    private void HandleInteract(Vector3 camPos)
    {
        Parameters parameters = new Parameters();
        parameters.PutExtra("x", camPos.x);
        parameters.PutExtra("y", camPos.y);
        parameters.PutExtra("z", camPos.z);
        EventBroadcaster.Instance.PostEvent(ActionEvent.Interacted.ToString(), parameters);
    }

    public static void GrabObject(GameObject obj)
    {
        obj.GetComponent<Rigidbody>().isKinematic = true;
        Instance.currentGrabable = obj;
    }

    public static void ReleaseObject(GameObject obj)
    {
        if (Instance.currentGrabable == obj)
        {
            Instance.currentGrabable = null;
            obj.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
