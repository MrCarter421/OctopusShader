Shader "Custom/OctopusCamouflage"
{
    Properties
    {
        [Header(BASE MESH)]
        _MainTex ("Base Texture (RGB)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 2)) = 1.0

        [Header(LEUCOPHORE LAYER  Bottom  Ambient Reflection)]
        _LeucophoreColor ("Leucophore Base Color", Color) = (0.85, 0.82, 0.78, 1)
        _LeucophoreAmbientBlend ("Ambient Light Blend", Range(0, 1)) = 0.5
        _LeucophoreIntensity ("Leucophore Intensity", Range(0, 2)) = 1.0

        [Header(IRIDOPHORE LAYER  Middle  Iridescence)]
        _IridophoreStrength ("Iridescence Strength", Range(0, 1)) = 0.3
        _IridophoreScale ("Iridescence Scale", Range(0.1, 20)) = 5.0
        _IridophoreSpeed ("Iridescence Shimmer Speed", Range(0, 5)) = 1.0
        _IridophoreHueShift ("Iridescence Hue Offset", Range(0, 1)) = 0.0
        _IridophoreHueRange ("Iridescence Hue Range", Range(0, 1)) = 0.3
        _IridophoreFresnelPower ("Fresnel Power (view angle)", Range(0.5, 8)) = 3.0

        [Header(LAYER BLENDING)]
        _BaseTextureStrength ("Base Texture Visibility", Range(0, 1)) = 1.0
        _LeucophoreOpacity ("Leucophore Layer Opacity", Range(0, 1)) = 0.5
        _IridophoreOpacity ("Iridophore Layer Opacity", Range(0, 1)) = 1.0
        _ChromatophoreOpacity ("Chromatophore Layer Opacity", Range(0, 1)) = 0.7
        _CamoTransitionOpacity ("Camo Transition Opacity", Range(0, 1)) = 1.0
        [KeywordEnum(Replace, Multiply, Overlay)] _BlendMode ("Chromatophore Blend Mode", Float) = 0

        [Header(CHROMATOPHORE LAYER  Top  Pigment Cells)]
        [Space(5)]
        _ChromatophoreScale ("Cell Scale", Range(1, 100)) = 30.0
        _ChromatophoreEdge ("Cell Edge Sharpness", Range(0.01, 0.5)) = 0.1

        [Header(Pigment Channel  RED)]
        _RedPigment ("Red Pigment Color", Color) = (0.7, 0.12, 0.08, 1)
        _RedExpansion ("Red Expansion", Range(0, 1)) = 0.5
        _RedWaveInfluence ("Red Wave Influence", Range(0, 1)) = 0.8

        [Header(Pigment Channel  YELLOW)]
        _YellowPigment ("Yellow Pigment Color", Color) = (0.85, 0.75, 0.15, 1)
        _YellowExpansion ("Yellow Expansion", Range(0, 1)) = 0.3
        _YellowWaveInfluence ("Yellow Wave Influence", Range(0, 1)) = 0.6

        [Header(Pigment Channel  BROWN)]
        _BrownPigment ("Brown Pigment Color", Color) = (0.4, 0.25, 0.1, 1)
        _BrownExpansion ("Brown Expansion", Range(0, 1)) = 0.6
        _BrownWaveInfluence ("Brown Wave Influence", Range(0, 1)) = 0.7

        [Header(Pigment Channel  BLACK)]
        _BlackPigment ("Black Pigment Color", Color) = (0.05, 0.03, 0.02, 1)
        _BlackExpansion ("Black Expansion", Range(0, 1)) = 0.2
        _BlackWaveInfluence ("Black Wave Influence", Range(0, 1)) = 0.9

        [Header(WAVE PROPAGATION  Neural Cascade)]
        _WaveOrigin ("Wave Origin (UV space)", Vector) = (0.5, 0.5, 0, 0)
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 2.0
        _WaveFrequency ("Wave Frequency", Range(0.1, 10)) = 3.0
        _WaveAmplitude ("Wave Amplitude", Range(0, 1)) = 0.5
        _WaveDecay ("Wave Decay (falloff)", Range(0, 5)) = 1.0
        [Toggle] _WaveRadial ("Radial Wave (vs Directional)", Float) = 1
        _WaveDirection ("Wave Direction (if directional)", Vector) = (1, 0, 0, 0)
        [Toggle] _MultiWave ("Enable Secondary Wave", Float) = 0
        _WaveOrigin2 ("Secondary Wave Origin", Vector) = (0.8, 0.3, 0, 0)
        _WaveSpeed2 ("Secondary Wave Speed", Range(0, 10)) = 1.5
        _WaveFrequency2 ("Secondary Wave Frequency", Range(0.1, 10)) = 2.0

        [Header(TRANSITION SYSTEM  Camouflage Shift)]
        _TargetColor ("Target Camouflage Color", Color) = (0.4, 0.45, 0.35, 1)
        _CamoBlend ("Camouflage Blend", Range(0, 1)) = 0.0
        _CamoNoiseTex ("Camo Pattern Noise", 2D) = "gray" {}
        _CamoNoiseScale ("Camo Pattern Scale", Range(0.1, 20)) = 3.0
        _CamoNoiseContrast ("Camo Pattern Contrast", Range(0, 3)) = 1.5
        _TransitionSharpness ("Transition Edge Sharpness", Range(0.01, 1)) = 0.3

        [Header(PAPILLAE  Texture Bumps)]
        _PapillaeStrength ("Papillae Bump Strength", Range(0, 2)) = 0.0
        _PapillaeScale ("Papillae Scale", Range(1, 50)) = 15.0
        _PapillaeSharpness ("Papillae Sharpness", Range(0.1, 5)) = 2.0

        [Header(ANIMATION)]
        _GlobalSpeed ("Global Animation Speed", Range(0, 5)) = 1.0
        _PulseSpeed ("Chromatophore Pulse Speed", Range(0, 10)) = 1.5
        _PulseAmplitude ("Chromatophore Pulse Amount", Range(0, 0.3)) = 0.05
        _BreathingSpeed ("Breathing Oscillation Speed", Range(0, 5)) = 0.5
        _BreathingAmount ("Breathing Oscillation Amount", Range(0, 0.2)) = 0.03

        [Header(RENDERING)]
        _Glossiness ("Smoothness", Range(0, 1)) = 0.6
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        _OcclusionStrength ("Occlusion Strength", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.5
        #pragma multi_compile _BLENDMODE_REPLACE _BLENDMODE_MULTIPLY _BLENDMODE_OVERLAY

        // ─────────────────────────────────────────────
        // TEXTURES
        // ─────────────────────────────────────────────
        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _CamoNoiseTex;

        // ─────────────────────────────────────────────
        // PROPERTIES
        // ─────────────────────────────────────────────
        float _NormalStrength;

        // Layer Blending
        float _BaseTextureStrength;
        float _LeucophoreOpacity;
        float _IridophoreOpacity;
        float _ChromatophoreOpacity;
        float _CamoTransitionOpacity;
        float _BlendMode;

        // Leucophore
        fixed4 _LeucophoreColor;
        float _LeucophoreAmbientBlend;
        float _LeucophoreIntensity;

        // Iridophore
        float _IridophoreStrength;
        float _IridophoreScale;
        float _IridophoreSpeed;
        float _IridophoreHueShift;
        float _IridophoreHueRange;
        float _IridophoreFresnelPower;

        // Chromatophore
        float _ChromatophoreScale;
        float _ChromatophoreEdge;

        fixed4 _RedPigment;
        float _RedExpansion;
        float _RedWaveInfluence;

        fixed4 _YellowPigment;
        float _YellowExpansion;
        float _YellowWaveInfluence;

        fixed4 _BrownPigment;
        float _BrownExpansion;
        float _BrownWaveInfluence;

        fixed4 _BlackPigment;
        float _BlackExpansion;
        float _BlackWaveInfluence;

        // Wave
        float4 _WaveOrigin;
        float _WaveSpeed;
        float _WaveFrequency;
        float _WaveAmplitude;
        float _WaveDecay;
        float _WaveRadial;
        float4 _WaveDirection;
        float _MultiWave;
        float4 _WaveOrigin2;
        float _WaveSpeed2;
        float _WaveFrequency2;

        // Transition
        fixed4 _TargetColor;
        float _CamoBlend;
        float _CamoNoiseScale;
        float _CamoNoiseContrast;
        float _TransitionSharpness;

        // Papillae
        float _PapillaeStrength;
        float _PapillaeScale;
        float _PapillaeSharpness;

        // Animation
        float _GlobalSpeed;
        float _PulseSpeed;
        float _PulseAmplitude;
        float _BreathingSpeed;
        float _BreathingAmount;

        // Rendering
        float _Glossiness;
        float _Metallic;
        float _OcclusionStrength;

        // ─────────────────────────────────────────────
        // STRUCTS
        // ─────────────────────────────────────────────
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float3 viewDir;
            float3 worldPos;
            float3 worldNormal;
            INTERNAL_DATA
        };

        // ─────────────────────────────────────────────
        // NOISE FUNCTIONS
        // ─────────────────────────────────────────────

        // Hash for Voronoi
        float2 hash2(float2 p)
        {
            p = float2(dot(p, float2(127.1, 311.7)),
                        dot(p, float2(269.5, 183.3)));
            return frac(sin(p) * 43758.5453);
        }

        // Voronoi - returns (distance to nearest, distance to second nearest)
        // This models the chromatophore cell pattern
        float2 voronoi(float2 p)
        {
            float2 ip = floor(p);
            float2 fp = frac(p);

            float d1 = 8.0; // nearest
            float d2 = 8.0; // second nearest

            for (int j = -1; j <= 1; j++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    float2 g = float2(i, j);
                    float2 o = hash2(ip + g);
                    float2 r = g + o - fp;
                    float d = dot(r, r);

                    if (d < d1)
                    {
                        d2 = d1;
                        d1 = d;
                    }
                    else if (d < d2)
                    {
                        d2 = d;
                    }
                }
            }
            return float2(sqrt(d1), sqrt(d2));
        }

        // Voronoi that also returns the cell ID for per-cell variation
        float3 voronoiWithID(float2 p)
        {
            float2 ip = floor(p);
            float2 fp = frac(p);

            float d1 = 8.0;
            float2 nearestCell = float2(0, 0);

            for (int j = -1; j <= 1; j++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    float2 g = float2(i, j);
                    float2 o = hash2(ip + g);
                    float2 r = g + o - fp;
                    float d = dot(r, r);

                    if (d < d1)
                    {
                        d1 = d;
                        nearestCell = ip + g;
                    }
                }
            }
            // z = cell hash for per-cell randomness
            float cellHash = frac(sin(dot(nearestCell, float2(12.9898, 78.233))) * 43758.5453);
            return float3(sqrt(d1), cellHash, 0);
        }

        // Simple 2D value noise for papillae and camo variation
        float valueNoise(float2 p)
        {
            float2 ip = floor(p);
            float2 fp = frac(p);
            fp = fp * fp * (3.0 - 2.0 * fp); // smoothstep

            float a = frac(sin(dot(ip, float2(127.1, 311.7))) * 43758.5453);
            float b = frac(sin(dot(ip + float2(1, 0), float2(127.1, 311.7))) * 43758.5453);
            float c = frac(sin(dot(ip + float2(0, 1), float2(127.1, 311.7))) * 43758.5453);
            float d = frac(sin(dot(ip + float2(1, 1), float2(127.1, 311.7))) * 43758.5453);

            return lerp(lerp(a, b, fp.x), lerp(c, d, fp.x), fp.y);
        }

        // FBM (fractal Brownian motion) for organic variation
        float fbm(float2 p, int octaves)
        {
            float value = 0.0;
            float amplitude = 0.5;
            float frequency = 1.0;
            for (int i = 0; i < octaves; i++)
            {
                value += amplitude * valueNoise(p * frequency);
                frequency *= 2.0;
                amplitude *= 0.5;
            }
            return value;
        }

        // ─────────────────────────────────────────────
        // HSV UTILITIES
        // ─────────────────────────────────────────────
        float3 hsv2rgb(float3 c)
        {
            float3 p = abs(frac(c.xxx + float3(1.0, 2.0/3.0, 1.0/3.0)) * 6.0 - 3.0);
            return c.z * lerp(float3(1,1,1), saturate(p - 1.0), c.y);
        }

        // ─────────────────────────────────────────────
        // WAVE FUNCTION
        // Simulates neural cascade signal propagation
        // ─────────────────────────────────────────────
        float computeWave(float2 uv, float time)
        {
            float wave = 0.0;

            // Primary wave
            if (_WaveRadial > 0.5)
            {
                float dist = distance(uv, _WaveOrigin.xy);
                wave = sin(dist * _WaveFrequency * 6.2832 - time * _WaveSpeed) * 0.5 + 0.5;
                wave *= exp(-dist * _WaveDecay);
            }
            else
            {
                float2 dir = normalize(_WaveDirection.xy);
                float proj = dot(uv, dir);
                wave = sin(proj * _WaveFrequency * 6.2832 - time * _WaveSpeed) * 0.5 + 0.5;
            }

            // Secondary wave (optional)
            if (_MultiWave > 0.5)
            {
                float dist2 = distance(uv, _WaveOrigin2.xy);
                float wave2 = sin(dist2 * _WaveFrequency2 * 6.2832 - time * _WaveSpeed2) * 0.5 + 0.5;
                wave2 *= exp(-dist2 * _WaveDecay);
                wave = saturate(wave + wave2 * 0.6);
            }

            return wave * _WaveAmplitude;
        }

        // ─────────────────────────────────────────────
        // CHROMATOPHORE EXPANSION
        // Models the elastic pigment sac expanding/contracting
        // dist     = distance from cell center (Voronoi)
        // expansion = 0-1 how expanded the sac is
        // cellHash  = per-cell random for variation
        // waveValue = neural wave influence
        // waveInfluence = how much this pigment responds to waves
        // ─────────────────────────────────────────────
        float chromatophoreCell(float dist, float expansion, float cellHash,
                                float waveValue, float waveInfluence,
                                float pulseTime, float breatheTime)
        {
            // Per-cell variation in expansion
            float cellVariation = lerp(0.7, 1.3, cellHash);

            // Neural wave modulation
            float waveModulation = lerp(1.0, 1.0 + waveValue, waveInfluence);

            // Subtle pulsing (chromatophores rhythmically pulse in real octopuses)
            float pulse = sin(pulseTime * _PulseSpeed + cellHash * 6.2832) * _PulseAmplitude;

            // Breathing oscillation
            float breathe = sin(breatheTime * _BreathingSpeed + cellHash * 3.14) * _BreathingAmount;

            // Final expansion radius
            float expandedRadius = expansion * cellVariation * waveModulation + pulse + breathe;
            expandedRadius = saturate(expandedRadius);

            // Soft-edged cell (models the elastic sac stretching)
            float cell = 1.0 - smoothstep(expandedRadius - _ChromatophoreEdge,
                                           expandedRadius + _ChromatophoreEdge, dist);
            return cell;
        }

        // ─────────────────────────────────────────────
        // VERTEX SHADER (papillae displacement)
        // ─────────────────────────────────────────────
        void vert(inout appdata_full v)
        {
            if (_PapillaeStrength > 0.001)
            {
                float2 papUV = v.texcoord.xy * _PapillaeScale;
                float bump = fbm(papUV, 3);
                bump = pow(bump, _PapillaeSharpness);
                v.vertex.xyz += v.normal * bump * _PapillaeStrength * 0.05;
            }
        }

        // ─────────────────────────────────────────────
        // SURFACE SHADER
        // ─────────────────────────────────────────────
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float time = _Time.y * _GlobalSpeed;
            float2 uv = IN.uv_MainTex;
            float4 baseTex = tex2D(_MainTex, uv);

            // ── LAYER 1: LEUCOPHORE (base reflective layer) ──
            // Leucophores reflect ambient wavelengths, providing a white/neutral backdrop
            float3 leucophore = _LeucophoreColor.rgb * _LeucophoreIntensity;

            // Blend toward a neutral ambient approximation
            // (In a real scenario you could sample a reflection probe here)
            float3 ambientApprox = float3(0.5, 0.55, 0.6); // slight blue-ish ambient
            leucophore = lerp(leucophore, leucophore * ambientApprox, _LeucophoreAmbientBlend);

            // ── LAYER 2: IRIDOPHORE (iridescent reflective plates) ──
            // Viewing-angle dependent color, like thin-film interference
            float3 viewDir = normalize(IN.viewDir);
            float3 worldNormal = WorldNormalVector(IN, float3(0, 0, 1));
            float fresnel = pow(1.0 - saturate(dot(viewDir, worldNormal)), _IridophoreFresnelPower);

            // Iridescent hue shifts with viewing angle and surface position
            float iriPhase = dot(uv * _IridophoreScale, float2(1.0, 0.618))
                           + fresnel * 2.0
                           + time * _IridophoreSpeed;
            float iriHue = frac(_IridophoreHueShift + sin(iriPhase) * _IridophoreHueRange);
            float3 iridescence = hsv2rgb(float3(iriHue, 0.7, 1.0));
            float iriMask = fresnel * _IridophoreStrength;

            // ── LAYER 3: CHROMATOPHORES (pigment cells) ──
            // Four pigment channels, each on their own Voronoi grid (offset for organic look)
            float pulseTime = time;
            float breatheTime = time;

            // Neural wave signal
            float waveValue = computeWave(uv, time);

            // Red chromatophore channel
            float2 redUV = uv * _ChromatophoreScale + float2(0.0, 0.0);
            float3 redVoronoi = voronoiWithID(redUV);
            float redCell = chromatophoreCell(redVoronoi.x, _RedExpansion, redVoronoi.y,
                                              waveValue, _RedWaveInfluence,
                                              pulseTime, breatheTime);

            // Yellow chromatophore channel (offset grid)
            float2 yellowUV = uv * _ChromatophoreScale * 0.9 + float2(3.7, 1.3);
            float3 yellowVoronoi = voronoiWithID(yellowUV);
            float yellowCell = chromatophoreCell(yellowVoronoi.x, _YellowExpansion, yellowVoronoi.y,
                                                  waveValue, _YellowWaveInfluence,
                                                  pulseTime, breatheTime);

            // Brown chromatophore channel (offset grid)
            float2 brownUV = uv * _ChromatophoreScale * 1.1 + float2(7.2, 5.9);
            float3 brownVoronoi = voronoiWithID(brownUV);
            float brownCell = chromatophoreCell(brownVoronoi.x, _BrownExpansion, brownVoronoi.y,
                                                 waveValue, _BrownWaveInfluence,
                                                 pulseTime, breatheTime);

            // Black chromatophore channel (offset grid, slightly larger cells)
            float2 blackUV = uv * _ChromatophoreScale * 0.7 + float2(11.1, 8.4);
            float3 blackVoronoi = voronoiWithID(blackUV);
            float blackCell = chromatophoreCell(blackVoronoi.x, _BlackExpansion, blackVoronoi.y,
                                                 waveValue, _BlackWaveInfluence,
                                                 pulseTime, breatheTime);

            // ── COMPOSITE ALL LAYERS ──
            // Start from the raw base texture
            float3 baseColor = baseTex.rgb * _BaseTextureStrength;

            // Leucophore layer blends onto the base (not replacing it)
            float3 color = lerp(baseColor, leucophore * baseTex.rgb, _LeucophoreOpacity);

            // Add iridophore shimmer (scaled by opacity)
            color = lerp(color, iridescence, iriMask * _IridophoreOpacity);

            // Layer chromatophores with blend mode support
            // Yellow is deepest chromatophore layer, then red, then brown, then black on top
            float chromaOp = _ChromatophoreOpacity;

            // Helper: blend a pigment cell onto the current color
            #if defined(_BLENDMODE_MULTIPLY)
                #define BLEND_PIGMENT(col, pigment, cell) lerp(col, col * pigment, cell * chromaOp)
            #elif defined(_BLENDMODE_OVERLAY)
                #define BLEND_PIGMENT(col, pigment, cell) lerp(col, lerp(2.0 * col * pigment, 1.0 - 2.0 * (1.0 - col) * (1.0 - pigment), step(0.5, col)), cell * chromaOp)
            #else
                #define BLEND_PIGMENT(col, pigment, cell) lerp(col, pigment, cell * chromaOp)
            #endif

            color = BLEND_PIGMENT(color, _YellowPigment.rgb, yellowCell * _YellowPigment.a);
            color = BLEND_PIGMENT(color, _RedPigment.rgb, redCell * _RedPigment.a);
            color = BLEND_PIGMENT(color, _BrownPigment.rgb, brownCell * _BrownPigment.a);
            color = BLEND_PIGMENT(color, _BlackPigment.rgb, blackCell * _BlackPigment.a);

            // ── CAMOUFLAGE TRANSITION ──
            // Blend toward a target color/pattern
            if (_CamoBlend > 0.001)
            {
                float camoNoise = tex2D(_CamoNoiseTex, uv * _CamoNoiseScale).r;
                camoNoise = pow(camoNoise, _CamoNoiseContrast);

                // Organic transition edge using noise
                float transitionMask = smoothstep(
                    _CamoBlend - _TransitionSharpness,
                    _CamoBlend + _TransitionSharpness,
                    camoNoise
                );

                // Target camo pattern with subtle variation
                float3 camoColor = _TargetColor.rgb * lerp(0.8, 1.2, fbm(uv * 5.0, 3));

                color = lerp(color, camoColor, (1.0 - transitionMask) * _CamoTransitionOpacity);
            }

            // ── SELF-OCCLUSION from chromatophore density ──
            float totalCoverage = saturate(redCell + yellowCell + brownCell + blackCell);
            float occlusion = lerp(1.0, 0.7, totalCoverage * _OcclusionStrength);

            // ── PAPILLAE NORMAL PERTURBATION ──
            float3 normalOut = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
            normalOut.xy *= _NormalStrength;

            if (_PapillaeStrength > 0.001)
            {
                float2 papUV = uv * _PapillaeScale;
                float papR = fbm(papUV + float2(0.01, 0), 3);
                float papL = fbm(papUV - float2(0.01, 0), 3);
                float papU = fbm(papUV + float2(0, 0.01), 3);
                float papD = fbm(papUV - float2(0, 0.01), 3);

                float3 papNormal = float3(papL - papR, papD - papU, 0.1);
                papNormal = normalize(papNormal);
                papNormal.xy *= _PapillaeStrength;

                normalOut.xy += papNormal.xy;
            }

            normalOut = normalize(normalOut);

            // ── OUTPUT ──
            o.Albedo = color * occlusion;
            o.Normal = normalOut;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1.0;
        }
        ENDCG
    }

    FallBack "Standard"
    CustomEditor "OctopusCamouflageInspector"
}
