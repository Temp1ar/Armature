﻿using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Core.Logging;
using JetBrains.Annotations;

namespace Armature.Core.BuildActions.Property
{
  /// <summary>
  ///   "Builds" a constructor Unit of the currently building Unit with provided names
  /// </summary>
  public class GetPropertyByNameBuildAction : IBuildAction
  {
    private readonly IReadOnlyCollection<string> _names;

    public GetPropertyByNameBuildAction([NotNull] params string[] names)
    {
      if (names is null || names.Length == 0) throw new ArgumentNullException(nameof(names));

      _names = names;
    }

    public void Process(IBuildSession buildSession)
    {
      var unitType = buildSession.GetUnitUnderConstruction().GetUnitType();

      var properties =
        _names.Select(
            name =>
              {
                var property = unitType.GetProperty(name);
                if (property == null)
                  throw new ArmatureException(string.Format("There is no property {0} in type {1}", _names, unitType.ToLogString()));

                return property;
              })
          .ToArray();

      buildSession.BuildResult = new BuildResult(properties);
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", _names));
  }
}