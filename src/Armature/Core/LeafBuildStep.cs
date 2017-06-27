﻿using System;
using System.Collections.Generic;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Base class for a build step which can match with the last unit in the build sequence only.
  /// Returns no build action if does not match building <see cref="UnitInfo"/>
  /// </summary>
  public abstract class LeafBuildStep : BuildStepBase
  {
    private readonly int _matchingWeight;

    protected LeafBuildStep(int matchingWeight)
    {
      _matchingWeight = matchingWeight;
    }

    public override MatchedBuildActions GetBuildActions(int inputMatchingWeight, ArrayTail<UnitInfo> matchingPattern)
    {
      if (matchingPattern.Length != 1) return null;

      var buildAction = GetBuildAction(matchingPattern.GetLastItem());

      return buildAction == null
        ? null
        : new MatchedBuildActions{
        {
          buildAction.BuildStage, 
          new List<WeightedBuildAction>{buildAction.BuildAction.WithWeight(inputMatchingWeight + _matchingWeight)}
        }};
    }

    //TODO: is it right, how to Remove such build step?
    public override bool Equals(IBuildStep obj)
    {
      return false;
    }

    protected abstract StagedBuildAction GetBuildAction(UnitInfo unitInfo);

    protected class StagedBuildAction
    {
      public readonly object BuildStage;
      public readonly IBuildAction BuildAction;

      public StagedBuildAction([NotNull] object buildStage, [NotNull] IBuildAction buildAction)
      {
        if (buildStage == null) throw new ArgumentNullException("buildStage");
        if (buildAction == null) throw new ArgumentNullException("buildAction");
        BuildStage = buildStage;
        BuildAction = buildAction;
      }
    }
  }
}