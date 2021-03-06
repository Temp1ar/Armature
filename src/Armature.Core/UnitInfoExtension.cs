﻿using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Armature.Core
{
  public static class UnitInfoExtension
  {
    /// <summary>
    ///   Returns a <see cref="Type" /> if <see cref="UnitInfo.Id" /> is a type, otherwise throws exception.
    /// </summary>
    [DebuggerStepThrough]
    [NotNull]
    public static Type GetUnitType([NotNull] this UnitInfo unitInfo) =>
      unitInfo != null ? (Type)unitInfo.Id : throw new ArgumentNullException(nameof(unitInfo));

    /// <summary>
    ///   Returns a <see cref="Type" /> if <see cref="UnitInfo.Id" /> is a type, otherwise null.
    /// </summary>
    [DebuggerStepThrough]
    [CanBeNull]
    public static Type GetUnitTypeSafe([NotNull] this UnitInfo unitInfo)
    {
      if (unitInfo == null) throw new ArgumentNullException(nameof(unitInfo));

      return unitInfo.Id as Type;
    }
  }
}