//
//	FAST32_hash
//	A very fast hashing function.  Requires 32bit support.
//	http://briansharpe.wordpress.com/2011/11/15/a-fast-and-simple-32bit-floating-point-hash-function/
//
//	The hash formula takes the form....
//	hash = mod( coord.x * coord.x * coord.y * coord.y, SOMELARGEFLOAT ) / SOMELARGEFLOAT
//	We truncate and offset the domain to the most interesting part of the noise.
//	SOMELARGEFLOAT should be in the range of 400.0->1000.0 and needs to be hand picked.  Only some give good results.
//	3D Noise is achieved by offsetting the SOMELARGEFLOAT value by the Z coordinate
//
//	generates 3 random numbers for each of the 8 cell corners

// float h = PerlinDerivedJordan(OUT.pos, _Octaves, _Offset, _Frequency, _Amplitude, _Lacunarity, _Persistence, _Warp0, _Warp, _Damp0, _Damp, _DampScale);

void FAST32_hash_3D(float3 gridcell, out float4 lowz_hash_0, out float4 lowz_hash_1, out float4 lowz_hash_2, out float4 highz_hash_0, out float4 highz_hash_1, out float4 highz_hash_2)
{
	//    gridcell is assumed to be an integer coordinate

	//	TODO: 	these constants need tweaked to find the best possible noise.
	//			probably requires some kind of brute force computational searching or something....
	const float2 OFFSET = float2(50.0, 161.0);
	const float DOMAIN = 69.0;
	const float3 SOMELARGEFLOATS = float3(635.298681, 682.357502, 668.926525);
	const float3 ZINC = float3(48.500388, 65.294118, 63.934599);

	//	truncate the domain
	gridcell.xyz = gridcell.xyz - floor(gridcell.xyz * (1.0 / DOMAIN)) * DOMAIN;
	float3 gridcell_inc1 = step(gridcell, float3(DOMAIN - 1.5, DOMAIN - 1.5, DOMAIN - 1.5)) * (gridcell + 1.0);

	//	calculate the noise
	float4 P = float4(gridcell.xy, gridcell_inc1.xy) + OFFSET.xyxy;
	P *= P;
	P = P.xzxz * P.yyww;
	float3 lowz_mod = float3(1.0 / (SOMELARGEFLOATS.xyz + gridcell.zzz * ZINC.xyz));
	float3 highz_mod = float3(1.0 / (SOMELARGEFLOATS.xyz + gridcell_inc1.zzz * ZINC.xyz));
	lowz_hash_0 = frac(P * lowz_mod.xxxx);
	highz_hash_0 = frac(P * highz_mod.xxxx);
	lowz_hash_1 = frac(P * lowz_mod.yyyy);
	highz_hash_1 = frac(P * highz_mod.yyyy);
	lowz_hash_2 = frac(P * lowz_mod.zzzz);
	highz_hash_2 = frac(P * highz_mod.zzzz);
}

