var JSHelpers = JSHelpers || {};

JSHelpers.setFocus = function (id) {
    const element = document.getElementById(id);
    console.log(id);
    element.focus();
}; 

JSHelpers.setClass = function (id, className) {
    const element = document.getElementById(id);
    element.classList.add(className);
};

JSHelpers.removeClass = function (id, className) {
    const element = document.getElementById(id);
    element.classList.remove(className);
};


JSHelpers.consoleLog = function (data) {
    console.log(data);
};

JSHelpers.setBtnFocus = function focusEditor(className) {
    document.getElementsByClassName(className)[0].querySelector("button").focus();
}