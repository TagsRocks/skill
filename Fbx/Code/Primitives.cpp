#include "Stdafx.h"
#include "Primitives.h"

using namespace System;
using namespace System::Collections::Generic;

namespace Skill
{
	namespace Fbx
	{
		Vertex::Vertex()
		{
			FbxVector4 zero4 = FbxVector4();
			FbxVector2 zero2 = FbxVector2();

			this->Position = zero4;
            this->Normal = zero4;
            this->Tangent = zero4;
			this->Binormal = zero4;
            this->UV = zero2;
			this->UV2 = zero2;
            this->Color = FbxColor();
			
			this->Bones.Clear();
			this->Weights.Clear();
		}
		Vertex::Vertex(Vertex& other)
		{
			this->Position = other.Position;
            this->Normal = other.Normal;
            this->Tangent = other.Tangent;
			this->Binormal = other.Binormal;
            this->UV = other.UV;
			this->UV2 = other.UV2 ;
            this->Color = other.Color;
			
			this->Bones.Clear();
			this->Weights.Clear();

			for(int i = 0; i < other.Bones.GetCount(); i++)
			{
				FbxNode* node = other.Bones.GetAt(i);
				double w = other.Weights.GetAt(i);
				this->Bones.Add(node);
				this->Weights.Add(w);
			}
		}

		void VertexArray::Destroy()
		{
			if(_Array) delete[] _Array;
			_Array = NULL;
			_Count = 0;
		}
		VertexArray::VertexArray()
		{
			_Array = NULL;
			_Count = 0;
		}
		VertexArray::~VertexArray()
		{			
			Destroy();
		}

		Vertex& VertexArray::operator[](int index)
		{
			return _Array[index];
		}

		int VertexArray::GetCount() { return _Count;}
		void VertexArray::Reserve(int count) { Destroy(); _Count = count; _Array = new Vertex[_Count];  }
		void VertexArray::SetCount(int count) 
		{
			Vertex* arr = new Vertex[count];
			for(int i=0; i < count; i++)
				arr[i] = _Array[i];
			Destroy();
			_Count = count;
			_Array = arr;
		} 
		void VertexArray::Clear() { Destroy(); }

		void VertexArray::SetAt(int index , Vertex v) {_Array[index] = v; }
		Vertex VertexArray::GetAt(int index ) { return _Array[index]; }


		Face::Face()
		{
			this->A = 0;
			this->B = 0;
			this->C = 0;
			this->MaterialId = -1;
		}

		int& Face::operator[]( int index )
		{
				if(index == 0) return A;
				else if(index == 1) return B;
				else return C;
		}

		MeshData::MeshData()
		{				
			
			this->HasNormal = false;
			this->HasUV2 = false;
			this->HasTangent = false;
			this->HasColor = false;
			this->HasSkin = false;

		}

		MeshData::~MeshData()
		{	
			this->Vertices.Clear();
			this->Faces.Clear();			
			this->Materials.Clear();

			this->HasNormal = false;
			this->HasUV2 = false;
			this->HasTangent = false;	
			this->HasColor = false;			
			this->HasSkin = false;			
		}
	}
}