﻿using System.Diagnostics;
using Resharper.Annotations;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Represents a result of building of an Unit, null is a valid value of the <see cref="Value" />.
  /// </summary>
  public class BuildResult
  {
    [CanBeNull]
    public readonly object Value;

    [DebuggerStepThrough]
    public BuildResult([CanBeNull] object value) => Value = value;

    [DebuggerStepThrough]
    public override string ToString() => Value.AsLogString();
  }
}