using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Skill.Managers
{
    #region SearchGridResult
    /// <summary>
    /// Contains information about found transforms after searching grids
    /// </summary>
    public class SearchGridResult
    {
        /// <summary>
        /// Found Transform
        /// </summary>
        public Transform Transform { get; private set; }
        /// <summary>
        /// Distance to center of search. can be used to sort transforms
        /// </summary>
        public float Distance { get; private set; }

        /// <summary>
        /// Create a SearchGridResult
        /// </summary>
        /// <param name="transform">Found Transform</param>
        /// <param name="distance">Distance to center of search</param>
        public SearchGridResult(Transform transform, float distance)
        {
            this.Transform = transform;
            this.Distance = distance;
        }
    }

    /// <summary>
    /// A Comparer of SearchGridResult to use in sort algorithm
    /// </summary>
    public class SearchGridResultComparer : IComparer<SearchGridResult>
    {
        /// <summary>
        /// Compare for descending sort
        /// </summary>
        public bool Descending { get; set; }

        /// <summary>
        /// Compare two given SearchGridResult 
        /// </summary>
        /// <param name="x">First SearchGridResult </param>
        /// <param name="y">Second SearchGridResult </param>
        /// <returns>Result of compare</returns>
        public int Compare(SearchGridResult x, SearchGridResult y)
        {
            if (Descending)
            {
                if (x.Distance > y.Distance)
                    return 1;
                else
                    return -1;
            }
            else
            {
                if (x.Distance < y.Distance)
                    return 1;
                else
                    return -1;
            }
        }
    }
    #endregion

    /// <summary>
    /// This class contains as need as Grid2Ds to keep track of, and quickly search, any Game Objects in a level.
    /// The center of SearchGrid2D is (0,0). just look at Transform.position and place GameObject somewhere in Grid2D.
    /// If an object is dynamic you have to update it in SearchGrid2D whenever it moves.
    /// </summary>
    /// <remarks>
    /// Usually this class is usefull for keep track of ai information like : cover points, shelters, ... .
    /// if objects have colliders, it is beter to use Physics.OverlapSphere.
    /// </remarks>
    public class SearchGrid2D : IEnumerable<Grid2D>
    {
        // usually searches done around player and player does not move very fast, so we hold reference to last success searched Grid
        // and always start search in that one to improve performance by removing search for valid grid by chance of new search is probebly inside last used grid.
        private Grid2D _LastUsedGrid;
        private Quad[] _QuadList;// list of quads to fill in GetQuads method
        private float _QuadSize; // size of each quad
        private int _GridDimension;// dimension of each grid
        private float _Length;// length of each grid
        private float _Length2; // _Length divided by 2
        private List<Grid2D> _Grids;

        /// <summary>
        /// Size of each quad along x or z axis
        /// </summary>
        public float QuadSize { get { return _QuadSize; } }
        /// <summary>
        /// Dimention of each grid along x or z axis
        /// </summary>
        public int GridDimension { get { return _GridDimension; } }

        /// <summary>
        /// Create a SearchGrid2D
        /// </summary>
        /// <param name="gridDimension"> Dimention of each grid along x or z axis </param>
        /// <param name="quadSize">Size of each quad along x or z axis</param>
        public SearchGrid2D(int gridDimension = 32, float quadSize = 32)
        {
            this._GridDimension = gridDimension;
            this._QuadSize = quadSize;
            this._Length = gridDimension * quadSize;
            this._Length2 = this._Length / 2;
            _Grids = new List<Grid2D>();
            _QuadList = new Quad[60];
        }

        /// <summary>
        /// Add Transform to search grid
        /// </summary>
        /// <param name="transform">Transform to add</param>        
        public void Add(Transform transform)
        {
            if (_LastUsedGrid != null)// first check for last used grid
            {
                if (_LastUsedGrid.Add(transform))
                    return;
            }
            Vector3 pos = transform.position;
            Grid2D quadGrid = GetQuadGrid(ref pos, true);// find the grid that contains Transform.posiotn
            if (quadGrid != null)
                quadGrid.Add(transform);
        }

        /// <summary>
        /// Remove Transform from search grid
        /// </summary>
        /// <param name="transform">Transform to remove</param>        
        public bool Remove(Transform transform)
        {
            if (_LastUsedGrid != null) // first check for last used grid
            {
                if (_LastUsedGrid.Remove(transform))
                    return true;
            }
            Vector3 pos = transform.position;
            Grid2D quadGrid = GetQuadGrid(ref pos, false);
            if (quadGrid != null)
            {
                return quadGrid.Remove(transform);
            }
            return false;
        }

        /// <summary>
        /// Update Transform in search grid. use this method whenever object moves.
        /// </summary>
        /// <param name="transform">Transform to update</param>           
        public void Update(Transform transform)
        {
            Remove(transform);
            Add(transform);
        }

        /// <summary>
        /// find thegrid that contains specified point
        /// </summary>
        /// <param name="position">Point to check</param>
        /// <param name="create"> Create grid if does not exist (created yet) </param>
        /// <returns></returns>
        private Grid2D GetQuadGrid(ref Vector3 position, bool create = true)
        {
            _LastUsedGrid = null;
            foreach (var qd in _Grids) // search in created grids
            {
                if (qd.Contains(ref position))
                {
                    _LastUsedGrid = qd;
                    break;
                }
            }
            if (_LastUsedGrid == null && create)
            {
                float x = Mathf.FloorToInt((position.x + _Length2) / _Length) * _Length - (Mathf.Sign(position.x) * _Length2);
                float z = Mathf.FloorToInt((position.z + _Length2) / _Length) * _Length - (Mathf.Sign(position.x) * _Length2);

                _LastUsedGrid = new Grid2D(_GridDimension, x, z, _QuadSize);
                _Grids.Add(_LastUsedGrid);
            }
            return _LastUsedGrid;
        }

        /// <summary>
        /// Search for Transforms in specified circle
        /// </summary>
        /// <param name="center">Center of circle</param>
        /// <param name="radius">Radius of circle</param>
        /// <param name="tag">Tag to filter Transforms</param>
        /// <returns>List of SearchGridResult that is inside circle</returns>
        public List<SearchGridResult> OverlapCircle(Vector3 center, float radius, string tag = null)
        {
            List<SearchGridResult> objects = new List<SearchGridResult>();
            OverlapCircle(center, radius, objects, tag);
            return objects;
        }

        /// <summary>
        /// Search for Transforms in specified circle
        /// </summary>
        /// <param name="position">Center of circle</param>
        /// <param name="radius">Radius of circle</param>                
        /// <param name="objects">List of objects to fill</param>
        /// <param name="tag">Tag to filter Transforms</param>
        public void OverlapCircle(Vector3 position, float radius, List<SearchGridResult> objects, string tag = null)
        {
            position.y = 0;
            int index = 0;
            foreach (var grid in _Grids)
            {
                index += grid.GetQuads(position, radius, _QuadList, index);
                if (index >= _QuadList.Length)
                {
                    index = _QuadList.Length;
                    break;
                }
            }

            if (tag != null)
            {
                for (int i = 0; i < index; i++)
                {
                    foreach (Transform tr in _QuadList[i].Objetcs)
                    {
                        if (tr != null && tr.tag == tag)
                        {
                            float distance = Vector3.Distance(position, tr.position);
                            if (distance <= radius)
                                objects.Add(new SearchGridResult(tr, distance));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    foreach (Transform tr in _QuadList[i].Objetcs)
                    {
                        if (tr != null)
                        {
                            float distance = Vector3.Distance(position, tr.position);
                            if (distance <= radius)
                                objects.Add(new SearchGridResult(tr, distance));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clear and remove all Transforms from search grid
        /// </summary>
        public void Clear()
        {
            foreach (var grid in _Grids)
            {
                grid.Clear();
            }
            _Grids.Clear();
            _LastUsedGrid = null;
        }

        /// <summary>
        /// Get enumerators of Grid2Ds
        /// </summary>
        /// <returns>IEnumerator<Grid2D></returns>
        public IEnumerator<Grid2D> GetEnumerator()
        {
            return _Grids.GetEnumerator();
        }

        /// <summary>
        /// Get enumerators of Grid2Ds
        /// </summary>
        /// <returns>IEnumerator<Grid2D></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_Grids as IEnumerable).GetEnumerator();
        }
    }

    #region Grid2D
    /// <summary>
    /// A larger 2d rectangle in world made of array of quads
    /// </summary>
    public class Grid2D
    {
        private int _Dimention;
        private float _X, _Z, _Top, _Right, _QuadSize;
        private float _Length;

        /// <summary>
        /// Array of quads in size [Dimention , Dimention]
        /// </summary>
        public Quad[,] Quads { get; private set; }

        /// <summary> Number of quads in each row and column </summary>
        public int Dimention { get { return _Dimention; } }
        /// <summary> Start position of Grid2D in x axis (min X) </summary>
        public float X { get { return _X; } }
        /// <summary> Start position of Grid2D in z axis (min Z) </summary>
        public float Z { get { return _Z; } }
        /// <summary> End position of Grid2D in x axis (max X) </summary>
        public float Right { get { return _Right; } }
        /// <summary> End position of Grid2D in z axis (max Z) </summary>
        public float Top { get { return _Top; } }
        /// <summary> Size of each quad </summary>
        public float QuadSize { get { return _QuadSize; } }
        /// <summary> Lenght of Grid2D along x or z axis (Dimention * QuadSize) </summary>
        public float Length { get { return _Length; } }

        /// <summary>
        /// Whether this Grid2D contains given point in 2D space
        /// </summary>
        /// <param name="pos">Point to check</param>
        /// <returns>True if containes, otherwise false</returns>
        public bool Contains(ref Vector3 pos)
        {
            return (pos.x >= _X) && (pos.z >= _Z) && (pos.x <= _Length) && (pos.z <= _Top);
        }

        /// <summary>
        /// Whether this Grid2D contains given point in 2D space
        /// </summary>
        /// <param name="pos">Point to check</param>
        /// <returns>True if containes, otherwise false</returns>
        public bool Contains(Vector3 pos)
        {
            return Contains(ref pos);
        }

        /// <summary>
        /// Create an instance of Grid2D
        /// </summary>
        /// <param name="dimention"> Number of quads in each row and column</param>
        /// <param name="x">Start position of Grid2D in x axis (min X)</param>
        /// <param name="z">Start position of Grid2D in z axis (min Z)</param>
        /// <param name="quadSize">Size of each quad</param>
        public Grid2D(int dimention, float x, float z, float quadSize = 32)
        {
            this._Dimention = dimention;
            this._X = x;
            this._Z = z;
            this._QuadSize = quadSize;
            _Length = dimention * quadSize;
            this._Right = x + _Length;
            this._Top = z + _Length;

            Quads = new Quad[dimention, dimention];
        }

        /// <summary>
        /// Add transform to Grid
        /// </summary>
        /// <param name="transform">Transform to add</param>
        /// <returns>True if success, otherwise false</returns>
        public bool Add(Transform transform)
        {
            Quad quad = GetQuad(transform.position);
            if (quad != null)
            {
                if (!quad.Objetcs.Contains(transform))
                    quad.Objetcs.Add(transform);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove transform from Grid
        /// </summary>
        /// <param name="transform">Transform to remove</param>
        /// <returns>True if success, otherwise false</returns>
        public bool Remove(Transform transform)
        {
            Quad quad = GetQuad(transform.position, false);
            if (quad != null)
            {
                return quad.Objetcs.Remove(transform);
            }
            return false;
        }

        /// <summary>
        /// Specify given point is in witch quad index
        /// </summary>
        /// <param name="pos">Point to check</param>
        /// <param name="row"> Row index of quad (if found) </param>
        /// <param name="column"> Column index of quad (if found) </param>
        /// <returns>True if point is inside of this Grid2D, otherwise false</returns>
        private bool GetQuadIndex(ref Vector3 pos, out int row, out int column)
        {
            row = column = -1;
            float x = pos.x - _X;
            float z = pos.z - _Z;
            bool result = true;

            if (x < 0 || z < 0 || x > _Length || z > _Length) result = false;

            row = Mathf.FloorToInt(x / _QuadSize);
            column = Mathf.FloorToInt(z / _QuadSize);
            return result;
        }

        /// <summary>
        /// Get the quad that includes specified point
        /// </summary>
        /// <param name="pos">Point to check</param>
        /// <param name="create">Create quad if does not exist (created yet)</param>
        /// <returns> The quad that includes specified point</returns>
        public Quad GetQuad(Vector3 pos, bool create = true)
        {
            int row, column;
            if (!GetQuadIndex(ref pos, out row, out column))
                return null;

            Quad quad = Quads[row, column];
            if (quad == null && create)
            {
                quad = new Quad(row, column);
                Quads[row, column] = quad;
            }
            return quad;
        }

        /// <summary>
        /// Get list of quads that intersect specified circle
        /// </summary>
        /// <param name="pos">Position of circle</param>
        /// <param name="radius">Radius of circle</param>
        /// <param name="quads">The destination of the quads to copy quads</param>
        /// <param name="startIndex">The zero-based index in array at which copying begins.</param>
        /// <returns>number of quads that intersect specified circle</returns>                
        public int GetQuads(Vector3 pos, float radius, Quad[] quads, int startIndex = 0)
        {
            int row, column;
            GetQuadIndex(ref pos, out row, out column);

            int delta = Mathf.FloorToInt(radius / _QuadSize) + 1;
            int count = startIndex;

            for (int i = row - delta; i < row + delta; i++)
            {
                if (i >= 0 && i < _Dimention)
                {
                    for (int j = column - delta; j < column + delta; j++)
                    {
                        if (j >= 0 && j < _Dimention)
                        {
                            Quad q = Quads[i, j];
                            if (q != null)
                                quads[startIndex++] = q;

                        }
                    }
                }
            }
            return startIndex - count;
        }

        /// <summary>
        /// Remove all registered Transforms from Grid
        /// </summary>
        public void Clear()
        {
            foreach (var q in Quads)
            {
                if (q != null)
                    q.Objetcs.Clear();
            }
        }
    }
    #endregion

    #region Quad
    /// <summary>
    /// Defines a rectangular 2D space
    /// </summary>
    public class Quad
    {
        /// <summary>
        /// List of transforms inside quad
        /// </summary>
        public List<Transform> Objetcs { get; private set; }

        /// <summary>
        /// Row index of quad in Grid2D
        /// </summary>
        public int Row { get; private set; }
        /// <summary>
        /// Column index of quad in Grid2D
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Create a Quad
        /// </summary>
        /// <param name="row">Row index of quad in Grid2D</param>
        /// <param name="column">Column index of quad in Grid2D</param>
        public Quad(int row, int column)
        {
            this.Row = row;
            this.Column = column;

            this.Objetcs = new List<Transform>();
        }
    }
    #endregion
}