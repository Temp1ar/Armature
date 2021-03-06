﻿using System;
using System.Linq;
using Armature;
using Armature.Core;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitSequenceMatcher;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.Functional
{
  public class BuildAllTest
  {
    [Test]
    public void should_return_two_implementation_of_one_interface()
    {
      // --arrange
      var target = CreateTarget();

      target.Treat<IDisposable>().AsCreated<SampleType1>();
      target.Treat<IDisposable>().AsCreated<SampleType2>();

      // --act
      var actual = target.BuildAllUnits(Unit.OfType<IDisposable>());

      // --assert
      actual.Should().HaveCount(2);
      actual.Should().ContainSingle(_ => _ is SampleType1);
      actual.Should().ContainSingle(_ => _ is SampleType2);
    }

    [Test]
    public void should_throw_if_more_than_one_build_stage_involved()
    {
      // --arrange
      var target = CreateTarget();

      target.Treat<IDisposable>().AsCreated<SampleType1>();
      target.Treat<IDisposable>().AsCreated<SampleType2>();
      target.Treat<IDisposable>().AsSingleton(); // involve BuildStage.Cache for first level unit

      // --act
      Action actual = () => target.BuildAllUnits(Unit.OfType<IDisposable>());

      // --assert
      actual.Should().ThrowExactly<ArmatureException>();
    }

    [Test]
    public void more_than_one_build_stage_can_be_used_for_redirected_type()
    {
      // --arrange
      var target = CreateTarget();

      target.Treat<IDisposable>().AsCreated<SampleType1>().AsSingleton();
      target.Treat<IDisposable>().AsCreated<SampleType2>();

      // --precondition
      var precondition = target.BuildAllUnits(Unit.OfType<IDisposable>());
      precondition.Should().HaveCount(2);
      precondition.Should().ContainSingle(_ => _ is SampleType1);
      precondition.Should().ContainSingle(_ => _ is SampleType2);
      var expectedSingleton = precondition.Single(_ => _ is SampleType1);

      // --act
      var actual = target.BuildAllUnits(Unit.OfType<IDisposable>());
      actual.Should().HaveCount(2);
      actual.Should().ContainSingle(_ => _ is SampleType1);
      actual.Should().ContainSingle(_ => _ is SampleType2);
      actual.Single(_ => _ is SampleType1).Should().BeSameAs(expectedSingleton);
      actual.Single(_ => _ is SampleType2).Should().NotBeSameAs(precondition.Single(_ => _ is SampleType2));
    }

    private static Builder CreateTarget() =>
      new Builder(BuildStage.Cache, BuildStage.Create)
      {
        new AnyUnitSequenceMatcher
        {
          new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
            .AddBuildAction(BuildStage.Create, GetLongestConstructorBuildAction.Instance)
        }
      };

    private class SampleType1 : IDisposable
    {
      public void Dispose()
      {
      }
    }

    private class SampleType2 : IDisposable
    {
      public void Dispose()
      {
      }
    }
  }
}