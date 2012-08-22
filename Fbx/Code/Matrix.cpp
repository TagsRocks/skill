#include "Stdafx.h"
#include "Matrix.h"

namespace Skill
{
	namespace Fbx
	{
		Matrix::Matrix(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24, double m31, double m32, double m33, double m34, double m41, double m42, double m43, double m44)
		{
			_11=m11;
			_12=m12;
			_13=m13;
			_14=m14;

			_21=m21;
			_22=m22;
			_23=m23;
			_24=m24;

			_31=m31;
			_32=m32;
			_33=m33;
			_34=m34;

			_41=m41;
			_42=m42;
			_43=m43;
			_44=m44;

		}		

		Matrix Matrix::Identity::get()
		{
			Matrix matrix;
			matrix._11 = 1.0f;
			matrix._12 = 0.0f;
			matrix._13 = 0.0f;
			matrix._14 = 0.0f;
			matrix._21 = 0.0f;
			matrix._22 = 1.0f;
			matrix._23 = 0.0f;
			matrix._24 = 0.0f;
			matrix._31 = 0.0f;
			matrix._32 = 0.0f;
			matrix._33 = 1.0f;
			matrix._34 = 0.0f;
			matrix._41 = 0.0f;
			matrix._42 = 0.0f;
			matrix._43 = 0.0f;
			matrix._44 = 1.0f;
			return matrix;
		}

		FbxVector4 Matrix::TransformNormal(FbxVector4 &normal, Matrix &matrix)
		{
			FbxVector4 vector;
			double num3 = ((normal[0] * matrix._11) + (normal[1]* matrix._21)) + (normal[2]* matrix._31);
			double num2 = ((normal[0] * matrix._12) + (normal[1] * matrix._22)) + (normal[2] * matrix._32);
			double num = ((normal[0] * matrix._13) + (normal[1]* matrix._23)) + (normal[2] * matrix._33);
			vector.Set(num3, num2, num);
			return vector;
		}
		FbxVector4 Matrix::Transform(FbxVector4 &position, Matrix &matrix)
		{
			FbxVector4 vector;
			double num3 = (((position[0] * matrix._11) + (position[1]* matrix._21)) + (position[2]* matrix._31)) + matrix._41;
			double num2 = (((position[0] * matrix._12) + (position[1] * matrix._22)) + (position[2] * matrix._32)) + matrix._42;
			double num = (((position[0] * matrix._13) + (position[1] * matrix._23)) + (position[2] * matrix._33)) + matrix._43;
			vector.Set(num3, num2, num);
			return vector;
		}

		Matrix Matrix::CreateTranslation(FbxVector4 &position)
		{
			Matrix matrix = Identity;
			matrix._41 = position[0];
			matrix._42 = position[1];
			matrix._43 = position[2];
			return matrix;
		}
		Matrix Matrix::CreateScale(FbxVector4 &scale)
		{
			Matrix matrix = Identity;

			matrix._11 = scale[0];			
			matrix._22 = scale[1];			
			matrix._33 = scale[2];			

			return matrix;
		}
		Matrix Matrix::CreateFromQuaternion(FbxQuaternion &quaternion)
		{
			Matrix matrix = Identity;

			double num9 = quaternion[0] * quaternion[0];
			double num8 = quaternion[1] * quaternion[1];
			double num7 = quaternion[2] * quaternion[2];
			double num6 = quaternion[0] * quaternion[1];
			double num5 = quaternion[2] * quaternion[3];
			double num4 = quaternion[2] * quaternion[0];
			double num3 = quaternion[1] * quaternion[3];
			double num2 = quaternion[1] * quaternion[2];
			double num = quaternion[0] * quaternion[3];
			matrix._11 = 1.0f - (2.0f * (num8 + num7));
			matrix._12 = 2.0f * (num6 + num5);
			matrix._13 = 2.0f * (num4 - num3);
			matrix._14 = 0.0f;
			matrix._21 = 2.0f * (num6 - num5);
			matrix._22 = 1.0f - (2.0f * (num7 + num9));
			matrix._23 = 2.0f * (num2 + num);
			matrix._24 = 0.0f;
			matrix._31 = 2.0f * (num4 + num3);
			matrix._32 = 2.0f * (num2 - num);
			matrix._33 = 1.0f - (2.0f * (num8 + num9));
			matrix._34 = 0.0f;
			matrix._41 = 0.0f;
			matrix._42 = 0.0f;
			matrix._43 = 0.0f;
			matrix._44 = 1.0f;

			return matrix;
		}

		Matrix Matrix::GlobalNode(FbxNode* node)
		{
			FbxAMatrix aMatrix = node->EvaluateGlobalTransform();
			return CreateFromAMatrix(aMatrix);
		}
		Matrix Matrix::LocalNode(FbxNode* node)
		{
			FbxAMatrix aMatrix = node->EvaluateLocalTransform();
			return CreateFromAMatrix(aMatrix);
		}
		Matrix Matrix::CreateFromAMatrix(FbxAMatrix& aMatrix)
		{
			Matrix matrix;

			// **** Method 1 ****

			matrix._11 = aMatrix.Get(0,0);
			matrix._12 = aMatrix.Get(0,1);
			matrix._13 = aMatrix.Get(0,2);
			matrix._14 = aMatrix.Get(0,3);

			matrix._21 = aMatrix.Get(1,0);
			matrix._22 = aMatrix.Get(1,1);
			matrix._23 = aMatrix.Get(1,2);
			matrix._24 = aMatrix.Get(1,3);

			matrix._31 = aMatrix.Get(2,0);
			matrix._32 = aMatrix.Get(2,1);
			matrix._33 = aMatrix.Get(2,2);
			matrix._34 = aMatrix.Get(2,3);

			matrix._41 = aMatrix.Get(3,0);
			matrix._42 = aMatrix.Get(3,1);
			matrix._43 = aMatrix.Get(3,2);
			matrix._44 = aMatrix.Get(3,3);

			// **** Method 2 ****

			//FbxVector4 translation = aMatrix.GetT();
			//FbxQuaternion rotation = aMatrix.GetQ(); 
			//FbxVector4 scale = aMatrix.GetS();

			//Matrix tM = CreateTranslation(translation);
			//Matrix rM = CreateFromQuaternion(rotation);
			//Matrix sM =  CreateScale(scale);

			//matrix = sM * rM * tM;

			return matrix;
		}
	}
}