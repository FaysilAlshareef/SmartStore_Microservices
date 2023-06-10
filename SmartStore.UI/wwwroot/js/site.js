// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var quantityInStockInput = document.getElementById("quantityInStock");
var quantityInCartInput = document.getElementById("quantityInCart");


quantityInCartInput.addEventListener("change", function () {
    quantityInStockInput.innerText = Number(quantityInCartInput.max) - quantityInCartInput.value;

});
quantityInCartInput.addEventListener("keyup", calcQuantity);

function calcQuantity() {
    debugger
    if (quantityInCartInput.value < quantityInCartInput.max) {
        quantityInStockInput.innerText = Number(quantityInCartInput.max) - quantityInCartInput.value;

    }

}