float4 PerlinSurflet3D_Deriv(float3 P)
{
	//	establish our grid cell and unit position
	float3 Pi = floor(P);
	float3 Pf = P - Pi;
	float3 Pf_min1 = Pf - 1.0;

	//	calculate the hash.
	//	( various hashing methods listed in order of speed )
	float4 hashx0, hashy0, hashz0, hashx1, hashy1, hashz1;
	FAST32_hash_3D(Pi, hashx0, hashy0, hashz0, hashx1, hashy1, hashz1);

	//	calculate the gradients
	float4 grad_x0 = hashx0 - 0.49999;
	float4 grad_y0 = hashy0 - 0.49999;
	float4 grad_z0 = hashz0 - 0.49999;
	float4 norm_0 = rsqrt(grad_x0 * grad_x0 + grad_y0 * grad_y0 + grad_z0 * grad_z0);
	grad_x0 *= norm_0;
	grad_y0 *= norm_0;
	grad_z0 *= norm_0;
	float4 grad_x1 = hashx1 - 0.49999;
	float4 grad_y1 = hashy1 - 0.49999;
	float4 grad_z1 = hashz1 - 0.49999;
	float4 norm_1 = rsqrt(grad_x1 * grad_x1 + grad_y1 * grad_y1 + grad_z1 * grad_z1);
	grad_x1 *= norm_1;
	grad_y1 *= norm_1;
	grad_z1 *= norm_1;
	float4 grad_results_0 = float2(Pf.x, Pf_min1.x).xyxy * grad_x0 + float2(Pf.y, Pf_min1.y).xxyy * grad_y0 + Pf.zzzz * grad_z0;
	float4 grad_results_1 = float2(Pf.x, Pf_min1.x).xyxy * grad_x1 + float2(Pf.y, Pf_min1.y).xxyy * grad_y1 + Pf_min1.zzzz * grad_z1;

	//	get lengths in the x+y plane
	float3 Pf_sq = Pf*Pf;
	float3 Pf_min1_sq = Pf_min1*Pf_min1;
	float4 vecs_len_sq = float2(Pf_sq.x, Pf_min1_sq.x).xyxy + float2(Pf_sq.y, Pf_min1_sq.y).xxyy;

	//	evaluate the surflet
	float4 m_0 = vecs_len_sq + Pf_sq.zzzz;
	m_0 = max(1.0 - m_0, 0.0);
	float4 m2_0 = m_0*m_0;
	float4 m3_0 = m_0*m2_0;

	float4 m_1 = vecs_len_sq + Pf_min1_sq.zzzz;
	m_1 = max(1.0 - m_1, 0.0);
	float4 m2_1 = m_1*m_1;
	float4 m3_1 = m_1*m2_1;

	//	calc the deriv
	float4 temp_0 = -6.0 * m2_0 * grad_results_0;
	float xderiv_0 = dot(temp_0, float2(Pf.x, Pf_min1.x).xyxy) + dot(m3_0, grad_x0);
	float yderiv_0 = dot(temp_0, float2(Pf.y, Pf_min1.y).xxyy) + dot(m3_0, grad_y0);
	float zderiv_0 = dot(temp_0, Pf.zzzz) + dot(m3_0, grad_z0);

	float4 temp_1 = -6.0 * m2_1 * grad_results_1;
	float xderiv_1 = dot(temp_1, float2(Pf.x, Pf_min1.x).xyxy) + dot(m3_1, grad_x1);
	float yderiv_1 = dot(temp_1, float2(Pf.y, Pf_min1.y).xxyy) + dot(m3_1, grad_y1);
	float zderiv_1 = dot(temp_1, Pf_min1.zzzz) + dot(m3_1, grad_z1);

	const float FINAL_NORMALIZATION = 2.3703703703703703703703703703704;	//	scales the final result to a strict 1.0->-1.0 range
	return float4(dot(m3_0, grad_results_0) + dot(m3_1, grad_results_1), float3(xderiv_0, yderiv_0, zderiv_0) + float3(xderiv_1, yderiv_1, zderiv_1)) * FINAL_NORMALIZATION;
}

void FAST32_hash_3D(float3 gridcell, out float4 lowz_hash, out float4 highz_hash)	//	generates a random number for each of the 8 cell corners
{
	//    gridcell is assumed to be an integer coordinate

	//	TODO: 	these constants need tweaked to find the best possible noise.
	//			probably requires some kind of brute force computational searching or something....
	const float2 OFFSET = float2(50.0, 161.0);
	const float DOMAIN = 69.0;
	const float SOMELARGEFLOAT = 635.298681;
	const float ZINC = 48.500388;

	//	truncate the domain
	gridcell.xyz = gridcell.xyz - floor(gridcell.xyz * (1.0 / DOMAIN)) * DOMAIN;
	float3 gridcell_inc1 = step(gridcell, float3(DOMAIN - 1.5, DOMAIN - 1.5, DOMAIN - 1.5)) * (gridcell + 1.0);

	//	calculate the noise
	float4 P = float4(gridcell.xy, gridcell_inc1.xy) + OFFSET.xyxy;
	P *= P;
	P = P.xzxz * P.yyww;
	highz_hash.xy = float2(1.0 / (SOMELARGEFLOAT + float2(gridcell.z, gridcell_inc1.z) * ZINC));
	lowz_hash = frac(P * highz_hash.xxxx);
	highz_hash = frac(P * highz_hash.yyyy);
}
//
//	Interpolation functions
//	( smoothly increase from 0.0 to 1.0 as x increases linearly from 0.0 to 1.0 )
//	http://briansharpe.wordpress.com/2011/11/14/two-useful-interpolation-functions-for-noise-development/
//
float3 Interpolation_C2(float3 x) { return x * x * x * (x * (x * 6.0 - 15.0) + 10.0); }
float3 Interpolation_C2_Deriv(float3 x) { return x * x * (x * (x * 30.0 - 60.0) + 30.0); }
//
//	Value3D_Deriv
//	Value3D noise with derivatives
//	returns float3( value, xderiv, yderiv, zderiv )
//
float4 Value3D_Deriv(float3 P)
{
	//	establish our grid cell and unit position
	float3 Pi = floor(P);
	float3 Pf = P - Pi;

	//	calculate the hash.
	//	( various hashing methods listed in order of speed )
	float4 hash_lowz, hash_highz;
	FAST32_hash_3D(Pi, hash_lowz, hash_highz);

	//	blend the results and return
	float3 blend = Interpolation_C2(Pf);
	float4 res0 = lerp(hash_lowz, hash_highz, blend.z);
	float4 res1 = lerp(res0.xyxz, res0.zwyw, blend.yyxx);
	float4 res3 = lerp(float4(hash_lowz.xy, hash_highz.xy), float4(hash_lowz.zw, hash_highz.zw), blend.y);
	float2 res4 = lerp(res3.xz, res3.yw, blend.x);
	return float4(res1.x, 0.0, 0.0, 0.0) + (float4(res1.yyw, res4.y) - float4(res1.xxz, res4.x)) * float4(blend.x, Interpolation_C2_Deriv(Pf));
}

