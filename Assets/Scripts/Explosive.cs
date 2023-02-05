using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : PowerUp
{
    public SpriteRenderer Renderer;
    public float TimeUntilExplosion = 3;
    public float ExplosionCountdown;
    public bool Armed = false;
    public float DamageRadius = 3f;
    [Header("Visuals")]
    public Color UnArmedColor = Color.white;
    public Color PassiveColor = Color.yellow;
    public Color ArmedFadeColor = Color.red;
    public Color PreExplosionColor = Color.black;
    public float PreExplosionColorTime = 0.5f;
    public GameObject ExplosionEffect;
    private RootController _rootController;
    private GameController _gameController;
    public float MoveUpDistance = 5;
    public float DesiredY;
    public float MoveUpTime = 0.5f;
    private float _moveUpTimer;
    private bool _isReadyForPickup = false;

    void Start()
    {
        _rootController = GameObject.FindObjectOfType<RootController>();
        _gameController = GameObject.FindObjectOfType<GameController>();
        ExplosionCountdown = TimeUntilExplosion;

        DesiredY = transform.position.y;
        _moveUpTimer = MoveUpTime;

        Vector3 p = transform.position;
        p.y = transform.position.y - MoveUpDistance;
        transform.position = p;
    }

    void Update()
    {
        if (!_isReadyForPickup)
        {
            IsAvailable = false;
            _moveUpTimer = Mathf.Max(_moveUpTimer - Time.deltaTime, 0);
            float t = 1 - (_moveUpTimer / MoveUpTime);

            Vector3 p = transform.position;
            p.y = Mathf.Lerp(DesiredY - MoveUpDistance, DesiredY, t);
            transform.position = p;

            if (t == 1) _isReadyForPickup = true;
        }
        else
        {
            IsAvailable = !Armed;
            _UpdateColor();

            if (!Armed) return;
            ExplosionCountdown -= Time.deltaTime;

            if (ExplosionCountdown <= 0)
            {
                _Explode();
            }
        }
    }

    private float MoveCurve(float t)
    {
        return t;
    }

    private void _Explode()
    {
        int rootDamageCount = 0;
        foreach (RootNode rootNode in _rootController.GetAllRootNodes())
        {
            if (((Vector2)(rootNode.Position - transform.position)).magnitude < DamageRadius)
            {
                _rootController.RemoveNode(rootNode);
                rootDamageCount++;
            }
        }
        Instantiate(ExplosionEffect).transform.position = transform.position;
        Destroy(gameObject);
        _gameController.Score += rootDamageCount * 10;
    }

    private void _UpdateColor()
    {
        if (!Armed)
        {
            Renderer.color = UnArmedColor;
            return;
        }

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

    public override void OnTaskComplete()
    {
        Armed = true;
    }
}
