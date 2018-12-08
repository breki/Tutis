var Calculator = require("./lib.js");

export function onButtonClick() {
    document.getElementById("text-div").innerText = "Hello world "
        + Calculator.calculator();
}
