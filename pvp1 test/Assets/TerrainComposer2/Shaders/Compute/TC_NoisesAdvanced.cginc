//	Code repository for GPU noise development blog
//	http://briansharpe.wordpress.com
//	https://github.com/BrianSharpe
//
//	I'm not one for copyrights.  Use the code however you wish.
//	All I ask is that credit be given back to the blog or myself when appropriate.
//	And also to let me know if you come up with any changes, improvements, thoughts or interesting uses for this stuff. :)
//	Thanks!
//
//	Brian Sharpe
//	brisharpe CIRCLE_A yahoo DOT com
//	http://briansharpe.wordpress.com
//	https://github.com/BrianSharpe
//
//===============================================================================
//  Scape Software License
//===============================================================================
//
//Copyright (c) 2007-2012, Giliam de Carpentier
//All rights reserved.
//
//Redistribution and use in source and binary forms, with or without
//modification, are permitted provided that the following conditions are met: 
//
//1. Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer. 
//2. Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNERS OR CONTRIBUTORS BE LIABLE 
//FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
//DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
//CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.;


//
//	PerlinSurflet3D_Deriv
//	Perlin Surflet 3D noise with derivatives
//	returns float4( value, xderiv, yderiv, zderiv )
//
#include "TC_Noise2.cginc"   

// Perlin
// =================================================================================================================================================================

