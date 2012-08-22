#pragma once

namespace Skill
{
	namespace Fbx
	{
		public value struct Matrix
		{
		public:
			
		double        _11, _12, _13, _14;
		double        _21, _22, _23, _24;
		double        _31, _32, _33, _34;
		double        _41, _42, _43, _44;							
		
		public:

		#pragma region Constructor


			/// <summary>
			/// Initializes a new instance of Matrix.
			/// </summary>			
			Matrix(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24, double m31, double m32, double m33, double m34, double m41, double m42, double m43, double m44);

		#pragma endregion 

			static property Matrix Identity { Matrix get(); }

		internal:

			static  FbxVector4 TransformNormal(FbxVector4 &normal, Matrix &matrix);
			static  FbxVector4 Transform(FbxVector4 &position, Matrix &matrix);
			

			static  Matrix CreateTranslation(FbxVector4 &position);			
			static  Matrix CreateScale(FbxVector4 &scale);			
			static  Matrix CreateFromQuaternion(FbxQuaternion &quaternion);

			static  Matrix GlobalNode(FbxNode* node);
			static  Matrix LocalNode(FbxNode* node);
			static  Matrix CreateFromAMatrix(FbxAMatrix& aMatrix);

		};
	}
}