void FAST32_hash_3D(float3 gridcell, float3 v1_mask, float3 v2_mask, out float4 hash_0, out float4 hash_1, out float4 hash_2)		//	generates 3 random numbers for each of the 4 3D cell corners.  cell corners:  v0=0,0,0  v3=1,1,1  the other two are user definable
{
	//    gridcell is assumed to be an integer coordinate

	//	TODO: 	these constants need tweaked to find the best possible noise.
	//			probably requires some kind of brute force computational searching or something....
	const float2 OFFSET = float2(50.0, 161.0);
	const float DOMAIN = 69.0;
	const float3 SOMELARGEFLOATS = float3(635.298681, 682.357502, 668.926525);
	const float3 ZINC = float3(48.500388, 65.294118, 63.934599);

	//	truncate the domain
	gridcell.xyz = gridcell.xyz - floor(gridcell.xyz * (1.0 / DOMAIN)) * DOMAIN;
	float3 gridcell_inc1 = step(gridcell, float3(DOMAIN - 1.5, DOMAIN - 1.5, DOMAIN - 1.5)) * (gridcell + 1.0);

	//	compute x*x*y*y for the 4 corners
	float4 P = float4(gridcell.xy, gridcell_inc1.xy) + OFFSET.xyxy;
	P *= P;
	float4 V1xy_V2xy = lerp(P.xyxy, P.zwzw, float4(v1_mask.xy, v2_mask.xy));		//	apply mask for v1 and v2
	P = float4(P.x, V1xy_V2xy.xz, P.z) * float4(P.y, V1xy_V2xy.yw, P.w);

	//	get the lowz and highz mods
	float3 lowz_mods = float3(1.0 / (SOMELARGEFLOATS.xyz + gridcell.zzz * ZINC.xyz));
	float3 highz_mods = float3(1.0 / (SOMELARGEFLOATS.xyz + gridcell_inc1.zzz * ZINC.xyz));

	//	apply mask for v1 and v2 mod values
	v1_mask = (v1_mask.z < 0.5) ? lowz_mods : highz_mods;
	v2_mask = (v2_mask.z < 0.5) ? lowz_mods : highz_mods;

	//	compute the final hash
	hash_0 = frac(P * float4(lowz_mods.x, v1_mask.x, v2_mask.x, highz_mods.x));
	hash_1 = frac(P * float4(lowz_mods.y, v1_mask.y, v2_mask.y, highz_mods.y));
	hash_2 = frac(P * float4(lowz_mods.z, v1_mask.z, v2_mask.z, highz_mods.z));
}
//
//	Given an arbitrary 3D point this calculates the 4 vectors from the corners of the simplex pyramid to the point
//	It also returns the integer grid index information for the corners
//
void Simplex3D_GetCornerVectors(float3 P,					//	input point
	out float3 Pi,			//	integer grid index for the origin
	out float3 Pi_1,			//	offsets for the 2nd and 3rd corners.  ( the 4th = Pi + 1.0 )
	out float3 Pi_2,
	out float4 v1234_x,		//	vectors from the 4 corners to the intput point
	out float4 v1234_y,
	out float4 v1234_z)
{
	//
	//	Simplex math from Stefan Gustavson's and Ian McEwan's work at...
	//	http://github.com/ashima/webgl-noise
	//

	//	simplex math constants
	const float SKEWFACTOR = 1.0 / 3.0;
	const float UNSKEWFACTOR = 1.0 / 6.0;
	const float SIMPLEX_CORNER_POS = 0.5;
	const float SIMPLEX_PYRAMID_HEIGHT = 0.70710678118654752440084436210485;	// sqrt( 0.5 )	height of simplex pyramid.

	P *= SIMPLEX_PYRAMID_HEIGHT;		// scale space so we can have an approx feature size of 1.0  ( optional )

										//	Find the vectors to the corners of our simplex pyramid
	Pi = floor(P + dot(P, float3(SKEWFACTOR, SKEWFACTOR, SKEWFACTOR)));
	float3 x0 = P - Pi + dot(Pi, float3(UNSKEWFACTOR, UNSKEWFACTOR, UNSKEWFACTOR));
	float3 g = step(x0.yzx, x0.xyz);
	float3 l = 1.0 - g;
	Pi_1 = min(g.xyz, l.zxy);
	Pi_2 = max(g.xyz, l.zxy);
	float3 x1 = x0 - Pi_1 + UNSKEWFACTOR;
	float3 x2 = x0 - Pi_2 + SKEWFACTOR;
	float3 x3 = x0 - SIMPLEX_CORNER_POS;

	//	pack them into a parallel-friendly arrangement
	v1234_x = float4(x0.x, x1.x, x2.x, x3.x);
	v1234_y = float4(x0.y, x1.y, x2.y, x3.y);
	v1234_z = float4(x0.z, x1.z, x2.z, x3.z);
} 

