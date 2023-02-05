using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    public float DistanceMultiplyer = 2.0f;
    public float MinSpeed = 1.0f;
    public float Speed = 5;
    private AntGroup _antGroup;
    // Start is called before the first frame update
    void Start()
    {
        _antGroup = FindObjectOfType<AntGroup>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var delta = _antGroup.transform.position - transform.position;
        //var asdf = transform.rotation * Vector3.up;
        //Vector3.Dot(delta.normalized, transform.up);
        var fff = Vector3.SignedAngle(delta.normalized, transform.up, Vector3.forward);

        //var a = transform.rotation.eulerAngles;
        //a.x += fff > 0 ? -1f : 1f;
        //transform.rotation = Quaternion.Euler(a);
        if (fff > Random.Range(20, 120))
        {
            transform.Rotate(0, 0, -Random.Range(100, 10000) * Time.deltaTime);
        } else if (fff < -Random.Range(20, 120))
        {
            transform.Rotate(0, 0, Random.Range(100, 10000) * Time.deltaTime);
        }

        //transform.LookAt(_antGroup.transform.position, Vector3.up);

        var distance = delta.magnitude;

        float movement = System.Math.Min(Speed, distance/DistanceMultiplyer + MinSpeed);

        transform.position += transform.up * movement * Time.deltaTime;
    }
}
