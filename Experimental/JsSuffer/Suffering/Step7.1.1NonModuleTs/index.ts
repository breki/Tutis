// @ts-ignore
import mylib = require("exports-loader?MyLib,MyLib.calculator!./nonmodule-lib.js");

const ProperMyLib: any = mylib;

export function onButtonClick() {
    const labelEl = document.getElementById("text-div");
    if (labelEl)
        labelEl.innerText = "Hello world "
            + ProperMyLib.MyLib.calculator();
}