//
//	SimplexPerlin3D_Deriv
//	SimplexPerlin3D noise with derivatives
//	returns float3( value, xderiv, yderiv, zderiv )
//
float4 SimplexPerlin3D_Deriv(float3 P)
{
	//	calculate the simplex vector and index math
	float3 Pi;
	float3 Pi_1;
	float3 Pi_2;
	float4 v1234_x;
	float4 v1234_y;
	float4 v1234_z;
	Simplex3D_GetCornerVectors(P, Pi, Pi_1, Pi_2, v1234_x, v1234_y, v1234_z);

	//	generate the random vectors
	//	( various hashing methods listed in order of speed )
	float4 hash_0;
	float4 hash_1;
	float4 hash_2;
	FAST32_hash_3D(Pi, Pi_1, Pi_2, hash_0, hash_1, hash_2);
	hash_0 -= 0.49999;
	hash_1 -= 0.49999;
	hash_2 -= 0.49999;

	//	normalize random gradient vectors
	float4 norm = rsqrt(hash_0 * hash_0 + hash_1 * hash_1 + hash_2 * hash_2);
	hash_0 *= norm;
	hash_1 *= norm;
	hash_2 *= norm;

	//	evaluate gradients
	float4 grad_results = hash_0 * v1234_x + hash_1 * v1234_y + hash_2 * v1234_z;

	//	evaluate the surflet f(x)=(0.5-x*x)^3
	float4 m = v1234_x * v1234_x + v1234_y * v1234_y + v1234_z * v1234_z;
	m = max(0.5 - m, 0.0);		//	The 0.5 here is SIMPLEX_PYRAMID_HEIGHT^2
	float4 m2 = m*m;
	float4 m3 = m*m2;

	//	calc the deriv
	float4 temp = -6.0 * m2 * grad_results;
	float xderiv = dot(temp, v1234_x) + dot(m3, hash_0);
	float yderiv = dot(temp, v1234_y) + dot(m3, hash_1);
	float zderiv = dot(temp, v1234_z) + dot(m3, hash_2);

	const float FINAL_NORMALIZATION = 37.837227241611314102871574478976;	//	scales the final result to a strict 1.0->-1.0 range

																			//	sum with the surflet and return
	return float4(dot(m3, grad_results), xderiv, yderiv, zderiv) * FINAL_NORMALIZATION;
}

