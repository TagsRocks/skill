
#pragma once

namespace Skill
{
	namespace Fbx
	{
		public enum class FbxAttributeType
		{
			Unknown,
			Null,
			Marker,
			Skeleton, 
			Mesh, 
			Nurbs, 
			Patch,
			Camera, 
			CameraStereo,
			CameraSwitcher,
			Light,
			OpticalReference,
			OpticalMarker,
			NurbsCurve,
			TrimNurbsSurface,
			Boundary,
			NurbsSurface,
			Shape,
			LODGroup,
			SubDiv,
			CachedEffect,
			Line
		};
	}
}