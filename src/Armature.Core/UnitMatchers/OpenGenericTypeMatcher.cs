﻿using System.Diagnostics;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Matches <see cref="UnitInfo" /> with an open generic type
  /// </summary>
  public class OpenGenericTypeMatcher : UnitInfoMatcher
  {
    [DebuggerStepThrough]
    public OpenGenericTypeMatcher(UnitInfo unitInfo) : base(unitInfo) { }

    public override bool Matches(UnitInfo unitInfo)
    {
      var unitType = unitInfo.GetUnitTypeSafe();
      return unitType != null && unitType.IsGenericType && Equals(unitType.GetGenericTypeDefinition(), UnitInfo.Id);
    }
  }
}