float Perlin3D(float3 P)
{
	//	establish our grid cell and unit position
	float3 Pi = floor(P);
	float3 Pf = P - Pi;
	float3 Pf_min1 = Pf - 1.0;

	//
	//	classic noise.
	//	requires 3 random values per point.  with an efficent hash function will run faster than improved noise
	//

	//	calculate the hash.
	//	( various hashing methods listed in order of speed )
	float4 hashx0, hashy0, hashz0, hashx1, hashy1, hashz1;
	FAST32_hash_3D(Pi, hashx0, hashy0, hashz0, hashx1, hashy1, hashz1);

	//	calculate the gradients
	float4 grad_x0 = hashx0 - 0.49999;
	float4 grad_y0 = hashy0 - 0.49999;
	float4 grad_z0 = hashz0 - 0.49999;
	float4 grad_x1 = hashx1 - 0.49999;
	float4 grad_y1 = hashy1 - 0.49999;
	float4 grad_z1 = hashz1 - 0.49999;
	float4 grad_results_0 = rsqrt(grad_x0 * grad_x0 + grad_y0 * grad_y0 + grad_z0 * grad_z0) * (float2(Pf.x, Pf_min1.x).xyxy * grad_x0 + float2(Pf.y, Pf_min1.y).xxyy * grad_y0 + Pf.zzzz * grad_z0);
	float4 grad_results_1 = rsqrt(grad_x1 * grad_x1 + grad_y1 * grad_y1 + grad_z1 * grad_z1) * (float2(Pf.x, Pf_min1.x).xyxy * grad_x1 + float2(Pf.y, Pf_min1.y).xxyy * grad_y1 + Pf_min1.zzzz * grad_z1);

	//	Classic Perlin Interpolation
	float3 blend = Interpolation_C2(Pf);
	float4 res0 = lerp(grad_results_0, grad_results_1, blend.z);
	float2 res1 = lerp(res0.xy, res0.zw, blend.y);
	float final = lerp(res1.x, res1.y, blend.x);
	final *= 1.1547005383792515290182975610039;		//	(optionally) scale things to a strict -1.0->1.0 range    *= 1.0/sqrt(0.75)
	return final;
}

float4 Simplex3D_GetSurfletWeights(float4 v1234_x, float4 v1234_y, float4 v1234_z)
{
	//	perlins original implementation uses the surlet falloff formula of (0.6-x*x)^4.
	//	This is buggy as it can cause discontinuities along simplex faces.  (0.5-x*x)^3 solves this and gives an almost identical curve

	//	evaluate surflet. f(x)=(0.5-x*x)^3
	float4 surflet_weights = v1234_x * v1234_x + v1234_y * v1234_y + v1234_z * v1234_z;
	surflet_weights = max(0.5 - surflet_weights, 0.0);		//	0.5 here represents the closest distance (squared) of any simplex pyramid corner to any of its planes.  ie, SIMPLEX_PYRAMID_HEIGHT^2
	return surflet_weights*surflet_weights*surflet_weights;
}

float SimplexPerlin3D(float3 P)
{
	//	calculate the simplex vector and index math
	float3 Pi;
	float3 Pi_1;
	float3 Pi_2;
	float4 v1234_x;
	float4 v1234_y;
	float4 v1234_z;
	Simplex3D_GetCornerVectors(P, Pi, Pi_1, Pi_2, v1234_x, v1234_y, v1234_z);

	//	generate the random vectors
	//	( various hashing methods listed in order of speed )
	float4 hash_0;
	float4 hash_1;
	float4 hash_2;
	FAST32_hash_3D(Pi, Pi_1, Pi_2, hash_0, hash_1, hash_2);
	hash_0 -= 0.49999;
	hash_1 -= 0.49999;
	hash_2 -= 0.49999;

	//	evaluate gradients
	float4 grad_results = rsqrt(hash_0 * hash_0 + hash_1 * hash_1 + hash_2 * hash_2) * (hash_0 * v1234_x + hash_1 * v1234_y + hash_2 * v1234_z);

	//	Normalization factor to scale the final result to a strict 1.0->-1.0 range
	//	x = sqrt( 0.75 ) * 0.5
	//	NF = 1.0 / ( x * ( ( 0.5 ? x*x ) ^ 3 ) * 2.0 )
	//	http://briansharpe.wordpress.com/2012/01/13/simplex-noise/#comment-36
	const float FINAL_NORMALIZATION = 37.837227241611314102871574478976;

	//	sum with the surflet and return
	return dot(Simplex3D_GetSurfletWeights(v1234_x, v1234_y, v1234_z), grad_results) * FINAL_NORMALIZATION;
}

