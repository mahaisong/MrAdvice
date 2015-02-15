﻿#region Mr. Advice
// Mr. Advice
// A simple post build weaving package
// https://github.com/ArxOne/MrAdvice
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion
namespace ArxOne.MrAdvice.Reflection.Groups
{
    using System.Collections.Generic;
    using System.Linq;
    using Mono.Cecil;

    /// <summary>
    /// Represents a reflection group.
    /// Can be assembly, module, type, [property], method, parameter, [return value]
    /// </summary>
    internal abstract class ReflectionNode
    {
        private ReflectionNode _parent;

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent, or null if top-level.
        /// </value>
        public ReflectionNode Parent
        {
            get
            {
                if (_parent == null)
                    _parent = LoadParent();
                return _parent;
            }
        }

        /// <summary>
        /// Loads the parent.
        /// </summary>
        /// <returns></returns>
        protected abstract ReflectionNode LoadParent();

        private IList<ReflectionNode> _children;

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public IEnumerable<ReflectionNode> Children
        {
            get
            {
                if (_children == null)
                    _children = LoadChildren().ToArray();
                return _children;
            }
        }

        /// <summary>
        /// Loads the children.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<ReflectionNode> LoadChildren();

        /// <summary>
        /// Gets the custom attributes at this level.
        /// </summary>
        /// <value>
        /// The custom attributes.
        /// </value>
        public abstract IEnumerable<CustomAttribute> CustomAttributes { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is generic.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is generic; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsGeneric { get { return false; } }

        /// <summary>
        /// Gets the method matching this node.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public virtual MethodDefinition Method { get { return null; } }

        /// <summary>
        /// Gets the self and ancestors.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReflectionNode> GetSelfAndAncestors()
        {
            for (var ancestor = this; ancestor != null; ancestor = ancestor.Parent)
                yield return ancestor;
        }

        /// <summary>
        /// Gets all the children (recursively).
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReflectionNode> GetAllChildren()
        {
            var children = Children.ToArray();
            return children.Concat(children.SelectMany(c => c.GetAllChildren()));
        }

        /// <summary>
        /// Gets the ancestors to all children (recursively).
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReflectionNode> GetAncestorsToChildren()
        {
            return GetSelfAndAncestors().Reverse().Concat(GetAllChildren());
        }

        /// <summary>
        /// Determines whether this member is in a generic tree.
        /// </summary>
        /// <returns></returns>
        public bool IsAnyGeneric()
        {
            return GetSelfAndAncestors().Any(n => n.IsGeneric);
        }
    }
}