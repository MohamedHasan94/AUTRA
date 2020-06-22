$(document).ready();
$("#manMode").click(function () { //Hide Auto mode data if Manual mode selected
    $("#autoModeData").fadeOut();
});

$("#autoMode").click(function () { //Show Auto mode data if Manual mode not selected
    $("#autoModeData").fadeIn();
}); 


$('#modalOpenProjectTitle').click(function () {
    $("#modalNewProjectDialog1").hide();
    $("#modalOpenProjectDialog").show();
    $('.fontBlue').removeClass("fontBlue");
    $('#modalOpenProjectTitle').addClass("fontBlue");

});

$('#modalNewProjectTitle').click(function () {
    $("#modalOpenProjectDialog").hide();
    $("#modalNewProjectDialog1").show();
    $('.fontBlue').removeClass("fontBlue");
    $('#modalNewProjectTitle').addClass("fontBlue");
});

//$('#modalFlipForward').click(function () {
//    $("#modalPage1").hide();
//    $("#modalPage2").show();
//});

//$('#form1ProjectData').submit(function (event) {
//    event.preventDefault();
//    $("#modalPage1").hide();
//    $("#modalPage2").show();
//});

//$("#modalFlipForward").on("click", function () {
//    var yourInputElement = $("#form1ProjectData")[0];
//    yourInputElement.checkValidity();
//    yourInputElement.reportValidity();
//})

$(' #closeStartUpBtn ').click(function () {
    $('#modalDivDetails').fadeOut();
});

$('#modalFlipBackward').click(function () {
    $("#modalPage2").hide();
    $("#modalPage1").show();
});


$(document).ready(function () {
$("form[name='form1ProjectData']").validate({
        // Specify validation rules
        rules: {
            project: "required",
            designer: "required",
            //location: "required",
            //city: "required",
            country: "required",
            owner: "required",
        },
        // Specify validation error messages
        messages: {
            project: "Please enter the project's title. ",
            designer: "Please enter designer's name. ",
            location: "Please enter the location. ",
            city: "Please enter the city. ",
            country: "Please enter the country.",
            owner: "Please enter the owner's name.",
        },
});

    $("form[name='form2StructureData']").validate({
        // Specify validation rules
        rules: {
            secSpace: "required",
            spaceY: "required",
            //location: "required",
            //city: "required",
            spaceZ: "required",
            spaceX: "required",
        },
        // Specify validation error messages
        messages: {
            secSpace: "Please enter the secondary beams spacing. ",
            spaceY: "Please enter the structure's height. ",
            spaceZ: "Please enter the spacing in y direction. ",
            spaceX: "Please enter the spacing in x direction. ",
        },
    });


$('#modalFlipForward').click(function () {  // capture the click
    if ($("#form1ProjectData").valid()) {   // test for validity
        $("#modalPage1").hide();
        $("#modalPage2").show();
    }
});

});