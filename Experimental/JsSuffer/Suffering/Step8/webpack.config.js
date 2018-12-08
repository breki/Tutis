const path = require("path");

module.exports = {
    entry: "./index.ts",
    mode: "development",
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: "ts-loader",
                exclude: /node_modules/
            }
            ]
    },
    output: {
        filename: "main.js",
        path: path.resolve(__dirname, "dist"),
        library: "EntryPoint",
        //libraryTarget: "var"
    }
};
