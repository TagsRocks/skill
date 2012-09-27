using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    /// <summary>
    /// Defines an interface for objects that can generate random values 
    /// </summary>
    public interface IRandomService
    {
        /// <summary>
        /// Returns a random float number between and min [inclusive] and max [inclusive].
        /// </summary>
        /// <param name="min">min value</param>
        /// <param name="max">max value</param>
        /// <returns>a random float number between and min [inclusive] and max [inclusive].</returns>
        float Range(float min, float max);
    }

    /// <summary>    
    /// Select random child by chance for execution and continue executing that until result of chlid be Running. 
    /// if result is Failure or success, selected another random child on next exevution
    /// </summary>
    public class RandomSelector : Composite
    {
        private class UnityRandomService : IRandomService
        {
            public float Range(float min, float max)
            {
                return UnityEngine.Random.Range(min, max);
            }
        }

        private static IRandomService _RandomService;

        /// <summary>        
        /// Gets or sets random value generation service
        /// </summary>
        /// <remarks>
        /// The main reason to write this property is :
        /// as for simulation BehaviorTree in Skill Studio i use Skill Dll, to avoid writing duplicate code for BehaviorTree (one in Skill Dll and one in Skill Studio)
        /// i could't use UnityEngine.Random class in Skill Studio application. so i create IRandomService interface to change random generation
        /// algorithm in Skill Studio application. Although you can change random generation algorith (:D).
        /// </remarks>
        public static IRandomService RandomService
        {
            get
            {
                if (_RandomService == null)
                    _RandomService = new UnityRandomService();
                return _RandomService;
            }
            set { _RandomService = value; }
        }


        private float _TotalWeight;// sum of weights of child behaviors

        /// <summary>
        /// CompositeType
        /// </summary>
        public override CompositeType CompositeType { get { return AI.CompositeType.Random; } }

        /// <summary>
        /// Create an instance of RandomSelector
        /// </summary>
        /// <param name="name">Name of Behavior node</param>
        public RandomSelector(string name)
            : base(name)
        {
            _TotalWeight = 0;
        }

        /// <summary>
        /// Add child . Remember to set weight of child before call this function
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parameters"></param>
        public override void Add(Behavior child, BehaviorParameterCollection parameters = null)
        {
            _TotalWeight += child.Weight;
            base.Add(child, parameters);
        }

        /// <summary>
        /// Remove all children
        /// </summary>
        public override void RemoveAll()
        {
            base.RemoveAll();
            _TotalWeight = 0;
        }

        /// <summary>
        /// Remove specified child from children
        /// </summary>
        /// <param name="child">behavior child to remove</param>
        /// <returns>true for success, otherwise false</returns>
        public override bool Remove(Behavior child)
        {
            bool r = base.Remove(child);
            if (r)
            {
                _TotalWeight -= child.Weight;
                if (_TotalWeight < 0) _TotalWeight = 0;
            }
            return r;
        }

        /// <summary>
        /// Behave
        /// </summary>
        /// <param name="state">State od BehaviorTree</param>
        /// <returns>Result</returns>
        protected override BehaviorResult Behave(BehaviorState state)
        {
            if (RunningChildIndex < 0)
                RunningChildIndex = GetRandomIndex();// pick random node
            BehaviorResult result = BehaviorResult.Failure;
            BehaviorContainer node = this[RunningChildIndex];
            state.Parameters = node.Parameters;
            result = node.Behavior.Trace(state);
            if (result != BehaviorResult.Running)
                RunningChildIndex = -1;
            return result;
        }

        /// <summary>
        /// Select random child by chance
        /// </summary>
        /// <returns>Index of selected child</returns>
        private int GetRandomIndex()
        {
            float rnd = RandomService.Range(0.0f, _TotalWeight);
            float sum = 0;
            for (int i = 0; i < ChildCount; i++)
            {
                sum += this[i].Behavior.Weight;
                if (sum >= rnd) return i;
            }
            return 0;
        }
    }
}
