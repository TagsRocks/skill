using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill.AI
{
    /// <summary>
    /// Defines type of Composite nodes that implemented in this library
    /// </summary>
    public enum CompositeType
    {
        /// <summary>
        /// run one child to finish after the other. If one or multiple fail the whole sequence fails, too.
        /// Without a reset or without finishing the last child node a sequence stores the last running child to immediately return to it on the next update.
        /// </summary>
        Sequence,
        /// <summary>
        /// visit all of their children during each traversal.
        /// A pre-specified number of children needs to fail to make the concurrent node fail, too.
        /// Instead of running its child nodes truly in parallel to each other there might be a specific traversal order which can be exploited when adding conditions
        /// to a concurrent node because an early failing condition prevents its following concurrent siblings from running.
        /// </summary>
        Concurrent,
        /// <summary>
        /// select a random child by chance for execution.
        /// </summary>
        Random,
        /// <summary>
        /// On each traversal priority selectors check which child to run in priority order until the first one succeeds or returns that it is running.
        /// One option is to call the last still running node again during the next behavior tree update. The other option is to always restart traversal
        /// from the highest priority child and implicitly cancel the last running child behavior if it isn’t chosen immediately again.
        /// </summary>
        Priority,
        /// <summary>
        /// Loops are like sequences but they loop around when reaching their last child during their traversal instead of returning to their parent node like sequence node do.
        /// </summary>
        Loop,
    }
}
