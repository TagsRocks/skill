#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace Skill
{
	namespace Fbx
	{		
		ref class SceneNode;
		class MeshData;
		struct Vertex;

		class ReduceNode
		{
		public:
			int PossibleNeighboreMerge;
			bool IsRemovable;
			int VertexId;
			int FaceId[5]; // maximum 5
		};

		public class MeshProcessor
		{

		private:

			double _PositionsTelorance;
			double _UvTelorance;


		public:

			double GetPositionsTelorance(){return _PositionsTelorance;}
			void SetPositionsTelorance(double value){ _PositionsTelorance = value;}

			double GetUvTelorance(){return _UvTelorance;}
			void SetUvTelorance(double value){ _UvTelorance = value;}

			//property double PositionsTelorance { double get(){ return _PositionsTelorance; } void set(double value) { _PositionsTelorance = value;} }
			//property double UvTelorance { double get(){ return _UvTelorance; } void set(double value) { _UvTelorance = value;} }
			
			MeshProcessor();
		
			void ReduceVertex(MeshData& mesh );
			MeshData* ReduceFace(MeshData& mesh);
			void GenerateNormals(MeshData& meshData);
			void GenerateTangentAndBinormals(MeshData& meshData);
			MeshData* Merge(FbxArray<MeshData*>& meshes);
			void TransformToGlobal(FbxNode* node, MeshData& meshData);
			void TransformToLocal(FbxNode* node, MeshData& meshData);
			void TransformToBindPose(FbxMesh* node,MeshData& meshData);
		private:
			// returns number of vertex
			
			void CreateTangent(Vertex& v0, Vertex& v1, Vertex& v2, FbxVector4* tangentArray);			
			

		};
	}
}