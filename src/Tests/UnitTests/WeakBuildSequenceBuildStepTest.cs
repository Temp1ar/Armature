﻿using System;
using System.Linq;
using Armature;
using Armature.Common;
using Armature.Core;
using Armature.Framework;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;
using Tests.Functional;

namespace Tests.UnitTests
{
  public class WeakBuildSequenceBuildStepTest
  {
    [Test]
    public void should_match_build_sequence_weak()
    {
      var expected = new SingletonBuildAction();

      // --arrange
      var target = new UnitSequenceWeakMatchingBuildStep(Match.Type<IDisposableValue1>(null));
      var next = new UnitSequenceWeakMatchingBuildStep(Match.Type<IDisposable>(null));
      target.AddBuildStep(next);
      next.AddBuildAction(BuildStage.Cache, expected);

      // --act
      var actual = target
        .GetBuildActions(0, new[] {Unit.OfType<IDisposableValue1>(), Unit.OfType<OneDisposableCtorClass>(), Unit.OfType<IDisposable>()}.GetTail(0))
        .GetTopmostAction(BuildStage.Cache);

      // --assert
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void should_return_children_merged_actions()
    {
      // --arrange
      var buildStep1 = new AnyUnitBuildStep().AddBuildAction(BuildStage.Cache, CreateByReflectionBuildAction.Instance);
      var singletonAction = new SingletonBuildAction();
      var buildStep2 = new AnyUnitBuildStep().AddBuildAction(BuildStage.Cache, singletonAction);

      var target = new UnitSequenceWeakMatchingBuildStep(Match.Type<string>(null));
      target.AddBuildStep(buildStep1);
      target.AddBuildStep(buildStep2);

      // --act
      var actual = target.GetBuildActions(0, new[] {Unit.OfType<string>(), Unit.OfType<int>()}.GetTail(0));

      // --assert
      actual[BuildStage.Cache]
        .Should()
        .HaveCount(2)
        .And
        .Subject.Select(_ => _.BuildAction)
        .Should()
        .BeEquivalentTo(CreateByReflectionBuildAction.Instance, singletonAction);
    }
  }
}