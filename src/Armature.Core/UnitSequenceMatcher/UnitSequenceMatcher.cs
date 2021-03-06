using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Common;
using Armature.Core.Logging;
using JetBrains.Annotations;

namespace Armature.Core.UnitSequenceMatcher
{
  /// <summary>
  ///   Base class implementing the logic of adding build actions
  /// </summary>
  public abstract class UnitSequenceMatcher : IUnitSequenceMatcher
  {
    private Dictionary<object, List<IBuildAction>> _buildActions;

    protected UnitSequenceMatcher(int weight) => Weight = weight;

    protected int Weight { get; }

    private Dictionary<object, List<IBuildAction>> LazyBuildAction
    {
      [DebuggerStepThrough] get => _buildActions ?? (_buildActions = new Dictionary<object, List<IBuildAction>>());
    }

    public abstract ICollection<IUnitSequenceMatcher> Children { get; }

    [CanBeNull]
    public abstract MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight);

    [DebuggerStepThrough]
    public IUnitSequenceMatcher AddBuildAction(object buildStage, IBuildAction buildAction)
    {
      LazyBuildAction
        .GetOrCreateValue(buildStage, () => new List<IBuildAction>())
        .Add(buildAction);
      return this;
    }

    public abstract bool Equals(IUnitSequenceMatcher other);

    [DebuggerStepThrough]
    [CanBeNull]
    protected MatchedBuildActions GetOwnActions(int matchingWeight)
    {
      if (_buildActions == null) return null;

      var result = new MatchedBuildActions();
      foreach (var pair in _buildActions)
        result.Add(pair.Key, pair.Value.Select(_ => _.WithWeight(matchingWeight)).ToList());

      return result;
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}<{1:n0}>", GetType().GetShortName(), Weight);
  }
}