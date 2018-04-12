﻿using System.Linq;
using System.Reflection;
using Armature.Logging;

namespace Armature.Core.BuildActions.Property
{
  /// <summary>
  /// Builds value to inject by using <see cref="PropertyInfo.PropertyType"/> and <see cref="InjectAttribute.InjectionPointId"/> as token
  /// </summary>
  public class CreatePropertyValueForInjectPointBuildAction : IBuildAction
  {
    public static readonly IBuildAction Instance = new CreatePropertyValueForInjectPointBuildAction();

    private CreatePropertyValueForInjectPointBuildAction()
    {
    }

    public void Process(IBuildSession buildSession)
    {
      var propertyInfo = (PropertyInfo)buildSession.GetUnitUnderConstruction().Id;

      var attribute = propertyInfo
        .GetCustomAttributes<InjectAttribute>()
        .SingleOrDefault();

      if (attribute == null)
        Log.WriteLine(LogLevel.Info, "{0}{{{1}}}", this, "No Property marked with InjectAttribute");
      else
      {
        var unitInfo = new UnitInfo(propertyInfo.PropertyType, attribute.InjectionPointId);
        buildSession.BuildResult = buildSession.BuildUnit(unitInfo);
      }
    }

    public void PostProcess(IBuildSession buildSession) {  }
  }
}