using UnityEngine;
using System.Collections;

/// <summary>
/// Runtime controller for the OctopusCamouflage shader.
/// Attach to any GameObject with a Renderer using the Custom/OctopusCamouflage shader.
/// Provides methods to trigger camouflage transitions, threat displays, passing clouds, etc.
/// All transitions are smoothly animated via coroutines.
/// </summary>
public class OctopusCamouflageController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Leave null to auto-find on this GameObject")]
    public Renderer targetRenderer;

    [Header("Transition Settings")]
    [Tooltip("Default duration for color transitions")]
    public float defaultTransitionDuration = 1.5f;

    [Tooltip("Easing curve for transitions")]
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Cached material instance
    Material _mat;

    // Coroutine handles for stopping overlapping transitions
    Coroutine _expansionCoroutine;
    Coroutine _camoCoroutine;
    Coroutine _waveCoroutine;
    Coroutine _papillaeCoroutine;

    // Property IDs (cached for performance)
    static readonly int ID_RedExpansion = Shader.PropertyToID("_RedExpansion");
    static readonly int ID_YellowExpansion = Shader.PropertyToID("_YellowExpansion");
    static readonly int ID_BrownExpansion = Shader.PropertyToID("_BrownExpansion");
    static readonly int ID_BlackExpansion = Shader.PropertyToID("_BlackExpansion");
    static readonly int ID_WaveAmplitude = Shader.PropertyToID("_WaveAmplitude");
    static readonly int ID_WaveSpeed = Shader.PropertyToID("_WaveSpeed");
    static readonly int ID_WaveFrequency = Shader.PropertyToID("_WaveFrequency");
    static readonly int ID_WaveDecay = Shader.PropertyToID("_WaveDecay");
    static readonly int ID_WaveOrigin = Shader.PropertyToID("_WaveOrigin");
    static readonly int ID_IridophoreStrength = Shader.PropertyToID("_IridophoreStrength");
    static readonly int ID_IridophoreSpeed = Shader.PropertyToID("_IridophoreSpeed");
    static readonly int ID_PapillaeStrength = Shader.PropertyToID("_PapillaeStrength");
    static readonly int ID_PapillaeScale = Shader.PropertyToID("_PapillaeScale");
    static readonly int ID_PapillaeSharpness = Shader.PropertyToID("_PapillaeSharpness");
    static readonly int ID_CamoBlend = Shader.PropertyToID("_CamoBlend");
    static readonly int ID_TargetColor = Shader.PropertyToID("_TargetColor");
    static readonly int ID_LeucophoreColor = Shader.PropertyToID("_LeucophoreColor");
    static readonly int ID_PulseSpeed = Shader.PropertyToID("_PulseSpeed");
    static readonly int ID_PulseAmplitude = Shader.PropertyToID("_PulseAmplitude");
    static readonly int ID_GlobalSpeed = Shader.PropertyToID("_GlobalSpeed");
    static readonly int ID_BaseTextureStrength = Shader.PropertyToID("_BaseTextureStrength");
    static readonly int ID_ChromatophoreOpacity = Shader.PropertyToID("_ChromatophoreOpacity");
    static readonly int ID_LeucophoreOpacity = Shader.PropertyToID("_LeucophoreOpacity");

    // ─────────────────────────────────────────────
    // SNAPSHOT: stores a full state for lerping
    // ─────────────────────────────────────────────
    public struct CamoState
    {
        public float redExpansion;
        public float yellowExpansion;
        public float brownExpansion;
        public float blackExpansion;
        public float waveAmplitude;
        public float waveSpeed;
        public float iridophoreStrength;
        public float papillaeStrength;
        public float camoBlend;
        public Color targetColor;
        public Color leucophoreColor;
        public float chromatophoreOpacity;
        public float baseTextureStrength;

        public static CamoState Lerp(CamoState a, CamoState b, float t)
        {
            return new CamoState
            {
                redExpansion = Mathf.Lerp(a.redExpansion, b.redExpansion, t),
                yellowExpansion = Mathf.Lerp(a.yellowExpansion, b.yellowExpansion, t),
                brownExpansion = Mathf.Lerp(a.brownExpansion, b.brownExpansion, t),
                blackExpansion = Mathf.Lerp(a.blackExpansion, b.blackExpansion, t),
                waveAmplitude = Mathf.Lerp(a.waveAmplitude, b.waveAmplitude, t),
                waveSpeed = Mathf.Lerp(a.waveSpeed, b.waveSpeed, t),
                iridophoreStrength = Mathf.Lerp(a.iridophoreStrength, b.iridophoreStrength, t),
                papillaeStrength = Mathf.Lerp(a.papillaeStrength, b.papillaeStrength, t),
                camoBlend = Mathf.Lerp(a.camoBlend, b.camoBlend, t),
                targetColor = Color.Lerp(a.targetColor, b.targetColor, t),
                leucophoreColor = Color.Lerp(a.leucophoreColor, b.leucophoreColor, t),
                chromatophoreOpacity = Mathf.Lerp(a.chromatophoreOpacity, b.chromatophoreOpacity, t),
                baseTextureStrength = Mathf.Lerp(a.baseTextureStrength, b.baseTextureStrength, t),
            };
        }
    }

    // ─────────────────────────────────────────────
    // BUILT-IN STATES
    // ─────────────────────────────────────────────
    public static readonly CamoState StateResting = new CamoState
    {
        redExpansion = 0.2f,
        yellowExpansion = 0.15f,
        brownExpansion = 0.3f,
        blackExpansion = 0.1f,
        waveAmplitude = 0.1f,
        waveSpeed = 0.5f,
        iridophoreStrength = 0.15f,
        papillaeStrength = 0f,
        camoBlend = 0f,
        targetColor = new Color(0.4f, 0.45f, 0.35f),
        leucophoreColor = new Color(0.85f, 0.82f, 0.78f),
        chromatophoreOpacity = 0.7f,
        baseTextureStrength = 1.0f
    };

    public static readonly CamoState StateAlert = new CamoState
    {
        redExpansion = 0.85f,
        yellowExpansion = 0.4f,
        brownExpansion = 0.6f,
        blackExpansion = 0.7f,
        waveAmplitude = 0.8f,
        waveSpeed = 5f,
        iridophoreStrength = 0.5f,
        papillaeStrength = 0.5f,
        camoBlend = 0f,
        targetColor = new Color(0.4f, 0.45f, 0.35f),
        leucophoreColor = new Color(0.85f, 0.82f, 0.78f),
        chromatophoreOpacity = 0.7f,
        baseTextureStrength = 1.0f
    };

    public static readonly CamoState StateThreat = new CamoState
    {
        redExpansion = 1f,
        yellowExpansion = 0.7f,
        brownExpansion = 0f,
        blackExpansion = 0.9f,
        waveAmplitude = 1f,
        waveSpeed = 8f,
        iridophoreStrength = 0.7f,
        papillaeStrength = 1.5f,
        camoBlend = 0f,
        targetColor = new Color(0.4f, 0.45f, 0.35f),
        leucophoreColor = new Color(0.85f, 0.82f, 0.78f),
        chromatophoreOpacity = 0.7f,
        baseTextureStrength = 1.0f
    };

    public static readonly CamoState StateSandyCamo = new CamoState
    {
        redExpansion = 0.1f,
        yellowExpansion = 0.6f,
        brownExpansion = 0.4f,
        blackExpansion = 0.05f,
        waveAmplitude = 0.15f,
        waveSpeed = 0.3f,
        iridophoreStrength = 0.05f,
        papillaeStrength = 0.8f,
        camoBlend = 0.4f,
        targetColor = new Color(0.76f, 0.7f, 0.55f),
        leucophoreColor = new Color(0.9f, 0.85f, 0.7f),
        chromatophoreOpacity = 0.7f,
        baseTextureStrength = 1.0f
    };

    public static readonly CamoState StateDeepOcean = new CamoState
    {
        redExpansion = 0.7f,
        yellowExpansion = 0f,
        brownExpansion = 0.8f,
        blackExpansion = 0.6f,
        waveAmplitude = 0.3f,
        waveSpeed = 1f,
        iridophoreStrength = 0.6f,
        papillaeStrength = 0f,
        camoBlend = 0f,
        targetColor = new Color(0.4f, 0.45f, 0.35f),
        leucophoreColor = new Color(0.15f, 0.2f, 0.35f),
        chromatophoreOpacity = 0.7f,
        baseTextureStrength = 1.0f
    };

    // ─────────────────────────────────────────────
    // LIFECYCLE
    // ─────────────────────────────────────────────
    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
            _mat = targetRenderer.material; // creates instance
    }

    void OnDestroy()
    {
        // Clean up material instance
        if (_mat != null)
            Destroy(_mat);
    }

    // ─────────────────────────────────────────────
    // PUBLIC API
    // ─────────────────────────────────────────────

    /// <summary>
    /// Smoothly transition to a predefined camouflage state.
    /// </summary>
    public void TransitionToState(CamoState target, float duration = -1f)
    {
        if (duration < 0) duration = defaultTransitionDuration;
        CamoState current = CaptureCurrentState();
        StopAllCamoCoroutines();
        _expansionCoroutine = StartCoroutine(AnimateStateTransition(current, target, duration));
    }

    /// <summary>
    /// Instantly snap to a state with no transition.
    /// </summary>
    public void SetStateImmediate(CamoState state)
    {
        StopAllCamoCoroutines();
        ApplyState(state);
    }

    /// <summary>
    /// Fire a "passing cloud" — a dark wave traveling across the body.
    /// This is a signature octopus behavior.
    /// </summary>
    public void TriggerPassingCloud(Vector2 direction, float speed = 3f, float duration = 3f)
    {
        if (_waveCoroutine != null) StopCoroutine(_waveCoroutine);
        _waveCoroutine = StartCoroutine(PassingCloudRoutine(direction, speed, duration));
    }

    /// <summary>
    /// Trigger a startled flash — briefly max out all chromatophores then return.
    /// </summary>
    public void TriggerStartleFlash(float flashDuration = 0.3f, float recoverDuration = 1.5f)
    {
        StopAllCamoCoroutines();
        StartCoroutine(StartleFlashRoutine(flashDuration, recoverDuration));
    }

    /// <summary>
    /// Set the wave origin point in UV space (0-1).
    /// Useful for making waves originate from a hit point.
    /// </summary>
    public void SetWaveOrigin(Vector2 uvPoint)
    {
        if (_mat != null)
            _mat.SetVector(ID_WaveOrigin, new Vector4(uvPoint.x, uvPoint.y, 0, 0));
    }

    /// <summary>
    /// Directly set a single pigment channel expansion (0-1).
    /// </summary>
    public void SetPigmentExpansion(PigmentChannel channel, float expansion)
    {
        if (_mat == null) return;
        expansion = Mathf.Clamp01(expansion);

        switch (channel)
        {
            case PigmentChannel.Red: _mat.SetFloat(ID_RedExpansion, expansion); break;
            case PigmentChannel.Yellow: _mat.SetFloat(ID_YellowExpansion, expansion); break;
            case PigmentChannel.Brown: _mat.SetFloat(ID_BrownExpansion, expansion); break;
            case PigmentChannel.Black: _mat.SetFloat(ID_BlackExpansion, expansion); break;
        }
    }

    /// <summary>
    /// Transition camouflage blend toward a target color.
    /// </summary>
    public void BlendToCamoColor(Color targetColor, float blend, float duration = -1f)
    {
        if (duration < 0) duration = defaultTransitionDuration;
        if (_camoCoroutine != null) StopCoroutine(_camoCoroutine);
        _camoCoroutine = StartCoroutine(AnimateCamoBlend(targetColor, blend, duration));
    }

    // ─────────────────────────────────────────────
    // ENUMS
    // ─────────────────────────────────────────────
    public enum PigmentChannel { Red, Yellow, Brown, Black }

    // ─────────────────────────────────────────────
    // INTERNAL
    // ─────────────────────────────────────────────
    CamoState CaptureCurrentState()
    {
        if (_mat == null) return StateResting;
        return new CamoState
        {
            redExpansion = _mat.GetFloat(ID_RedExpansion),
            yellowExpansion = _mat.GetFloat(ID_YellowExpansion),
            brownExpansion = _mat.GetFloat(ID_BrownExpansion),
            blackExpansion = _mat.GetFloat(ID_BlackExpansion),
            waveAmplitude = _mat.GetFloat(ID_WaveAmplitude),
            waveSpeed = _mat.GetFloat(ID_WaveSpeed),
            iridophoreStrength = _mat.GetFloat(ID_IridophoreStrength),
            papillaeStrength = _mat.GetFloat(ID_PapillaeStrength),
            camoBlend = _mat.GetFloat(ID_CamoBlend),
            targetColor = _mat.GetColor(ID_TargetColor),
            leucophoreColor = _mat.GetColor(ID_LeucophoreColor),
            chromatophoreOpacity = _mat.GetFloat(ID_ChromatophoreOpacity),
            baseTextureStrength = _mat.GetFloat(ID_BaseTextureStrength),
        };
    }

    void ApplyState(CamoState state)
    {
        if (_mat == null) return;
        _mat.SetFloat(ID_RedExpansion, state.redExpansion);
        _mat.SetFloat(ID_YellowExpansion, state.yellowExpansion);
        _mat.SetFloat(ID_BrownExpansion, state.brownExpansion);
        _mat.SetFloat(ID_BlackExpansion, state.blackExpansion);
        _mat.SetFloat(ID_WaveAmplitude, state.waveAmplitude);
        _mat.SetFloat(ID_WaveSpeed, state.waveSpeed);
        _mat.SetFloat(ID_IridophoreStrength, state.iridophoreStrength);
        _mat.SetFloat(ID_PapillaeStrength, state.papillaeStrength);
        _mat.SetFloat(ID_CamoBlend, state.camoBlend);
        _mat.SetColor(ID_TargetColor, state.targetColor);
        _mat.SetColor(ID_LeucophoreColor, state.leucophoreColor);
        _mat.SetFloat(ID_ChromatophoreOpacity, state.chromatophoreOpacity);
        _mat.SetFloat(ID_BaseTextureStrength, state.baseTextureStrength);
    }

    void StopAllCamoCoroutines()
    {
        if (_expansionCoroutine != null) StopCoroutine(_expansionCoroutine);
        if (_camoCoroutine != null) StopCoroutine(_camoCoroutine);
        if (_waveCoroutine != null) StopCoroutine(_waveCoroutine);
        if (_papillaeCoroutine != null) StopCoroutine(_papillaeCoroutine);
    }

    // ─────────────────────────────────────────────
    // COROUTINES
    // ─────────────────────────────────────────────
    IEnumerator AnimateStateTransition(CamoState from, CamoState to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = transitionCurve.Evaluate(Mathf.Clamp01(elapsed / duration));
            ApplyState(CamoState.Lerp(from, to, t));
            yield return null;
        }
        ApplyState(to);
    }

    IEnumerator PassingCloudRoutine(Vector2 direction, float speed, float duration)
    {
        if (_mat == null) yield break;

        // Save current wave settings
        float origAmp = _mat.GetFloat(ID_WaveAmplitude);
        float origSpeed = _mat.GetFloat(ID_WaveSpeed);
        float origDecay = _mat.GetFloat(ID_WaveDecay);

        // Configure passing cloud wave
        _mat.SetFloat(ID_WaveAmplitude, 0.9f);
        _mat.SetFloat(ID_WaveSpeed, speed);
        _mat.SetFloat(ID_WaveDecay, 0.3f);

        yield return new WaitForSeconds(duration);

        // Restore
        _mat.SetFloat(ID_WaveAmplitude, origAmp);
        _mat.SetFloat(ID_WaveSpeed, origSpeed);
        _mat.SetFloat(ID_WaveDecay, origDecay);
    }

    IEnumerator StartleFlashRoutine(float flashDuration, float recoverDuration)
    {
        CamoState before = CaptureCurrentState();

        // Flash: max everything
        CamoState flash = before;
        flash.redExpansion = 1f;
        flash.yellowExpansion = 0.8f;
        flash.brownExpansion = 0.9f;
        flash.blackExpansion = 1f;
        flash.waveAmplitude = 1f;
        flash.iridophoreStrength = 0.8f;

        ApplyState(flash);
        yield return new WaitForSeconds(flashDuration);

        // Recover smoothly
        float elapsed = 0f;
        while (elapsed < recoverDuration)
        {
            elapsed += Time.deltaTime;
            float t = transitionCurve.Evaluate(Mathf.Clamp01(elapsed / recoverDuration));
            ApplyState(CamoState.Lerp(flash, before, t));
            yield return null;
        }
        ApplyState(before);
    }

    IEnumerator AnimateCamoBlend(Color targetColor, float targetBlend, float duration)
    {
        if (_mat == null) yield break;

        float startBlend = _mat.GetFloat(ID_CamoBlend);
        Color startColor = _mat.GetColor(ID_TargetColor);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = transitionCurve.Evaluate(Mathf.Clamp01(elapsed / duration));
            _mat.SetFloat(ID_CamoBlend, Mathf.Lerp(startBlend, targetBlend, t));
            _mat.SetColor(ID_TargetColor, Color.Lerp(startColor, targetColor, t));
            yield return null;
        }

        _mat.SetFloat(ID_CamoBlend, targetBlend);
        _mat.SetColor(ID_TargetColor, targetColor);
    }
}