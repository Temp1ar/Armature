﻿using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Property;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.Parameters;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;
using JetBrains.Annotations;

namespace Armature
{
  public class Tuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public Tuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher) : base(unitSequenceMatcher) { }

    /// <summary>
    ///   Provided values will be used to inject the into created object. See <see cref="ForParameter" /> for details
    /// </summary>
    public Tuner UsingParameters(params object[] values)
    {
      if (values == null || values.Length == 0)
        throw new Exception("null");

      foreach (var parameter in values)
        if (parameter is IParameterValueBuildPlan buildPlan)
          buildPlan.Apply(UnitSequenceMatcher);
        else if (parameter is IBuildPlan)
          throw new ArmatureException("IParameterValueBuildPlan or plain object value expected");
        else
          UnitSequenceMatcher
            .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(new ParameterByValueMatcher(parameter), InjectPointMatchingWeight.WeakTypedParameter))
            .AddBuildAction(BuildStage.Create, new SingletonBuildAction(parameter));

      return this;
    }

    /// <summary>
    ///   Provided values will be injected into properties of the created object.  See <see cref="ForProperty" /> for details.
    ///   Also value can be a build plan returned by one of the method of the <see cref="Property" /> class, which specifies properties to inject dependencies.
    /// </summary>
    public Tuner InjectProperty(params object[] values)
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Initialize, InjectIntoPropertiesBuildAction.Instance);

      foreach (var value in values)
        if (value is IPropertyValueBuildPlan buildPlan)
          buildPlan.Apply(UnitSequenceMatcher);
        else if (value is IBuildPlan)
          throw new ArmatureException("IPropertyValueBuildPlan or plain object value expected");
        else
          UnitSequenceMatcher
            .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(new PropertyByValueMatcher(value), InjectPointMatchingWeight.WeakTypedParameter))
            .AddBuildAction(BuildStage.Create, new SingletonBuildAction(value));

      return this;
    }

    /// <summary>
    ///   Register Unit as an eternal singleton <see cref="SingletonBuildAction" /> for details
    /// </summary>
    public void AsSingleton() => UnitSequenceMatcher.AddBuildAction(BuildStage.Cache, new SingletonBuildAction());

    /// <summary>
    ///   Instantiate a Unit using a constructor with the biggest number of parameters
    /// </summary>
    public Tuner UsingLongestConstructor()
    {
      UnitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(ConstructorMatcher.Instance))
        .AddBuildAction(BuildStage.Create, GetLongestConstructorBuildAction.Instance);
      return this;
    }

    /// <summary>
    ///   Instantiate a Unit using a constructor marked with <see cref="InjectAttribute" />(<paramref name="injectionPointId" />)
    /// </summary>
    public Tuner UsingInjectPointConstructor(object injectionPointId)
    {
      UnitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(ConstructorMatcher.Instance))
        .AddBuildAction(BuildStage.Create, new GetInjectPointConstructorBuildAction(injectionPointId));
      return this;
    }

    /// <summary>
    ///   Instantiate a Unit using constructor without parameters
    /// </summary>
    public Tuner UsingParameterlessConstructor() => UsingConstructorWithParameters();

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters provided as generic arguments
    /// </summary>
    public Tuner UsingConstructorWithParameters<T1>() => UsingConstructorWithParameters(typeof(T1));

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters provided as generic arguments
    /// </summary>
    public Tuner UsingConstructorWithParameters<T1, T2>() => UsingConstructorWithParameters(typeof(T1), typeof(T2));

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters provided as generic arguments
    /// </summary>
    public Tuner UsingConstructorWithParameters<T1, T2, T3>() => UsingConstructorWithParameters(typeof(T1), typeof(T2), typeof(T3));

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters provided as generic arguments
    /// </summary>
    public Tuner UsingConstructorWithParameters<T1, T2, T3, T4>() => UsingConstructorWithParameters(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters as provided in <paramref name="parameterTypes" />
    /// </summary>
    public Tuner UsingConstructorWithParameters(params Type[] parameterTypes)
    {
      UnitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(ConstructorMatcher.Instance))
        .AddBuildAction(BuildStage.Create, new GetConstructorByParameterTypesBuildAction(parameterTypes));
      return this;
    }

    /// <summary>
    ///   Doing the same as <see cref="BuildPlansCollectionExtension.Building{T}" /> but w/o breaking fluent syntax
    /// </summary>
    public Tuner BuildingWhich([NotNull] Action<SequenceTuner> tuneAction)
    {
      if (tuneAction is null) throw new ArgumentNullException(nameof(tuneAction));

      tuneAction(new SequenceTuner(UnitSequenceMatcher));
      return this;
    }
  }
}