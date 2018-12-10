const path = require("path");
var DuplicatePackageCheckerPlugin = require("duplicate-package-checker-webpack-plugin");
var BundleAnalyzerPlugin = require("webpack-bundle-analyzer").BundleAnalyzerPlugin;

module.exports = {
    entry: "./index.ts",
    mode: "development",
    devtool: "inline-source-map",
    module: {
        rules: [
            {
                test: require.resolve("./nonmodule-lib.js"),
                use: "imports-loader?$=jquery"
            },
            {
                test: /\.tsx?$/,
                use: "ts-loader",
                exclude: /node_modules/
            }
        ]
    },
    plugins: [
        new DuplicatePackageCheckerPlugin(),
        new BundleAnalyzerPlugin({
            analyzerMode: "static",
            openAnalyzer: false
        })
    ],
    output: {
        filename: "main.js",
        path: path.resolve(__dirname, "dist"),
        library: "EntryPoint"
        //libraryTarget: "var"
    }
};
