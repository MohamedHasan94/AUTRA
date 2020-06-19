var dark=document.getElementById("dark");
var light = document.getElementById("light");
var analysisResult = document.getElementById("analysisResult");


$('#sidebarBtn').click(function () {
    $(this).toggleClass("click");
    $('.sidebar').toggleClass("show");
});

function flipDiv(id) {
    $('.activeDiv').fadeOut();
    $('.activeDiv').removeClass("activeDiv");
    $(id).addClass("activeDiv");
    $(id).fadeIn();
}

$('#drawElement').click(function () {
    flipDiv('#drawElementDetails');
});
$(' #addNodeCoord ').click(function () {
    flipDiv('#addNodeCoordDetails');
});
$(' #addNodeBeam ').click(function () {
    flipDiv('#addNodeBeamDetails');
});
$(' #elementSection ').click(function () {
    flipDiv('#elementSectionDetails');
});
$(' #moveSection ').click(function () {
    flipDiv('#moveSectionDetails');
});
$(' #copySection ').click(function () {
    flipDiv('#copySectionDetails');
});
$(' #floorLoadIcon ').click(function () {
    flipDiv('#floorLoadDetails');
});
$(' #lineLoadIcon ').click(function () {
    flipDiv('#lineLoadDetails');
});
$(' #pointLoadIcon ').click(function () {
    flipDiv('#pointLoadDetails');
});
$(' #showLoadIcon ').click(function () {
    flipDiv('#showLoadDetails');
});
$(' #analysisResult ').click(function () {
    flipDiv('#analysisResultDetails');
});

$(' #viewsIcon ').click(function () {
    flipDiv('#viewsDetails');
});
$(' #measureIcon ').click(function () {
    flipDiv('#measureDetails');
});


$('#analysisResultBtn').click(function () {
    $('#analysisResultDetails').fadeOut();
});
$('#drawElementBtn').click(function () {
    $('#drawElementDetails').fadeOut();
});
$(' #addNodeCoordBtn ').click(function () {
    $('#addNodeCoordDetails').fadeOut();
});
$(' #addNodeBeamBtn ').click(function () {
    $('#addNodeBeamDetails').fadeOut();
});
$(' #elementSectionBtn ').click(function () {
    $('#elementSectionDetails').fadeOut();
});
$(' #moveSectionBtn ').click(function () {
    $('#moveSectionDetails').fadeOut();
});
$(' #copySectionBtn ').click(function () {
    $('#copySectionDetails').fadeOut();
});
$(' #floorLoadBtn ').click(function () {
    $('#floorLoadDetails').fadeOut();
});
$(' #lineLoadBtn ').click(function () {
    $('#lineLoadDetails').fadeOut();
});
$(' #pointLoadBtn ').click(function () {
    $('#pointLoadDetails').fadeOut();
});
$(' #showLoadBtn ').click(function () {
    $('#showLoadDetails').fadeOut();
});
$(' #analysisResultBtn ').click(function () {
    $('#analysisResultDetails').fadeOut();
});
$(' #viewsBtn ').click(function () {
    $('#viewsDetails').fadeOut();
});
$(' #measureBtn ').click(function () {
    $('#measureDetails').fadeOut();
});