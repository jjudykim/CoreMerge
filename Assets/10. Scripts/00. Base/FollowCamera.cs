using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class FollowCamera : MonoBehaviour
{
    public enum States
    {
        Stopped,              // 완전 정지
        Holding,              // 타겟 유지, 화면 고정
        Following,            // 타겟 추적
        Focusing,             // 연출 대상 / 좌표로 이동
    }
    
    [Header("Target")] 
    [field: SerializeField] private Transform defaultTarget { get; set; }
    private Transform currentTarget;

    [Header("Follow")] 
    [SerializeField] private States currentState;
    [field: SerializeField] private float lerpSpeed = 1.0f; //Damping 값
    [SerializeField] private Vector3 offset;

    [Header("Bounds (Map Clamp)")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private Vector2 boundsMin = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 boundsMax = new Vector2(10f, 10f);
    
    
    private Camera Cam { get; set; }
    private States State { get; set; } = States.Stopped;

    // Focusing
    private Transform focusTarget;
    private Vector3 focusWorldPosition;

    private Coroutine focusRoutine;
    
    private void Awake()
    {
        Cam = GetComponent<Camera>();
        if (Cam == null)
            Cam = Camera.main;
        
        SetTarget(defaultTarget);
        ChangeState(States.Following);
    }

    private void Update()
    {
        #region 테스트 코드
        //if (Input.GetKeyDown(KeyCode.F1))
        //{
        //    ChangeState(States.Stopped);
        //}
        //if (Input.GetKeyDown(KeyCode.F2))
        //{
        //    ChangeState(States.Holding);
        //}
        //if (Input.GetKeyDown(KeyCode.F3))
        //{
        //    SetTarget(Player.LocalPlayer.transform);
        //    ChangeState(States.Following);
        //}
        //if (Input.GetKeyDown(KeyCode.F4))
        //{
        //    ChangeState(States.Following);
        //}
        #endregion
        
        switch (State)
        {
            case States.Stopped:
                return;
            
            case States.Holding:
                return;
            
            case States.Following:
                TickFollow();
                return;
            
            case States.Focusing:
                TickFocus();
                return;
        }
    }

    private void TickFollow()
    {
        if (currentTarget == null)
            return;

        Vector3 desired = currentTarget.position + offset;
        desired.z = transform.position.z;
        
        Vector3 next = Vector3.Lerp(transform.position, desired, lerpSpeed * Time.deltaTime);
        next.z = transform.position.z;

        next = ClampToBounds(next);
        transform.position = next;
    }
    
    private void TickFocus()
    {
        Vector3 desired;

        desired = focusWorldPosition + offset;
        desired.z = transform.position.z;

        Vector3 next = Vector3.Lerp(transform.position, desired, lerpSpeed * Time.deltaTime);
        next.z = transform.position.z;
        
        next = ClampToBounds(next);

        transform.position = next;
    }
    

    private Vector3 ClampToBounds(Vector3 camPos)
    {
        if (useBounds == false || Cam == null)
            return camPos;
        
        float halfH = Cam.orthographicSize;
        float halfW = Cam.orthographicSize * Cam.aspect;

        float minX = boundsMin.x + halfW;
        float maxX = boundsMax.x - halfW;
        float minY = boundsMin.y + halfH;
        float maxY = boundsMax.y - halfH;
        
        if (minX > maxX) camPos.x = (boundsMin.x + boundsMax.x) * 0.5f;
        else camPos.x = Mathf.Clamp(camPos.x, minX, maxX);

        if (minY > maxY) camPos.y = (boundsMin.y + boundsMax.y) * 0.5f;
        else camPos.y = Mathf.Clamp(camPos.y, minY, maxY);

        return camPos;
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
    
    public void ChangeState(States newState)
    {
        State = newState;
    }
    
    public void StopCamera()
    {
        StopFocusRoutine();
        ChangeState(States.Stopped);
    }
    
    public void HoldCamera()
    {
        StopFocusRoutine();
        ChangeState(States.Holding);
    }
    
    public void ResumeFollow()
    {
        StopFocusRoutine();
        ChangeState(States.Following);
    }
    
    public void FocusOnTarget(Transform target, float duration, bool returnToFollow = true)
    {
        if (target == null)
            return;

        StopFocusRoutine();

        focusTarget = target;

        ChangeState(States.Focusing);
        focusRoutine = StartCoroutine(CoFocus(duration, returnToFollow));
    }
    
    public void FocusOnPosition(Vector3 worldPos, float duration, bool returnToFollow = true)
    {
        StopFocusRoutine();

        focusWorldPosition = worldPos;

        ChangeState(States.Focusing);
        focusRoutine = StartCoroutine(CoFocus(duration, returnToFollow));
    }

    private IEnumerator CoFocus(float duration, bool returnToFollow)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        if (returnToFollow)
            ChangeState(States.Following);
    }

    private void StopFocusRoutine()
    {
        if (focusRoutine != null)
        {
            StopCoroutine(focusRoutine);
            focusRoutine = null;
        }
    }
}
