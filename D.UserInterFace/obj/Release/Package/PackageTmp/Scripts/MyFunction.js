
function getDateDiff(date1, date2, interval) {
    var second = 1000,
    minute = second * 60,
    hour = minute * 60,
    day = hour * 24,
    week = day * 7;
    date1 = new Date(date1).getTime();
    date2 = new Date(date2).getTime();
    var timediff = date2 - date1;
    if (isNaN(timediff)) return NaN;
    switch (interval) {
        case "MMYY":
            {
                var OneDay = 1000 * 60 * 60 * 24;
                var days = Math.floor(timediff / OneDay);
                var months = Math.floor(days / 30);
                var years = Math.floor(months / 12);
                return years + "." + months
            }
        case "years":
            return date2.getFullYear() - date1.getFullYear();
        case "months":
            return ((date2.getFullYear() * 12 + date2.getMonth()) - (date1.getFullYear() * 12 + date1.getMonth()));
        case "weeks":
            return Math.floor(timediff / week);
        case "days":
            return Math.floor(timediff / day);
        case "hours":
            return Math.floor(timediff / hour);
        case "minutes":
            return Math.floor(timediff / minute);
        case "seconds":
            return Math.floor(timediff / second);
        default:
            return undefined;
    }
}

function JsDate(jsDate,format) {
    var value = new Date(
                            parseInt((jsDate).replace(/(^.*\()|([+-].*$)/g, ''))
                            );
    var day = value.getDate();
    var month = value.getMonth() + 1;
    var year = value.getFullYear();
    switch (format) {
        case "dd/mm/yy":
            return day + "/" + month + "/" + year;
        case "mm/dd/yy":
            return month + "/" + day + "/" + year;
        case "yy/mm/dd":
            return year + "/" + month + "/" + day;
        case "mm/yy": {
            var m = month < 10 ? '0' + month : month;
            return m + "/" + year;
        }
            return month + "/" + year;
        default:
            return day + "/" + month + "/" + year;
    }
}

function getCurrentFiscalYear() {
    //get current date
    var today = new Date();
    //get current month
    var curMonth = today.getMonth(); //get current month
    var fiscalYr = "";
    if (curMonth > 3) { //
        var nextYr1 = (today.getFullYear() + 1).toString();
        fiscalYr = today.getFullYear().toString() + "-" + nextYr1.charAt(2) + nextYr1.charAt(3);
    }
    else {
        var nextYr2 = today.getFullYear().toString();
        fiscalYr = (today.getFullYear() - 1).toString() + "-" + nextYr2.charAt(2) + nextYr2.charAt(3);
    }
    return fiscalYr;
}

function getToday() {
    //get current date
    var today = new Date();
    var day = today.getDate() < 10 ? '0' + today.getDate() : today.getDate();
    var month = (today.getMonth() + 1) < 10 ? '0' + (today.getMonth() + 1) : (today.getMonth() + 1);
    var year = today.getFullYear();
    return day + "-" + month + "-" + year;;
}

function getDateTime() {
    //get current date
    var month= ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
    var today = new Date();
    var day = today.getDate() < 10 ? '0' + today.getDate() : today.getDate();
    var month = month[today.getMonth()];
    var year = today.getFullYear();
    var hours = today.getHours() < 10 ? '0' + today.getHours() : today.getHours();
    var minutes = today.getMinutes() < 10 ? '0' + today.getMinutes() : today.getMinutes();
    return day + "-" + month + "-" + year + " " + hours + ":" + minutes;
}

function getJsDateToday() {
    //get current date
    var today = new Date();
    var day = today.getDate() < 10 ? '0' + today.getDate() : today.getDate();
    var month = (today.getMonth() + 1) < 10 ? '0' + (today.getMonth() + 1) : (today.getMonth() + 1);
    var year = today.getFullYear();
    return year + "-" + month + "-" + day;;
}

function getCurrentMonthYear() {
    //get current date
    var today = new Date();
    //get current month
    var month = today.getMonth() + 1;
    var curMonth = month < 10 ? '0' + month : month;

    var monthYr = curMonth + "/" + today.getFullYear().toString()
    return monthYr;
}

function number_format(number, decimals, dec_point, thousands_sep)
{
    number = (number + '')
   .replace(/[^0-9+\-Ee.]/g, '');
    var n = !isFinite(+number) ? 0 : +number,
      prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
      sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
      dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
      s = '',
      toFixedFix = function (n, prec) {
          var k = Math.pow(10, prec);
          return '' + (Math.round(n * k) / k)
            .toFixed(prec);
      };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n))
      .split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '')
      .length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1)
          .join('0');
    }
    return s.join(dec);
}

function getCurrentFY(jsDate) {
    //get current date
    var today = new Date(
                            parseInt((jsDate).replace(/(^.*\()|([+-].*$)/g, ''))
                            );
    //get current month
    var curMonth = today.getMonth();

    var fiscalYr = "";
    if (curMonth > 3) { //
        var nextYr1 = (today.getFullYear() + 1).toString();
        fiscalYr = today.getFullYear().toString() + "-" + nextYr1.charAt(2) + nextYr1.charAt(3);
    } else {
        var nextYr2 = today.getFullYear().toString();
        fiscalYr = (today.getFullYear() - 1).toString() + "-" + nextYr2.charAt(2) + nextYr2.charAt(3);
    }
    return fiscalYr;
}

function addCommas(nStr) {
    nStr += '';
    var x = nStr.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

