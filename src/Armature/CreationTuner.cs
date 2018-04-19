﻿using Armature.Core;
using Armature.Core.BuildActions.Creation;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;
using Resharper.Annotations;

namespace Armature
{
  public class CreationTuner<T> : UnitSequenceExtensibility
  {
    private readonly object _token;

    public CreationTuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher, [CanBeNull] object token) : base(unitSequenceMatcher) => _token = token;

    /// <summary>
    /// Specifies that unit of type <typeparamref name="T"/> should be created using default creation strategy
    /// specified in <see cref="Default.CreationBuildAction"/> 
    /// </summary>
    public Tuner ByDefault()
    {
      var sequenceMatcher = new StrictUnitSequenceMatcher(Match.Type<T>(_token));

      UnitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(sequenceMatcher)
        .AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
      return new Tuner(sequenceMatcher);
    }
    
    /// <summary>
    /// Specifies that unit of type <typeparamref name="T"/> should be created using reflection
    /// </summary>
    /// <returns></returns>
    public Tuner ByReflection()
    {
      var sequenceMatcher = new StrictUnitSequenceMatcher(Match.Type<T>(_token));

      UnitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(sequenceMatcher)
        .AddBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);
      return new Tuner(sequenceMatcher);
    }
  }
}