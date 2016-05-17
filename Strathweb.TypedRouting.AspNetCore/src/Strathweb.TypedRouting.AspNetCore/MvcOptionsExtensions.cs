﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Strathweb.TypedRouting.AspNetCore
{
    public static class MvcOptionsExtensions
    {
        public static TypedRoute GetRoute(this MvcOptions opts, string template, Action<TypedRoute> configSetup)
        {
            return AddRoute(template, configSetup).ForHttpMethods("GET");
        }

        public static TypedRoute PostRoute(this MvcOptions opts, string template, Action<TypedRoute> configSetup)
        {
            return AddRoute(template, configSetup).ForHttpMethods("POST");
        }

        public static TypedRoute PutRoute(this MvcOptions opts, string template, Action<TypedRoute> configSetup)
        {
            return AddRoute(template, configSetup).ForHttpMethods("PUT");
        }

        public static TypedRoute DeleteRoute(this MvcOptions opts, string template, Action<TypedRoute> configSetup)
        {
            return AddRoute(template, configSetup).ForHttpMethods("DELETE");
        }

        public static TypedRoute TypedRoute(this MvcOptions opts, string template, Action<TypedRoute> configSetup)
        {
            return AddRoute(template, configSetup);
        }

        private static TypedRoute AddRoute(string template, Action<TypedRoute> configSetup)
        {
            var route = new TypedRoute(template);
            configSetup(route);

            if (TypedRoutingApplicationModelConvention.Routes.ContainsKey(route.ControllerType))
            {
                var controllerActions = TypedRoutingApplicationModelConvention.Routes[route.ControllerType];
                controllerActions.Add(route);
            }
            else
            {
                var controllerActions = new List<TypedRoute> { route };
                TypedRoutingApplicationModelConvention.Routes.Add(route.ControllerType, controllerActions);
            }

            return route;
        }

        public static void EnableTypedRouting(this MvcOptions opts)
        {
            opts.Conventions.Add(new TypedRoutingApplicationModelConvention());
        }
    }
}
