﻿using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Represents a result of building an until, null is a valid value of the <see cref="Value"/>.
  /// </summary>
  public class BuildResult
  {
    [CanBeNull]
    public readonly object Value;

    public BuildResult(object value)
    {
      Value = value;
    }

    public override string ToString()
    {
      return Value == null ? "null" : Value.ToString();
    }
  }
}