//require("./nonmodule-lib.js");
var mylib = require("exports-loader?MyLib,MyLib.calculator!./nonmodule-lib.js");

export function onButtonClick() {
    document.getElementById("text-div").innerText = "Hello world "
        + mylib.MyLib.calculator();
}
