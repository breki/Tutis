export function calculator() {
    return 678;
}

export function onButtonClick() {
    document.getElementById("text-div").innerText = "Hello world " + calculator();
}
