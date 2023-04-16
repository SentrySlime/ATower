
half4x4 _FaceMatrix;
half4 _Rect;
half _FovRad;
half _FovExpansion;

bool isDirectionSync(half3 dir)
{
	return dir.z > 0;
}

bool isUVAcceptable(half2 uv)
{
	if (abs(uv.x - 0.5f) <= 0.5f)
		return abs(uv.y - 0.5f) <= 0.5f;
	else
		return false;
}

half2 getCoordinate(half3 dir)
{

	return dir.xy / dir.z;
}


#if SHADER_API_GLES
// GLES 2.0 doesn't have hyperbolic functions...
half sinh(half x)
{
	half ex = exp(x);
	return (ex - 1 / ex)*0.5f;
}
#endif

half2 mapTransform(half2 uv)
{
	uv = half2(uv.x, 1 - uv.y);
	// _FovRad -> 0 - 360 -> 0 - pi/2
	uv = uv - half2(0.5, 0.5);
#if MAP_STEREOGRAPHIC
	uv = uv * 2 * tan(_FovRad);
#elif MAP_EQUIDISTANT
	uv = uv * _FovRad * 4;
#elif MAP_EQUISOLID
	uv = uv * 4 * sin(_FovRad);
#elif MAP_ORTHOGRAPHIC
	uv = uv * 4 * sin(_FovRad);
#elif MAP_GEOGRAPHIC || MAP_MILLER
	half aspect = (_ScreenParams.y*(_ScreenParams.z - 1));
	uv = uv * (_FovRad * 4) + half2(-UNITY_HALF_PI * aspect, UNITY_HALF_PI);
#elif MAP_CASSINI
	uv = uv * _FovRad * 4;
#endif
	uv = uv * _Rect.xy + _Rect.zw;
	return uv;
}

// https://en.wikipedia.org/wiki/Stereographic_projection
half3 mapStereographic(half2 uv)
{
	half xy = dot(uv, uv);
	half xyp = 1 / (1 + xy);
	half xyp2 = 2 * xyp;
	return half3(uv.x * xyp2, -uv.y * xyp2, -(-1 + xy) * xyp);
}

// http://mathworld.wolfram.com/AzimuthalEquidistantProjection.html
half3 mapEquidistant(half2 uv)
{
	half c = length(uv);

	return half3(uv.x*sin(c), -uv.y*sin(c), c*cos(c));
}

// https://en.wikipedia.org/wiki/Lambert_azimuthal_equal-area_projection
half3 mapEquisolid(half2 uv)
{
	half xy = dot(uv, uv);
	half p = sqrt(1 - xy * 0.25);
	return half3(uv.x * p, -uv.y * p, 1 - xy * 0.5);
}

// https://en.wikipedia.org/wiki/Orthographic_projection_in_cartography
half3 mapOrthographic(half2 uv)
{
	half xy = dot(uv, uv);
	half p = sqrt(xy);
	return half3(uv.x * p, -uv.y * p, p * sqrt(1 - xy));
}

// https://en.wikipedia.org/wiki/Equirectangular_projection
half3 mapGeographic(half2 uv)
{
	half cx, cy, sx, sy;
	sincos(uv.x, sx, cx);
	sincos(uv.y, sy, cy);
	return half3(cx * sy, cy, -sx * sy);
}

// https://en.wikipedia.org/wiki/Miller_cylindrical_projection
half3 mapMiller(half2 uv)
{
	half cx, cy, sx, sy;
	sincos(uv.x, sx, cx);
	sy = sin(uv.y);
	cy = cos(1.25 * atan(sinh(1.25*uv.y)));
	return half3(cx * sy, cy, -sx * sy);
}

// https://en.wikipedia.org/wiki/Cassini_projection
half3 mapCassini(half2 uv)
{
	half cx, cy, sx, sy;
	sincos(uv.x, sx, cx);
	sincos(uv.y, sy, cy);
	return half3(sx / cx, -sy * cx, cy);
}

half3 map(half2 uv)
{
#if MAP_STEREOGRAPHIC
	return mapStereographic(uv);
#elif MAP_EQUIDISTANT
	return mapEquidistant(uv);
#elif MAP_EQUISOLID
	return mapEquisolid(uv);
#elif MAP_ORTHOGRAPHIC
	return mapOrthographic(uv);
#elif MAP_GEOGRAPHIC
	return mapGeographic(uv);
#elif MAP_MILLER
	return mapMiller(uv);
#elif MAP_CASSINI
	return mapCassini(uv);
#endif
}


bool maskStereographic(half2 uv)
{
	return true;
}

bool maskEquidistant(half2 uv)
{
	return dot(uv, uv) < UNITY_PI * UNITY_PI;
}

bool maskEquisolid(half2 uv)
{
	half xy = dot(uv, uv);
	return xy * 0.25 < 1;
}

bool maskOrthographic(half2 uv)
{
	half xy = dot(uv, uv);
	return xy < 1;
}

bool maskGeographic(half2 uv)
{
	return abs(uv.y - UNITY_HALF_PI) < UNITY_HALF_PI;
}

bool maskMiller(half2 uv)
{
	return abs(uv.y - UNITY_HALF_PI) < UNITY_HALF_PI;
}

bool maskCassini(half2 uv)
{
	return abs(uv.x) < UNITY_HALF_PI;
}

bool mask(half2 uv)
{
#if MAP_STEREOGRAPHIC
	return maskStereographic(uv);
#elif MAP_EQUIDISTANT
	return maskEquidistant(uv);
#elif MAP_EQUISOLID
	return maskEquisolid(uv);
#elif MAP_ORTHOGRAPHIC
	return maskOrthographic(uv);
#elif MAP_GEOGRAPHIC
	return maskGeographic(uv);
#elif MAP_MILLER
	return maskMiller(uv);
#elif MAP_CASSINI
	return maskCassini(uv);
#endif
}

