var path = require("path");
var DuplicatePackageCheckerPlugin = require("duplicate-package-checker-webpack-plugin");
var BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;

module.exports = {
    entry: "./index.ts",
    mode: "development",
    devtool: "inline-source-map",
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: "ts-loader",
                exclude: /node_modules/
            }
            ]
    },
    resolve: {
        alias: {
            leaflet: path.resolve(__dirname, 'node_modules/leaflet')
        }
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
        library: "EntryPoint",
        //libraryTarget: "var"
    }
};