float Value3D(float3 P)
{
	//	establish our grid cell and unit position
	float3 Pi = floor(P);
	float3 Pf = P - Pi;

	//	calculate the hash.
	//	( various hashing methods listed in order of speed )
	float4 hash_lowz, hash_highz;
	FAST32_hash_3D(Pi, hash_lowz, hash_highz);

	//	blend the results and return
	float3 blend = Interpolation_C2(Pf);
	float4 res0 = lerp(hash_lowz, hash_highz, blend.z);
	float2 res1 = lerp(res0.xy, res0.zw, blend.y);
	return lerp(res1.x, res1.y, blend.x);
}

float4 FAST32_hash_3D_Cell(float3 gridcell)	//	generates 4 different random numbers for the single given cell point
{
	//    gridcell is assumed to be an integer coordinate

	//	TODO: 	these constants need tweaked to find the best possible noise.
	//			probably requires some kind of brute force computational searching or something....
	const float2 OFFSET = float2(50.0, 161.0);
	const float DOMAIN = 69.0;
	const float4 SOMELARGEFLOATS = float4(635.298681, 682.357502, 668.926525, 588.255119);
	const float4 ZINC = float4(48.500388, 65.294118, 63.934599, 63.279683);

	//	truncate the domain
	gridcell.xyz = gridcell - floor(gridcell * (1.0 / DOMAIN)) * DOMAIN;
	gridcell.xy += OFFSET.xy;
	gridcell.xy *= gridcell.xy;
	return frac((gridcell.x * gridcell.y) * (1.0 / (SOMELARGEFLOATS + gridcell.zzzz * ZINC)));
}

static const int MinVal = -1;
static const int MaxVal = 1;
float Cellular3D(float3 xyz, int cellType, int distanceFunction)
{
	int xi = int(floor(xyz.x));
	int yi = int(floor(xyz.y));
	int zi = int(floor(xyz.z));

	float xf = xyz.x - float(xi);
	float yf = xyz.y - float(yi);
	float zf = xyz.z - float(zi);

	float dist1 = 9999999.0;
	float dist2 = 9999999.0;
	float dist3 = 9999999.0;
	float dist4 = 9999999.0;
	float3 cell;

	for (int z = MinVal; z <= MaxVal; z++) {
		for (int y = MinVal; y <= MaxVal; y++) {
			for (int x = MinVal; x <= MaxVal; x++) {
				cell = FAST32_hash_3D_Cell(float3(xi + x, yi + y, zi + z)).xyz;
				cell.x += (float(x) - xf);
				cell.y += (float(y) - yf);
				cell.z += (float(z) - zf);
				float dist = 0.0;
				if (distanceFunction == 1)
				{
					dist = sqrt(dot(cell, cell));
				}
				else if (distanceFunction == 2)
				{
					dist = dot(cell, cell);
				}
				else if (distanceFunction == 3)
				{
					dist = abs(cell.x) + abs(cell.y) + abs(cell.z);
					dist *= 0.5;
					dist *= dist;
				}
				else if (distanceFunction == 4)
				{
					dist = max(abs(cell.x), max(abs(cell.y), abs(cell.z)));
					dist *= dist;
				}
				else if (distanceFunction == 5)
				{
					dist = dot(cell, cell) + cell.x*cell.y + cell.x*cell.z + cell.y*cell.z;
				}
				else if (distanceFunction == 6)
				{
					dist = pow(abs(cell.x*cell.x*cell.x*cell.x + cell.y*cell.y*cell.y*cell.y + cell.z*cell.z*cell.z*cell.z), 0.25);
				}
				else if (distanceFunction == 7)
				{
					dist = sqrt(abs(cell.x)) + sqrt(abs(cell.y)) + sqrt(abs(cell.z));
					dist *= 0.5;
					dist *= dist;
				}

				if (dist < dist1)
				{
					dist4 = dist3;
					dist3 = dist2;
					dist2 = dist1;
					dist1 = dist;
				}
				else if (dist < dist2)
				{
					dist4 = dist3;
					dist3 = dist2;
					dist2 = dist;
				}
				else if (dist < dist3)
				{
					dist4 = dist3;
					dist3 = dist;
				}
				else if (dist < dist4)
				{
					dist4 = dist;
				}
			}
		}
	}

	if (cellType == 1)	// F1
		return dist1 / 2;	//	scale return value from 0.0->1.333333 to 0.0->1.0  	(2/3)^2 * 3  == (12/9) == 1.333333
	else if (cellType == 2)	// F2
		return dist2 / 2.5;
	else if (cellType == 3)	// F3
		return dist3 / 3;
	else if (cellType == 4)	// F4
		return dist4 / 4;
	else if (cellType == 5)	// F2 - F1 
		return dist2 - dist1;
	else if (cellType == 6)	// F3 - F2 
		return dist3 - dist2;
	else if (cellType == 7)	// F1 + F2/2
		return (dist1 + dist2) / 4.0;
	else if (cellType == 8)	// F1 * F2
		return (dist1 * dist2) / 2.0;
	else if (cellType == 9)	// Crackle
		return max(1.0, 10 * (dist2 - dist1)) * 0.10;
	else return dist1;
}

