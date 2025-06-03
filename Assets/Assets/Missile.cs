using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _rb;
    private Transform _target;

    [Header("Movement")]
    [SerializeField] private float _speed = 1500f;
    [SerializeField] private float _rotateSpeed = 95f;

    [Header("Prediction")]
    [SerializeField] private float _maxDistancePredict = 100f;
    [SerializeField] private float _minDistancePredict = 5f;
    [SerializeField] private float _maxTimePrediction = 5f;
    private Vector3 _standardPrediction, _deviatedPrediction;

    [Header("Deviation")]
    [SerializeField] private float _deviationAmount = 50f;
    [SerializeField] private float _deviationSpeed = 2f;

    [Header("Self-Destruct")]
    [SerializeField] private float _selfDestructTime = 3f; // Time before missile self-destructs
    private float _timeSinceTargetSet;

    private void Start()
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (_target == null) return;

        _timeSinceTargetSet += Time.fixedDeltaTime;
        if (_timeSinceTargetSet >= _selfDestructTime)
        {
            SelfDestruct();
            return;
        }

        _rb.velocity = Vector3.zero;
        _rb.velocity = transform.forward * _speed;

        var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, _target.position));

        PredictMovement(leadTimePercentage);
        AddDeviation(leadTimePercentage);
        RotateRocket();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _timeSinceTargetSet = 0f; // Reset timer when a new target is set
    }

    private void PredictMovement(float leadTimePercentage)
    {
        var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);
        _standardPrediction = _target.position;
    }

    private void AddDeviation(float leadTimePercentage)
    {
        var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);
        var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;
        _deviatedPrediction = _standardPrediction + predictionOffset;
    }

    private void RotateRocket()
    {
        var heading = _deviatedPrediction - transform.position;
        var rotation = Quaternion.LookRotation(heading);
        _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (_target == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _standardPrediction);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
    }

    private void SelfDestruct()
    {
       // Debug.Log("Missile self-destructed due to timeout.");
        Destroy(gameObject);
    }
}
