﻿using System;
using System.Reflection;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class CreateWithFactoryMethodBuildAction<TR> : IBuildAction
  {
    private readonly Func<UnitBuilder, TR> _factoryMethod;

    public CreateWithFactoryMethodBuildAction([NotNull] Func<UnitBuilder, TR> factoryMethod)
    {
      if (factoryMethod == null) throw new ArgumentNullException("factoryMethod");
      _factoryMethod = factoryMethod;
    }

    public void Execute(UnitBuilder unitBuilder)
    {
      if(unitBuilder.BuildResult == null)
        unitBuilder.BuildResult = new BuildResult(_factoryMethod(unitBuilder));
    }

    public void PostProcess(UnitBuilder unitBuilder)
    {}
    
    public override string ToString()
    {
      return string.Format("{0}: {1}", GetType().Name, _factoryMethod);
    }
  }

  public abstract class CreateWithFactoryMethodBuildAction : IBuildAction
  {
    public void Execute(UnitBuilder unitBuilder)
    {
      if (unitBuilder.BuildResult == null)
      {
        var result = Execute(unitBuilder, unitBuilder.GetValuesForParameters(GetMethod().GetParameters()));
        unitBuilder.BuildResult = new BuildResult(result);
      }
    }

    public void PostProcess(UnitBuilder unitBuilder)
    {}

    protected abstract MethodBase GetMethod();
    protected abstract object Execute(UnitBuilder unitBuilder, object[] values);

    public override string ToString()
    {
      return string.Format("{0}: {1}", GetType().Name, GetMethod());
    }
  }

  public class CreateWithFactoryMethodBuildAction<T1,  TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<UnitBuilder, T1, TR> _factoryMethod;

    public CreateWithFactoryMethodBuildAction([NotNull] Func<UnitBuilder, T1, TR> factoryMethod)
    {
      if (factoryMethod == null) throw new ArgumentNullException("factoryMethod");
      _factoryMethod = factoryMethod;
    }

    protected override MethodBase GetMethod()
    {
      return _factoryMethod.Method;
    }

    protected override object Execute(UnitBuilder unitBuilder, object[] values)
    {
      return _factoryMethod(unitBuilder, (T1) values[0]);
    }
  }

  public class CreateWithFactoryMethodBuildAction<T1, T2, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<UnitBuilder, T1, T2, TR> _factoryMethod;

    public CreateWithFactoryMethodBuildAction([NotNull] Func<UnitBuilder, T1, T2, TR> factoryMethod)
    {
      if (factoryMethod == null) throw new ArgumentNullException("factoryMethod");
      _factoryMethod = factoryMethod;
    }

    protected override MethodBase GetMethod()
    {
      return _factoryMethod.Method;
    }

    protected override object Execute(UnitBuilder unitBuilder, object[] values)
    {
      return _factoryMethod(unitBuilder, (T1) values[0], (T2) values[1]);
    }
  }

  public class CreateWithFactoryMethodBuildAction<T1, T2, T3, TR> : CreateWithFactoryMethodBuildAction
  {
    private readonly Func<UnitBuilder, T1, T2, T3, TR> _factoryMethod;

    public CreateWithFactoryMethodBuildAction([NotNull] Func<UnitBuilder, T1, T2, T3, TR> factoryMethod)
    {
      if (factoryMethod == null) throw new ArgumentNullException("factoryMethod");
      _factoryMethod = factoryMethod;
    }

    protected override MethodBase GetMethod()
    {
      return _factoryMethod.Method;
    }

    protected override object Execute(UnitBuilder unitBuilder, object[] values)
    {
      return _factoryMethod(unitBuilder, (T1) values[0], (T2) values[1], (T3) values[2]);
    }
  }
}