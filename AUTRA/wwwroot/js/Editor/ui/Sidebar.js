var dark=document.getElementById("dark");
var light=document.getElementById("light");
light.style.display='none';

//$(function () {
//    $('[data-toggle="popover"]').popover()
//  })

$('#sidebarBtn').click(function () {
    $(this).toggleClass("click");
    $('.sidebar').toggleClass("show");
});
$('.feat-btn').click(function () {
    $('nav ul .feat-show').toggleClass("show");
    $('nav ul .first').toggleClass("rotate");
});

            $('#elementSec').click(function () {
                $('#elementSecDetails').fadeToggle();
            });

    
            // $('#elementSec').popover({
            //     trigger: 'hover'
            // })
    
            $('#elementMove').click(function () {
                $('#elementMoveDetails').fadeToggle();
            });

            $('#elementCopy').click(function () {
                $('#elementCopyDetails').fadeToggle();
            });

$('.serv-btn').click(function () {
    $('nav ul .serv-show').toggleClass("show1");
    $('nav ul .second').toggleClass("rotate");
});

            $('#drawElement').click(function () {
                $('#drawElementDetails').fadeToggle();
            });

            $('#addNodeCoord').click(function () {
                $('#addNodeCoordDetails').fadeToggle();
            });

            $('#addNodeBeam').click(function () {
            $('#addNodeBeamDetails').fadeToggle();
            });


$('.loads-btn').click(function () {
    $('nav ul .loads-show').toggleClass("show2");
    $('nav ul .third').toggleClass("rotate");
});
            $('#addFloorLoad').click(function () {
                $('#addFloorLoadDetails').fadeToggle();
            });

            $('#addLineLoad').click(function () {
                $('#addLineLoadDetails').fadeToggle();
            });

            $('#addPointLoad').click(function () {
                $('#addPointLoadDetails').fadeToggle();
            });


$('.myModel-btn').click(function () {
    $('nav ul .myModel-show').toggleClass("show3");
    $('nav ul .fourth').toggleClass("rotate");
});

//$('#mySelectBtn').click(function () {
//    $('#mySelectShow').fadeToggle();
//});

//$('.mySelect-btn').click(function () {
//    console.log("jqworking");
//    $('nav ul .mySelect-show').toggleClass("show5");
//    $('nav ul .fifth').toggleClass("rotate");
//});

$('nav ul li').click(function () {
    $(this).addClass("active").siblings().removeClass("active");
});
