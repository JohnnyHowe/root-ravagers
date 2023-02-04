using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    public float DistanceMultiplyer = 2.0f;
    public float MinSpeed = 1.0f;
    public float Speed = 5;
    private AntGroup _antGroup;

    private float _currentSpeed = 0;
    private float _currentRotation = 0;
    private float _timeToNextCheck = 0;

    // Start is called before the first frame update
    void Start()
    {
        _antGroup = FindObjectOfType<AntGroup>();
    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(0, 0, _currentRotation * Time.deltaTime);
        transform.position += transform.up * _currentSpeed * Time.deltaTime;

        _timeToNextCheck -= Time.deltaTime;

        var delta = _antGroup.transform.position - transform.position;
        var signedAngle = Vector3.SignedAngle(delta.normalized, transform.up, Vector3.forward);

        if (Mathf.Abs(signedAngle) > 90)
        {
            _timeToNextCheck = 0;
        }


        if (_timeToNextCheck > 0)
        {
            return;
        }


        _timeToNextCheck = Random.Range(0.01f, 0.5f);


        if (signedAngle > Random.Range(1, 30))
        {
            _currentRotation = -Random.Range(1, 2000);
        } else if (signedAngle < -Random.Range(1, 120))
        {
            _currentRotation = Random.Range(1, 2000);
        }


        var distance = delta.magnitude;
        _currentSpeed = System.Math.Min(Speed * Random.Range(0.5f, 2f), distance * Random.Range(0.5f, 2f) + MinSpeed);
    }
}
