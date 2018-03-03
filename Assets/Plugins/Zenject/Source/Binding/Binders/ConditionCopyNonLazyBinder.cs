using System;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class ConditionCopyNonLazyBinder : CopyNonLazyBinder
    {
        public ConditionCopyNonLazyBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        public CopyNonLazyBinder When(BindingCondition condition)
        {
            BindInfo.Condition = condition;
            return this;
        }

        public CopyNonLazyBinder WhenInjectedIntoInstance(object instance)
        {
            return When(r => ReferenceEquals(r.ObjectInstance, instance));
        }

        public CopyNonLazyBinder WhenInjectedIntoParentInstance(object instance)
        {
            return When(r => ReferenceEquals(r.ParentContext.ObjectInstance, instance));
        }

        public CopyNonLazyBinder WhenNotInjectedIntoParentInstance(object instance)
        {
            return When(r => !ReferenceEquals(r.ParentContext.ObjectInstance, instance));
        }

        public CopyNonLazyBinder WhenNotInjectedIntoInstance(object instance)
        {
            return When(r => !ReferenceEquals(r.ObjectInstance, instance));
        }

        public CopyNonLazyBinder WhenInjectedInto(params Type[] targets)
        {
            return When(r => targets.Where(x => r.ObjectType != null && r.ObjectType.DerivesFromOrEqual(x)).Any());
        }

        public CopyNonLazyBinder WhenInjectedInto<T>()
        {
            return When(r => r.ObjectType != null && r.ObjectType.DerivesFromOrEqual(typeof(T)));
        }

	    public CopyNonLazyBinder WhenNotInjectedInto<T>()
	    {
		    return When(r => r.ObjectType == null || !r.ObjectType.DerivesFromOrEqual(typeof(T)));
	    }

	    public CopyNonLazyBinder WhenInjectedIntoContextWithParent<T>()
        {
            return When(r => r.ParentContext.ObjectType != null && r.ParentContext.ObjectType.DerivesFromOrEqual(typeof(T)));
        }
    }
}
