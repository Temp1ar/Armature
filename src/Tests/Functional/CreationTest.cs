﻿using Armature;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitSequenceMatcher;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional
{
  public class CreationTest
  {
    [Test]
    public void should_add_default_creation_strategy()
    {
      // --arrange
      var target = CreateTarget();
      target
        .Treat<Subject>()
        .AsIs();

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().BeOfType<Subject>();
    }

    [Test]
    public void should_create_type_on_building_interface()
    {
      // --arrange
      var target = CreateTarget();
      target
        .Treat<ISubject1>()
        .As<Subject>();

      // --act
      var actual = target.Build<ISubject1>();

      // --assert
      actual.Should().BeOfType<Subject>();
    }

    [Test]
    public void should_use_factory_method()
    {
      var expected = new Subject();

      // --arrange
      var target = CreateTarget();
      target
        .Treat<Subject>()
        .CreatedBy(_ => expected);

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().Be(expected);
    }

    [Test]
    public void should_autowire_value_into_factory_method()
    {
      var expected = new Subject();
      const string expectedString = "expected397";

      // --arrange
      var target = CreateTarget();

      target
        .Treat<Subject>()
        .CreatedBy<string>(
          (_, value) =>
            {
              value.Should().Be(expectedString);
              return expected;
            })
        .UsingParameters(expectedString);
      
      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().Be(expected);
    }

    [Test]
    public void creation_build_action_should_be_added_only_once()
    {
      // --arrange
      var target = CreateTarget();

      target
        .Treat<ISubject1>()
        .As<Subject>(AddCreateBuildAction.No);

      target
        .Treat<ISubject2>()
        .As<Subject>(AddCreateBuildAction.No);

      target
        .Treat<Subject>()
        .AsIs()
        .AsSingleton();

      // --act
      var actual1 = target.Build<ISubject1>();
      var actual2 = target.Build<ISubject2>();
      var actual3 = target.Build<Subject>();

      // --assert
      actual1.Should().BeSameAs(actual2);
      actual1.Should().BeSameAs(actual3);
    }

    [Test]
    public void should_use_creation_strategy_registered_with_token()
    {
      const string token = "token";
      var expected = new Subject();
      
      // --arrange
      var target = CreateTarget();

      target
        .Treat<Subject>()
        .AsIs();

      target
        .Treat<Subject>(token)
        .CreatedBy(_ => expected);


      // --act
      var actual = target.UsingToken(token).Build<Subject>();

      // --assert
      actual.Should().Be(expected);
    }
    
    [Test]
    public void should_pass_token_to_creation_strategy()
    {
      const string token = "token";
      var createdByFactory = new Subject();
      
      // --arrange
      var target = CreateTarget();

      target
        .Treat<ISubject1>()
        .As<Subject>(token);

      target
        .Treat<Subject>(token)
        .AsIs();
      
      target
        .Treat<Subject>()
        .CreatedBy(_ => createdByFactory);

      // --act
      var actual = target.Build<ISubject1>();

      // --assert
      actual.Should().BeOfType<Subject>().And.Should().NotBeSameAs(createdByFactory);
    }
    
    [Test]
    public void should_use_instance()
    {
      var expected = new Subject();
      
      // --arrange
      var target = CreateTarget();

      target
        .Treat<Subject>()
        .AsInstance(expected);
      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().BeSameAs(expected);
    }
    
    private static Builder CreateTarget()
    {
      var treatAll = new AnyUnitSequenceMatcher
      {
        // inject into constructor
        new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              new GetInjectPointConstructorBuildAction(), // constructor marked with [Inject] attribute has more priority
              GetLongesConstructorBuildAction.Instance // constructor with largest number of parameters has less priority
            })
      };

      var container = new Builder(new[] {BuildStage.Cache, BuildStage.Create});
      container.Children.Add(treatAll);
      return container;
    }

    private interface ISubject1{}
    private interface ISubject2{}
    private class Subject : ISubject1, ISubject2{}
  }
}