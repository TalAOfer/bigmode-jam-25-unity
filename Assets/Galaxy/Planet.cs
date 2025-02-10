using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using System.Collections;
using DG.Tweening;

public class Planet : MonoBehaviour
{
    public PlanetType planetType;
    private bool shouldShowBackground => planetType != PlanetType.Moon;
    [ShowIf("shouldShowBackground")]
    public SpriteRenderer BackgroundSR;
    public Color BackgroundColor { get; private set; }

    [Header("Planet Properties")]
    public bool shouldOrbit;
    public float gravity = 12f;
    public Planet parentPlanet;
    public bool shouldShowIndicator;
    [ShowIf("shouldShowIndicator")]
    public Sprite indicatorSprite;
    [ShowIf("shouldOrbit")]
    public float speed;
    public bool IsVisible { get; private set; }

    public void OnBecameVisible() => IsVisible = true;
    public void OnBecameInvisible() => IsVisible = false;
    public float PlanetRadius => transform.localScale.x / 2f;
    public float Angle { get; private set; }
    private float orbitRadius;
    public float OrbitRadius => orbitRadius;
    [ShowInInspector, ReadOnly] public bool completed { get; private set; }

    private readonly float DEG2RAD = Mathf.Deg2Rad;

    public void Initialize()
    {
        orbitRadius = parentPlanet != null ?
            Vector2.Distance((Vector2)transform.position, (Vector2)parentPlanet.transform.position) :
            0;

        if (planetType != PlanetType.Moon)
        {
            BackgroundColor = BackgroundSR.color;

            Color transparentColor = BackgroundColor;
            transparentColor.a = 0f;
            BackgroundSR.color = transparentColor;
        }
    }

    public IEnumerator OnPlanetComplete()
    {
        completed = true;
        GameManager.Instance.audioController.PlayOneShot("World interaction/Puzzle Complete");
        yield return CameraController.Instance.WatchPlanetRoutine(this);

        GameManager.Instance.screenFlash.TriggerFlash();
        GameManager.Instance.screenFlash.TriggerPuzzleComplete();

        yield return new WaitForSeconds(1f);

        yield return CameraController.Instance.GoBackToPlayerRoutine();
    }

    public void RescalePremadeDecorations()
    {
        List<DecorationManager> decorationManagers = GetComponentsInChildren<DecorationManager>().ToList();

        if (decorationManagers.Count > 0)
        {
            foreach (DecorationManager manager in decorationManagers)
            {
                manager.RescaleDecorations();
            }
        }
    }

    public void UpdatePlanet(float dt)
    {
        if (shouldOrbit && parentPlanet != null)
        {
            Angle += speed * dt;
            transform.position = new Vector2(
                parentPlanet.transform.position.x + Mathf.Sin(Angle * DEG2RAD) * orbitRadius,
                parentPlanet.transform.position.y + Mathf.Cos(Angle * DEG2RAD) * orbitRadius
            );

            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerPickup")) 
        {
            GameManager.Instance.backgroundManager.FadePlanetSkybox(this, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerPickup"))
        {
            GameManager.Instance.backgroundManager.FadePlanetSkybox(this, false);
        }
    }
}

public enum PlanetType
{
    Sun,
    Planet,
    Moon
}