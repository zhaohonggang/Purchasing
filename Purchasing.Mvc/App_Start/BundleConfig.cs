﻿using System.Web;
using System.Web.Optimization;

namespace Purchasing.Mvc
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui")
                .Include("~/Scripts/external/jquery/jquery-1.7.1.js")
                .Include("~/Scripts/external/jquery-ui/jquery-ui-1.8.17.js"));

            bundles.Add(new ScriptBundle("~/bundles/head")
                .IncludeDirectory("~/Scripts/public/head", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/common")
                .IncludeDirectory("~/Scripts/public/common", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/order")
                .IncludeDirectory("~/Scripts/public/order", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/landing")
                .Include("~/Scripts/public/single/Landing.js"));
            
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Css/main").Include(
                      "~/Css/Site.css",
                      "~/Css/jquery-ui-1.8.16.caes.css",
                      "~/Css/jquery.qtip.min.css",
                      "~/Css/jquery.tzCheckbox.css",
                      "~/Css/Datatables.css",
                      "~/Css/custom.css",
                      "~/Css/icons.css"));

            RegisterIndividualAssets(bundles);
            
            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }

        static void RegisterIndividualAssets(BundleCollection bundles)
        {
            //css
            bundles.Add(new StyleBundle("~/Css/single/chosen").Include("~/Css/single/chosen.css"));
            bundles.Add(new StyleBundle("~/Css/single/fileuploader/fileuploader").Include("~/Css/single/fileuploader/fileuploader.css"));
            bundles.Add(new StyleBundle("~/Css/single/orderrequest").Include("~/Css/single/orderrequest.css"));

            //scripts
            bundles.Add(new ScriptBundle("~/bundles/single/chosen").Include("~/Scripts/public/single/chosen.jquery.js"));
            bundles.Add(new ScriptBundle("~/bundles/single/chosen").Include("~/Scripts/public/single/chosen.jquery.js"));
        }
    }
}
