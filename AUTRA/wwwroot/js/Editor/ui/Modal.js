$(document).ready();
$("#manMode").click(function () { //Hide Auto mode data if Manual mode selected
    $("#autoModeData").fadeOut();
});

$("#autoMode").click(function () { //Show Auto mode data if Manual mode not selected
    $("#autoModeData").fadeIn();
}); 