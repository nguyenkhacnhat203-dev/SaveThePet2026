using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PaticelFirework : MonoBehaviour
{
    [Range(-180, 180)]
    public float angle = 45f;

    [Range(1, 180)]
    public float spreadAngle = 40f;

    public float minForce = 12f;
    public float maxForce = 14f;

    public int particleCount = 120;

    public float lifeTime = 6f;
    public float size = 1f;

    public float gravity = 9.8f;

    public float destroyAfter = 3.5f;

    public List<Sprite> spriteList = new();

    private ParticleSystem ps;
    private ParticleSystemRenderer psRenderer;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        psRenderer = GetComponent<ParticleSystemRenderer>();

        SetupParticle();
        SetupSprites();
    }

    void Start()
    {
        Burst();
       
    }

    void SetupParticle()
    {
        var main = ps.main;

        main.loop = false;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

#if UNITY_2020_2_OR_NEWER
        main.useUnscaledTime = true;
#else
        main.deltaTime = ParticleSystemDeltaTime.Unscaled;
#endif

        main.gravityModifier = gravity / 9.8f;

        var emission = ps.emission;
        emission.enabled = false;
        emission.rateOverTime = 0;

        var shape = ps.shape;
        shape.enabled = false;
    }

    void SetupSprites()
    {
        if (spriteList == null || spriteList.Count == 0)
            return;

        psRenderer.renderMode = ParticleSystemRenderMode.Billboard;

        var texSheet = ps.textureSheetAnimation;
        texSheet.enabled = true;
        texSheet.mode = ParticleSystemAnimationMode.Sprites;

        for (int i = texSheet.spriteCount - 1; i >= 0; i--)
        {
            texSheet.RemoveSprite(i);
        }

        foreach (var s in spriteList)
        {
            texSheet.AddSprite(s);
        }

        texSheet.frameOverTime = new ParticleSystem.MinMaxCurve(0f);
        texSheet.startFrame = new ParticleSystem.MinMaxCurve(0f, spriteList.Count);
    }

    [ContextMenu("Burst")]
    public void Burst()
    {
        ps.Clear();

        ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams();

        for (int i = 0; i < particleCount; i++)
        {
            float randomAngle =
                angle + Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);

            float rad = randomAngle * Mathf.Deg2Rad;

            Vector3 dir = new Vector3(
                Mathf.Sin(rad),
                Mathf.Cos(rad),
                0f
            );

            float force = Random.Range(minForce, maxForce);

            emit.position = transform.position;
            emit.velocity = dir * force;

            emit.startLifetime = Random.Range(lifeTime * 0.8f, lifeTime);
            emit.startSize = Random.Range(size * 0.7f, size * 1.3f);

            ps.Emit(emit, 1);
        }

        ps.Play();

        StartCoroutine(DestroyRoutine());
    }

    IEnumerator DestroyRoutine()
    {
        yield return new WaitForSecondsRealtime(destroyAfter);
        Destroy(gameObject);
    }
}