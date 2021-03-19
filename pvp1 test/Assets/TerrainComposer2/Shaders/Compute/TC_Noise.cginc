Texture2D<float4> _PermTable2D, _Gradient3D;


float3 fade(float3 t)
{
	return t * t * t * (t * (t * 6 - 15) + 10); 
}

float4 perm2d(float2 uv) 
{
	return _PermTable2D.SampleLevel(_LinearRepeat, uv, 0);
}

float gradperm(float x, float3 p)
{
	float3 g = _Gradient3D.SampleLevel(_LinearRepeat, float2(x, 0), 0).rgb *2.0 - 1.0;
	return dot(g, p);
}

float inoise(float3 p)
{
	float3 P = fmod(floor(p), 256.0);	
	p -= floor(p);                      
	float3 f = fade(p);                 

	P = P / 256.0;
	const float one = 1.0 / 256.0;

	float4 AA = perm2d(P.xy) + P.z;

	return lerp(lerp(lerp(gradperm(AA.x, p),
		gradperm(AA.z, p + float3(-1, 0, 0)), f.x),
		lerp(gradperm(AA.y, p + float3(0, -1, 0)),
			gradperm(AA.w, p + float3(-1, -1, 0)), f.x), f.y),

		lerp(lerp(gradperm(AA.x + one, p + float3(0, 0, -1)),
			gradperm(AA.z + one, p + float3(-1, 0, -1)), f.x),
			lerp(gradperm(AA.y + one, p + float3(0, -1, -1)),
				gradperm(AA.w + one, p + float3(-1, -1, -1)), f.x), f.y), f.z);
}

// fractal sum, range -1.0 / 1.0
float fBm(float3 p, int octaves)
{
	float amp = 0.5;
	float sum = 0;
	p.xz -= 500;
	
	for (int i = 0; i < octaves; i++)
	{
		sum += inoise(p * _Frequency) * amp;
		_Frequency *= _Lacunarity;
		amp *= _Persistence;
	}
	return sum + 0.5; // remap
}

float fBm(float3 p)
{
	float amp = 0.5;
	float sum = 0;
	p.xz -= 500;

	sum += inoise(p * _Frequency) * amp;
	_Frequency *= _Lacunarity;
	amp *= _Persistence;
	 
	return sum + 0.5; // remap
}


// fractal abs sum, range 0.0 / 1.0
float turbulence(float3 p, int octaves)
{
	float sum = 0;
	float amp = 1.0;
	p.xz -= 500;
	
	for (int i = 0; i < octaves; i++)
	{
		sum += abs(inoise(p*_Frequency))*amp;
		_Frequency *= _Lacunarity;
		amp *= _Persistence;
	}
	return sum;
}

float turbulence(float3 p)
{
	float sum = 0;
	float amp = 1.0;
	p.xz -= 500;

	sum += abs(inoise(p*_Frequency))*amp;
	_Frequency *= _Lacunarity;
	amp *= _Persistence;

	return sum;
}

// Ridged multifractal, range 0.0 / 1.0
float ridge(float h, float offset)
{
	h = abs(h);
	h = offset - h;
	h = h * h;
	return h;
}

float ridgedmf(float3 p, int octaves, float offset)
{
	float sum = 0;
	float amp = 0.5;
	float prev = 1.0;
	p.xz -= 500;
	
	for (int i = 0; i < octaves; i++)
	{
		float n = ridge(inoise(p * _Frequency), offset);
		sum += n*amp*prev;
		prev = n;
		_Frequency *= _Lacunarity;
		amp *= _Persistence;
	}
	return sum;
}

float ridgedmf(float3 p, float offset)
{
	float sum = 0;
	float amp = 0.5;
	float prev = 1.0;
	p.xz -= 500;
	
	float n = ridge(inoise(p * _Frequency), offset);
	sum += n*amp*prev;
	prev = n;
	_Frequency *= _Lacunarity;
	amp *= _Persistence;

	return sum;
}