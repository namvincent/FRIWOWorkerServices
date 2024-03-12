var JSHelpers = JSHelpers || {};

JSHelpers.setMinMaxDate = function (id) {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();


    var yesterday = new Date(today);
    yesterday.setDate(today.getDate() - 2);
    var ddyes = yesterday.getDate();
    var mmyes = yesterday.getMonth() + 1;
    var yyyes = yesterday.getFullYear();
  
    

    if (dd < 10) {
        dd= '0' + dd
    }
    if (mm < 10) {
        mm = '0' + mm
    }


    if (ddyes < 10) {
        ddyes = '0' + ddyes
    }
    if (mmyes < 10) {
        mmyes = '0' + mmyes
    }


    today = yyyy + '-' + mm + '-' + dd;
 
    //tomorrow = yytmr + '-' + mmtmr + '-' + ddtmr;

    yesterday = yyyes + '-' + mmyes + '-' + ddyes;
    document.getElementById(id).value = today;
    //console.log(tomorrow, yesterday);
    document.getElementById(id).setAttribute("max", today);
    document.getElementById(id).setAttribute("min", yesterday);

  
   
};

JSHelpers.consoleLog = function (data) {
    console.log(data);
};