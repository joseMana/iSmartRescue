using System.Web;
using System.Web.Optimization;

namespace iSmartRescue
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/vendor/jquery/jquery.min.js",
                        "~/vendor/bootstrap/js/bootstrap.bundle.min.js",
                        "~/vendor/jquery-easing/jquery.easing.min.js", 
                        "~/js/sb-admin-2.min.js",
                        "~/vendor/chart.js/Chart.min.js", 
                        "~/js/demo/chart-area-demo.js",
                        "~/js/demo/chart-pie-demo.js"));


               bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/css/sb-admin-2.min.css",
                      "~/font.css"));
            bundles.Add(new StyleBundle("~/Content/fontawesome").Include(
                      "~/vendor/fontawesome-free/css/all.min.css"));

        }
    }
}
