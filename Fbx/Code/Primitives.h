#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace Skill
{
	namespace Fbx
	{
		struct Vertex
        {
		public:

			Vertex(Vertex& other);
			Vertex();

			FbxVector4 Position;
            FbxVector4 Normal;
            FbxVector4 Tangent;
			FbxVector4 Binormal;
            FbxVector2 UV;
			FbxVector2 UV2;
            FbxColor Color;
			
			FbxArray<FbxNode*> Bones;
			FbxArray<double> Weights;
            
        };

		class VertexArray
		{
		private:
			Vertex* _Array;
			int _Count;

			void Destroy();

		public:

			~VertexArray();
			VertexArray();
			
			Vertex& operator[](int index);

			int GetCount();
			void Reserve(int count);
			void SetCount(int count);
			void Clear();
			void SetAt(int index , Vertex v);
			Vertex GetAt(int index );
		};


		struct Face
		{
		public:			
			int A;
			int B;
			int C;
			int MaterialId;

			Face();
			int& operator[]( int index );

		};		

		class Material
		{
		public:
			FbxString Name;
			int Index;
		};


		class MeshData
		{
		public :

			MeshData();

			VertexArray Vertices;
			FbxArray<Face> Faces;			
			FbxArray<Material> Materials;

			bool HasNormal;
			bool HasUV2;
			bool HasTangent;
			bool HasColor;		
			bool HasSkin;

			~MeshData();
		};
	}
}