float4 Cellular_weight_samples(float4 samples)
{
	samples = samples * 2.0 - 1.0;
	//return (1.0 - samples * samples) * sign(samples);	// square
	return (samples * samples * samples) - sign(samples);	// cubic (even more variance)
}

float Cellular3D(float3 P)
{
	//	establish our grid cell and unit position
	float3 Pi = floor(P);
	float3 Pf = P - Pi;

	//	calculate the hash.
	//	( various hashing methods listed in order of speed )
	float4 hash_x0, hash_y0, hash_z0, hash_x1, hash_y1, hash_z1;
	FAST32_hash_3D(Pi, hash_x0, hash_y0, hash_z0, hash_x1, hash_y1, hash_z1);

	//	generate the 8 random points
	//	restrict the random point offset to eliminate artifacts
	//	we'll improve the variance of the noise by pushing the points to the extremes of the jitter window
	const float JITTER_WINDOW = 0.166666666;	// 0.166666666 will guarentee no artifacts. It is the intersection on x of graphs f(x)=( (0.5 + (0.5-x))^2 + 2*((0.5-x)^2) ) and f(x)=( 2 * (( 0.5 + x )^2) + x * x )
	hash_x0 = Cellular_weight_samples(hash_x0) * JITTER_WINDOW + float4(0.0, 1.0, 0.0, 1.0);
	hash_y0 = Cellular_weight_samples(hash_y0) * JITTER_WINDOW + float4(0.0, 0.0, 1.0, 1.0);
	hash_x1 = Cellular_weight_samples(hash_x1) * JITTER_WINDOW + float4(0.0, 1.0, 0.0, 1.0);
	hash_y1 = Cellular_weight_samples(hash_y1) * JITTER_WINDOW + float4(0.0, 0.0, 1.0, 1.0);
	hash_z0 = Cellular_weight_samples(hash_z0) * JITTER_WINDOW + float4(0.0, 0.0, 0.0, 0.0);
	hash_z1 = Cellular_weight_samples(hash_z1) * JITTER_WINDOW + float4(1.0, 1.0, 1.0, 1.0);

	//	return the closest squared distance
	float4 dx1 = Pf.xxxx - hash_x0;
	float4 dy1 = Pf.yyyy - hash_y0;
	float4 dz1 = Pf.zzzz - hash_z0;
	float4 dx2 = Pf.xxxx - hash_x1;
	float4 dy2 = Pf.yyyy - hash_y1;
	float4 dz2 = Pf.zzzz - hash_z1;
	float4 d1 = dx1 * dx1 + dy1 * dy1 + dz1 * dz1;
	float4 d2 = dx2 * dx2 + dy2 * dy2 + dz2 * dz2;
	d1 = min(d1, d2);
	d1.xy = min(d1.xy, d1.wz);
	return min(d1.x, d1.y) * (9.0 / 12.0);	//	scale return value from 0.0->1.333333 to 0.0->1.0  	(2/3)^2 * 3  == (12/9) == 1.333333
}