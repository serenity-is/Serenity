import { build, npmCopy } from "@serenity-is/tsbuild";

npmCopy([
    "bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css",
    "bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js",
    "chart.js/dist/chart.umd.js",
    "daterangepicker/daterangepicker.css",
    "daterangepicker/daterangepicker.js",
    "flatpickr/dist/flatpickr.css",
    "jquery-knob/dist/jquery.knob.min.js",
    "jquery-sparkline/jquery.sparkline.min.js",
    "jvectormap-content/world-mill.js",
    "jvectormap-next/jquery-jvectormap.css",
    "jvectormap-next/jquery-jvectormap.min.js",
    "moment/min/moment.min.js"
]);

await build({});