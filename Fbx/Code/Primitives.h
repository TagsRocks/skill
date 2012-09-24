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

		class TextureInfo
		{
		public:			

			const char* Name;
			const char* FileName;
			FbxTexture::ETextureUse TextureUse;
			FbxFileTexture::EMaterialUse MaterialUse;
			FbxTexture::EMappingType MappingType;
			bool SwapUV;
			FbxDouble2 Translation;
			FbxDouble2 Scale;
			FbxDouble2 Rotation;
			double DefaultAlpha;
			FbxTexture::EBlendMode BlendMode;	

			TextureInfo()
			{				
				FileName = NULL;
				Name = NULL;
			}
		};
		
		class Material
		{
		public:
			const char* Name;
			bool IsPhong;

			FbxDouble3 Ambient;
			FbxDouble3 Diffuse;
			FbxDouble3 Specular;
			FbxDouble3 Emissive;
			FbxDouble TransparencyFactor;			
			FbxDouble Shininess;
			FbxDouble ReflectionFactor;

			TextureInfo* AmbientTexture;
			TextureInfo* DiffuseTexture;
			TextureInfo* SpecularTexture;
			TextureInfo* EmissiveTexture;

			int Index;

			Material()
			{
				AmbientTexture = NULL;
				DiffuseTexture = NULL;
				SpecularTexture = NULL;
				EmissiveTexture = NULL;
			}
			~Material()
			{
				AmbientTexture = NULL;
				DiffuseTexture = NULL;
				SpecularTexture = NULL;
				EmissiveTexture = NULL;
			}
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