float PerlinNormal()
{
	float frequency = _Frequency;
	float amplitude = 1;
	float sum = 0;

	for (uint i = 0; i < _Octaves; i++)
	{
		float h = 0;
		h = Perlin3D(pos * frequency);
		sum += h * amplitude;
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return (sum / 2) + 0.5;
}

float PerlinSimplex()
{
	float frequency = _Frequency;
	float amplitude = 1;
	float sum = 0;

	for (uint i = 0; i < _Octaves; i++)
	{
		float h = 0;
		h = SimplexPerlin3D(pos * frequency);
		sum += h * amplitude;
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return (sum / 3) + 0.5;
}

float PerlinValue()
{
	float frequency = _Frequency * 1.5;
	float amplitude = 1;
	float sum = 0;

	for (uint i = 0; i < _Octaves; i++)
	{
		float h = 0;
		h = Value3D(pos * frequency);
		sum += h * amplitude;
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return (sum / 2);
}



// Billow
// =================================================================================================================================================================

float BillowNormal()
{
	float frequency = _Frequency;
	float amplitude = 1;
	float sum = 0;

	for (uint i = 0; i < _Octaves; i++)
	{
		float h = 0;
		h = abs(Perlin3D(pos * frequency));  
		sum += h * amplitude;
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return sum;
}

float BillowSimplex()
{
	float frequency = _Frequency;
	float amplitude = 1;
	float sum = 0;

	for (uint i = 0; i < _Octaves; i++)
	{
		float h = 0;
		h = abs(SimplexPerlin3D(pos * frequency));
		sum += h * amplitude;
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return (sum / 2);
}

float BillowValue()
{
	float frequency = _Frequency * 2.5;
	float amplitude = 1;
	float sum = 0;

	for (uint i = 0; i < _Octaves; i++)
	{
		float h = 0;
		h = abs(Value3D(pos * frequency));
		sum += h * amplitude;
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return (sum / 2);
}



// Ridged
// =================================================================================================================================================================

float RidgedNormal()
{
	float frequency = _Frequency;
	float amplitude = 1;
	float sum = 0;

	for (uint i = 0; i < _Octaves; i++)
	{
		float h = 0;
		h = 0.5 * (0 - abs(4 * Perlin3D(pos * frequency)));
		sum += h * amplitude;
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return (sum / 2) + 1;
}

float RidgedSimplex()
{
	float frequency = _Frequency;
	float amplitude = 1;
	float sum = 0;

	for (uint i = 0; i < _Octaves; i++)
	{
		float h = 0;
		h = 0.5 * (0 - abs(4 * SimplexPerlin3D(pos * frequency)));
		sum += h * amplitude;
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return (sum / 3) + 1;
}

float RidgedValue()
{
	float frequency = _Frequency * 2.5;
	float amplitude = 1;
	float sum = 0;

	for (uint i = 0; i < _Octaves; i++)
	{
		float h = 0;
		h = 0.5 * (0 - abs(4 * Value3D(pos * frequency)));
		sum += h * amplitude;
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return (sum / 3.5) + 1.25;
}



// IQ
// =================================================================================================================================================================

float IQNormal()
{
	float frequency = _Frequency;
	float amplitude = 1;
	float sum = 0;
	float3 dsum = float3(0.0, 0.0, 0.0);

	for (uint i = 0; i < _Octaves; i++)
	{
		float4 n = PerlinSurflet3D_Deriv(pos * frequency);
		dsum += n.yzw;
		sum += amplitude * n.x / (1 + dot(dsum, dsum));
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return (sum / 1) + 0.75;
}

float IQSimplex()
{
	float frequency = _Frequency;
	float amplitude = 1;
	float sum = 0;
	float3 dsum = float3(0.0, 0.0, 0.0);

	for (uint i = 0; i < _Octaves; i++)
	{
		float4 n = SimplexPerlin3D_Deriv(pos * frequency);
		dsum += n.yzw;
		sum += amplitude * n.x / (1 + dot(dsum, dsum));
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return (sum / 1) + 0.75;
}

float IQValue()
{
	float frequency = _Frequency * 1.5;
	float amplitude = 1;
	float sum = 0;
	float3 dsum = float3(0.0, 0.0, 0.0);

	for (uint i = 0; i < _Octaves; i++)
	{
		float4 n = Value3D_Deriv(pos * frequency);
		dsum += n.yzw;
		sum += amplitude * n.x / (1 + dot(dsum, dsum));
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return sum;
}



// Swiss
// =================================================================================================================================================================

float SwissNormal()
{
	float sum = 0.0;
	float3 dsum = float3(0.0, 0.0, 0.0);
	float frequency = _Frequency;
	float amplitude = _Amplitude;
	
	// _Warp0 = rigdeOffset
	// _Warp = rigdeSize

	for (uint i = 0; i < _Octaves; i++)
	{
		float4 n = 0.5 * (0 + (_Warp0 - abs(PerlinSurflet3D_Deriv((pos + _Warp * dsum) * frequency))));
		sum += amplitude * n.x;
		dsum += amplitude * n.yzw * -n.x;
		frequency *= _Lacunarity;
		amplitude *= _Persistence * saturate(sum);
	}
	return sum;
}

float SwissSimplex()
{
	float sum = 0.0;
	float frequency = _Frequency;
	float amplitude = _Amplitude;
	float3 dsum = float3(0.0, 0.0, 0.0);
	
	for (uint i = 0; i < _Octaves; i++)
	{
		float4 n = 0.5 * (0 + (_Warp0 - abs(SimplexPerlin3D_Deriv((pos + _Warp * dsum) * frequency))));
		sum += amplitude * n.x;
		dsum += amplitude * n.yzw * -n.x;
		frequency *= _Lacunarity;
		amplitude *= _Persistence * saturate(sum);
	}
	return sum;
}

float SwissValue()
{
	float sum = 0.0;
	float frequency = _Frequency;
	float amplitude = _Amplitude;
	float3 dsum = float3(0.0, 0.0, 0.0);
	
	for (uint i = 0; i < _Octaves; i++)
	{
		float4 n = 0.5 * (0 + (_Warp0 - abs(Value3D_Deriv((pos + _Warp * dsum) * frequency))));
		sum += amplitude * n.x;
		dsum += amplitude * n.yzw * -n.x;
		frequency *= _Lacunarity;
		amplitude *= _Persistence * saturate(sum);
	}
	return sum;
}



// Jordan
// =================================================================================================================================================================

float JordanNormal()
{
	float frequency = _Frequency;
	float amplitude = _Amplitude;
	float4 n = PerlinSurflet3D_Deriv(pos * frequency);
	float4 n2 = n * n.x;
	float sum = n2.x;
	float3 dsum_warp = _Warp0 * n2.yzw;
	float3 dsum_damp = _Damp0 * n2.yzw;
	float damped_amp = amplitude * _Persistence;

	for (uint i = 1; i < _Octaves; i++)
	{
		n = PerlinSurflet3D_Deriv((pos * frequency) + dsum_warp.xyz);
		n2 = n * n.x;
		sum += damped_amp * n2.x;
		dsum_warp += _Warp * n2.yzw;
		dsum_damp += _Damp * n2.yzw;
		frequency *= _Lacunarity;
		amplitude *= _Persistence * saturate(sum);
		damped_amp = amplitude * (1 - _DampScale / (1 + dot(dsum_damp, dsum_damp)));
	}
	return sum;
}

float JordanSimplex()
{
	float frequency = _Frequency;
	float amplitude = _Amplitude;
	float4 n = SimplexPerlin3D_Deriv(pos * frequency);
	float4 n2 = n * n.x;
	float sum = n2.x;
	float3 dsum_warp = _Warp0 * n2.yzw;
	float3 dsum_damp = _Damp0 * n2.yzw;
	float damped_amp = amplitude * _Persistence;

	for (uint i = 1; i < _Octaves; i++)
	{
		n = SimplexPerlin3D_Deriv((pos * frequency) + dsum_warp.xyz);
		n2 = n * n.x;
		sum += damped_amp * n2.x;
		dsum_warp += _Warp * n2.yzw;
		dsum_damp += _Damp * n2.yzw;
		frequency *= _Lacunarity;
		amplitude *= _Persistence * saturate(sum);
		damped_amp = amplitude * (1 - _DampScale / (1 + dot(dsum_damp, dsum_damp)));
	}
	return sum;
}

float JordanValue()
{
	float frequency = _Frequency;
	float amplitude = _Amplitude;
	float4 n = Value3D_Deriv(pos * frequency);
	float4 n2 = n * n.x;
	float sum = n2.x;
	float3 dsum_warp = _Warp0 * n2.yzw;
	float3 dsum_damp = _Damp0 * n2.yzw;
	float damped_amp = amplitude * _Persistence;

	for (uint i = 0; i < _Octaves; i++)
	{
		n = Value3D_Deriv((pos * frequency) + dsum_warp.xyz);
		n2 = n * n.x;
		sum += damped_amp * n2.x;
		dsum_warp += _Warp * n2.yzw;
		dsum_damp += _Damp * n2.yzw;
		frequency *= _Lacunarity;
		amplitude *= _Persistence * saturate(sum);
		damped_amp = amplitude * (1 - _DampScale / (1 + dot(dsum_damp, dsum_damp)));
	}
	return sum;
}

float3 perlinNoiseDeriv(float2 p, float seed)
{
	// Calculate 2D integer coordinates i and fraction p.
	float2 i = floor(p);
	float2 f = p - i;

	// Get weights from the coordinate fraction
	float2 w = f * f * f * (f * (f * 6 - 15) + 10); // 6f^5 - 15f^4 + 10f^3
	float4 w4 = float4(1, w.x, w.y, w.x * w.y);

	// Get the derivative dw/df
	float2 dw = f * f * (f * (f * 30 - 60) + 30); // 30f^4 - 60f^3 + 30f^2

												  // Get the derivative d(w*f)/df
	float2 dwp = f * f * f * (f * (f * 36 - 75) + 40); // 36f^5 - 75f^4 + 40f^3

													   // Get the four randomly permutated indices from the noise lattice nearest to
													   // p and offset these numbers with the seed number.
	float4 perm = _PermTable2D.SampleLevel(_LinearRepeat, i / 256, 0) + seed;

	// Permutate the four offseted indices again and get the 2D gradient for each
	// of the four permutated coordinates-seed pairs.
	float4 g1 = _Gradient3D.SampleLevel(_LinearRepeat, perm.xy, 0) * 2 - 1;
	float4 g2 = _Gradient3D.SampleLevel(_LinearRepeat, perm.zw, 0) * 2 - 1;

	// Evaluate the four lattice gradients at p
	float a = dot(g1.xy, f);
	float b = dot(g2.xy, f + float2(-1, 0));
	float c = dot(g1.zw, f + float2(0, -1));
	float d = dot(g2.zw, f + float2(-1, -1));

	// Bi-linearly blend between the gradients, using w4 as blend factors.
	float4 grads = float4(a, b - a, c - a, a - b - c + d);
	float n = dot(grads, w4);

	// Calculate the derivatives dn/dx and dn/dy
	float dx = (g1.x + (g1.z - g1.x)*w.y) + ((g2.y - g1.y)*f.y - g2.x +
		((g1.y - g2.y - g1.w + g2.w)*f.y + g2.x + g1.w - g2.z - g2.w)*w.y)*
		dw.x + ((g2.x - g1.x) + (g1.x - g2.x - g1.z + g2.z)*w.y)*dwp.x;
	float dy = (g1.y + (g2.y - g1.y)*w.x) + ((g1.z - g1.x)*f.x - g1.w + ((g1.x -
		g2.x - g1.z + g2.z)*f.x + g2.x + g1.w - g2.z - g2.w)*w.x)*dw.y +
		((g1.w - g1.y) + (g1.y - g2.y - g1.w + g2.w)*w.x)*dwp.y;

	// Return the noise value, roughly normalized in the range [-1, 1]
	// Also return the pseudo dn/dx and dn/dy, scaled by the same factor
	return float3(n, dx, dy) * 1.5;
}

float jordanTurbulence(float2 p, float seed, int octaves, float lacunarity = 2.0, float freq = 1, float gain1 = 0.8, float gain = 0.5, float warp0 = 0.4, float warp = 0.35, float damp0 = 1.0, float damp = 0.8, float damp_scale = 1.0)
{
	float3 n = perlinNoiseDeriv(p, seed);
	float3 n2 = n * n.x;
	float sum = n2.x;
	float2 dsum_warp = warp0*n2.yz;
	float2 dsum_damp = damp0*n2.yz;

	float amp = gain1;
	freq = lacunarity;
	float damped_amp = amp * gain;

	for (int i = 1; i < octaves; i++)
	{
		n = perlinNoiseDeriv(p * freq + dsum_warp.xy, seed + i / 256.0);
		n2 = n * n.x;
		sum += damped_amp * n2.x;
		dsum_warp += warp * n2.yz;
		dsum_damp += damp * n2.yz;
		freq *= lacunarity;
		amp *= gain;
		damped_amp = amp * (1 - damp_scale / (1 + dot(dsum_damp, dsum_damp)));
	}
	return sum;
}




// Cell
// =================================================================================================================================================================

float CellNormal()
{
	float frequency = _Frequency;
	float amplitude = 1;
	float sum = 0;

	for (uint i = 0; i < _Octaves; i++)
	{
		float h = 0;
		h = Cellular3D(pos * frequency, _CellType, _DistanceFunction);
		sum += h * amplitude;
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return sum;
}

float CellFast()
{
	float frequency = _Frequency;
	float amplitude = 1;
	float sum = 0;
	
	for (uint i = 0; i < _Octaves; i++)
	{
		float h = 0;
		h = Cellular3D(pos * frequency);
		sum += h * amplitude;
		frequency *= _Lacunarity;
		amplitude *= _Persistence;
	}
	return sum;
}
