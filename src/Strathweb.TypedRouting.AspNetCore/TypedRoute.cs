﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Strathweb.TypedRouting.AspNetCore
{
    public class TypedRoute : AttributeRouteModel
    {
        public TypedRoute(string template)
        {
            Template = template;
            Constraints = new List<IActionConstraintMetadata>();
            Filters = new FilterCollection();
            FilterTypes = new List<Type>();
        }

        public TypeInfo ControllerType { get; private set; }

        public MethodInfo ActionMember { get; private set; }

        public List<IActionConstraintMetadata> Constraints { get; private set; }

        public FilterCollection Filters { get; }

        public List<Type> FilterTypes { get; }

        public TypedRoute Controller<TController>()
        {
            ControllerType = typeof(TController).GetTypeInfo();
            return this;
        }

        public TypedRoute Action<T>(Expression<Action<T>> expression)
        {
            ActionMember = GetMethodInfoInternal(expression);
            ControllerType = ActionMember.DeclaringType.GetTypeInfo();
            return this;
        }

        public TypedRoute Action<T>(Expression<Func<T, Task>> expression)
        {
            ActionMember = GetMethodInfoInternal(expression);
            ControllerType = ActionMember.DeclaringType.GetTypeInfo();
            return this;
        }

        private static MethodInfo GetMethodInfoInternal(dynamic expression)
        {
            var method = expression.Body as MethodCallExpression;
            if (method != null)
                return method.Method;

            throw new ArgumentException("Expression is incorrect!");
        }

        public TypedRoute WithName(string name)
        {
            Name = name;
            return this;
        }

        public TypedRoute ForHttpMethods(params string[] methods)
        {
            Constraints.Add(new HttpMethodActionConstraint(methods));
            return this;
        }

        public TypedRoute WithConstraints(params IActionConstraintMetadata[] constraints)
        {
            Constraints.AddRange(constraints);
            return this;
        }

        public TypedRoute WithFilters(params IFilterMetadata[] filters)
        {
            foreach (var filter in filters)
            {
                Filters.Add(filter);
            }

            return this;
        }

        public TypedRoute WithFilter<T>() where T : IFilterMetadata
        {
            FilterTypes.Add(typeof(T));
            return this;
        }

        public TypedRoute WithAuthorizationPolicy(string authorizationPolicyName)
        {
            Filters.Add(new AuthorizeFilter(authorizationPolicyName));
            return this;
        }

        public TypedRoute WithAuthorizationPolicy(AuthorizationPolicy policy)
        {
            Filters.Add(new AuthorizeFilter(policy));

            return this;
        }
    }
}
