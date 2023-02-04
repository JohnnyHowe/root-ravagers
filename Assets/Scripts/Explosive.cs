using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public float TimeUntilExplosion = 3;
    public float ExplosionCountdown;
    public bool Armed = false;
    public float DamageRadius = 3f;
    [Header("Visuals")]
    public Color PassiveColor = Color.white;
    public Color ArmedFadeColor = Color.red;
    public Color PreExplosionColor = Color.black;
    public float PreExplosionColorTime = 0.5f;
    public GameObject ExplosionEffect;
    private RootController _rootController;

    void Start()
    {
        _rootController = GameObject.FindObjectOfType<RootController>();
        ExplosionCountdown = TimeUntilExplosion;
    }

    void Update()
    {
        _UpdateColor();

        if (!Armed) return;
        ExplosionCountdown -= Time.deltaTime;

        if (ExplosionCountdown <= 0)
        {
            _Explode();
        }
    }

    private void _Explode() {
        foreach (RootNode rootNode in _rootController.GetAllRootNodes()) {
            if ((rootNode.Position - transform.position).magnitude < DamageRadius) {
                _rootController.RemoveNode(rootNode);
            }
        }
        Instantiate(ExplosionEffect).transform.position = transform.position;
        Destroy(gameObject);
    }

    private void _UpdateColor()
    {
        float t = 1 - Mathf.InverseLerp(PreExplosionColorTime, TimeUntilExplosion, ExplosionCountdown);

        if (t < 1)
        {
            Renderer.color = Color.Lerp(PassiveColor, ArmedFadeColor, t);
        }
        else
        {
            Renderer.color = PreExplosionColor;
        }
    }
}
