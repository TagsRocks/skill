#pragma once

namespace Skill
{
	namespace Fbx
	{
		class MeshData;		

		typedef FbxNode* P_FbxNode;

		class BoneSkin
		{
		public:

			/// <summary>
			/// Name of bone
			/// </summary>
			P_FbxNode Bone;
			/// <summary>
			/// WeightIndex data for vertices that influenced by bone
			/// </summary>
			double* Weights;
			int* Indices;
			int WeightCount;

			BoneSkin()
			{
				Bone = NULL;
				Weights = NULL;
				Indices = NULL;
				WeightCount = 0;
			}
			~BoneSkin()
			{
				if(Indices) delete[] Indices;
				if(Weights) delete[] Weights;

				Bone = NULL;
				Weights = NULL;
				Indices = NULL;
				WeightCount = 0;
			}

		};

		/// <summary>
		/// Contains weight value for single vertex
		/// </summary>
		struct VertexWeight
		{
			/// <summary>
			/// Reference to bone
			/// </summary>
			P_FbxNode Bone;
			/// <summary>
			/// Weight of vertex
			/// </summary>
			double Weight;

			VertexWeight(P_FbxNode bone,double w)
			{
				Bone = bone;
				Weight=w;
			}

		};

		class VertexWeightArray
		{
		public:
			FbxArray<VertexWeight> Array;

			VertexWeightArray()
			{
				Array.Clear();
			}

			~VertexWeightArray()
			{
				Array.Clear();
			}
		};

		class FbxMeshLoader
		{
		public:
			FbxMeshLoader(FbxMesh* mesh);
			~FbxMeshLoader(void);

			void Fill(MeshData* meshdata);

		private:
			void LoadControlsPoints();
			void LoadPolygonIndices();
			void LoadNormals();
			void LoadUvsA();
			void LoadUvsB();
			FbxVector2* LoadUvs(FbxGeometryElementUV* leUV, int& uvsCount);
			void LoadColors();
			void LoadSkins();
			void LoadFaceMaterials();
			void LoadMaterialNames();
			void GenerateBlendWeightIndices();			
			void Destroy();


			FbxMesh* _Mesh;
			int _ControlsPointsCount;
			FbxVector4* _ControlsPoints;

			FbxVector4* _Normals;
			int _NormalCount;

			VertexWeightArray* _WeightPerVertex;
			int _VertexWeightCount;

			int* _PolygonIndices;
			int _PolyCount;

			FbxVector2* _UvsA;
			int _UvsCountA;

			FbxVector2* _UvsB;
			int _UvsCountB;

			FbxColor* _Colors;
			int _ColorCount;

			BoneSkin* _BoneSkins;
			int _BoneSkinCount;

			int* _FaceMaterial;
			FbxString* _MaterialNames;
			int _MaterialCount;
			
		};

	}}