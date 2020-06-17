
template _tmp_831
{
    name = "template1";
    type = GRAPHICAL;
    width = 110;
    maxheight = 110;
    columns = (1, 1);
    gap = 5;
    fillpolicy = EVEN;
    filldirection = HORIZONTAL;
    fillstartfrom = TOPLEFT;
    margins = (0, 0, 0, 0);
    gridxspacing = 0.5;
    gridyspacing = 0.5;
    version = 4;
    created = "08.02.2006 20:02";
    modified = "17.06.2020 02:56";
    notes = "";
    colors = "153;152;160;161;162;163;164;165;154;155;156;157;158;159;";

    row _tmp_832
    {
        name = "Row";
        height = 103.5;
        visibility = TRUE;
        usecolumns = FALSE;
        rule = "";
        contenttype = "DRAWING";
        sorttype = COMBINE;

        text _tmp_869
        {
            name = "Project";
            x1 = 1.06518570409389;
            y1 = 64;
            x2 = 1.06518570409389;
            y2 = 64;
            string = "PROJECT:";
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 1.8;
            fontratio = 1;
            fontslant = 0;
            fontstyle = 0;
            angle = 0;
            justify = CENTERED;
            pen = -1;
        };

        text _tmp_871
        {
            name = "Text_1";
            x1 = 2;
            y1 = 49;
            x2 = 2;
            y2 = 49;
            string = "TITLE:";
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 1.8;
            fontratio = 1;
            fontslant = 0;
            fontstyle = 0;
            angle = 0;
            justify = CENTERED;
            pen = -1;
        };

        text _tmp_872
        {
            name = "Text_2";
            x1 = 1.5;
            y1 = 34;
            x2 = 1.5;
            y2 = 34;
            string = "SCALE:";
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 1.8;
            fontratio = 1;
            fontslant = 0;
            fontstyle = 0;
            angle = 0;
            justify = LEFT;
            pen = -1;
        };

        text _tmp_873
        {
            name = "Text_3";
            x1 = 73.5;
            y1 = 23.5;
            x2 = 73.5;
            y2 = 23.5;
            string = "DATE:";
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 1.8;
            fontratio = 1;
            fontslant = 0;
            fontstyle = 0;
            angle = 0;
            justify = LEFT;
            pen = -1;
        };

        text _tmp_874
        {
            name = "Text_4";
            x1 = 55.22216796875;
            y1 = 23.5;
            x2 = 55.22216796875;
            y2 = 23.5;
            string = "DRAWN:";
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 1.8;
            fontratio = 1;
            fontslant = 0;
            fontstyle = 0;
            angle = 0;
            justify = CENTERED;
            pen = -1;
        };

        text _tmp_876
        {
            name = "Text_5";
            x1 = 38.109619140625;
            y1 = 34;
            x2 = 38.109619140625;
            y2 = 34;
            string = "CHECKED:";
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 1.8;
            fontratio = 1;
            fontslant = 0;
            fontstyle = 0;
            angle = 0;
            justify = CENTERED;
            pen = -1;
        };

        text _tmp_877
        {
            name = "Text_6";
            x1 = 1.1658935546875;
            y1 = 12;
            x2 = 1.1658935546875;
            y2 = 12;
            string = "DRAWING Name:";
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 1.8;
            fontratio = 1;
            fontslant = 0;
            fontstyle = 0;
            angle = 0;
            justify = CENTERED;
            pen = -1;
        };

        text _tmp_878
        {
            name = "Text_7";
            x1 = 90.72021484375;
            y1 = 12;
            x2 = 90.72021484375;
            y2 = 12;
            string = "REV:";
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 1.8;
            fontratio = 1;
            fontslant = 0;
            fontstyle = 0;
            angle = 0;
            justify = CENTERED;
            pen = -1;
        };

        valuefield _tmp_881
        {
            name = "SCALE1_field";
            location = (10.0006103515625, 29.5);
            formula = "GetValue(\"SCALE1\")";
            datatype = STRING;
            class = "";
            cacheable = TRUE;
            justify = RIGHT;
            visibility = TRUE;
            angle = 0;
            length = 10;
            decimals = 0;
            sortdirection = ASCENDING;
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 2.5;
            fontratio = 1.5;
            fontstyle = 0;
            fontslant = 0;
            pen = -1;
            oncombine = NONE;
        };

        valuefield _tmp_884
        {
            name = "DRAWN";
            location = (60.501220703125, 19);
            formula = "GetValue(\"USERDEFINED.DR_DRAWN_BY\")";
            datatype = STRING;
            class = "";
            cacheable = TRUE;
            justify = RIGHT;
            visibility = TRUE;
            angle = 0;
            length = 4;
            decimals = 0;
            sortdirection = ASCENDING;
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 2.5;
            fontratio = 1.5;
            fontstyle = 0;
            fontslant = 0;
            pen = -1;
            oncombine = NONE;
        };

        valuefield _tmp_886
        {
            name = "CHECKED";
            location = (57.5, 29.5);
            formula = "GetValue(\"USERDEFINED.DR_CHECKED_BY\")";
            datatype = STRING;
            class = "";
            cacheable = TRUE;
            justify = RIGHT;
            visibility = TRUE;
            angle = 0;
            length = 5;
            decimals = 0;
            sortdirection = ASCENDING;
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 2.5;
            fontratio = 1.5;
            fontstyle = 0;
            fontslant = 0;
            pen = -1;
            oncombine = NONE;
        };

        valuefield _tmp_888
        {
            name = "DATE(APPROVAL_SENT)";
            location = (91, 19);
            formula = "GetValue(\"USERDEFINED.DR_APPROVAL_SENT\")";
            datatype = INTEGER;
            class = "Date";
            cacheable = TRUE;
            justify = RIGHT;
            visibility = TRUE;
            angle = 0;
            length = 10;
            decimals = 0;
            sortdirection = ASCENDING;
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 2.5;
            fontratio = 1;
            fontstyle = 0;
            fontslant = 0;
            pen = -1;
            oncombine = NONE;
            unit = "dd.mm.yyyy";
        };

        valuefield _tmp_890
        {
            name = "DRAWING_NUMBER";
            location = (8.5, 5);
            formula = "GetValue(\"TITLE\")";
            maxnumoflines = 1;
            datatype = STRING;
            class = "";
            cacheable = TRUE;
            formatzeroasempty = FALSE;
            justify = CENTERED;
            visibility = TRUE;
            angle = 0;
            length = 20;
            decimals = 0;
            sortdirection = ASCENDING;
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 4;
            fontratio = 1.5;
            fontstyle = 0;
            fontslant = 0;
            pen = -1;
            oncombine = NONE;
            aligncontenttotop = FALSE;
        };

        valuefield _tmp_898
        {
            name = "Title1";
            location = (16.509765625, 42);
            formula = "GetValue(\"TITLE1\")";
            maxnumoflines = 1;
            datatype = STRING;
            class = "";
            cacheable = TRUE;
            formatzeroasempty = FALSE;
            justify = CENTERED;
            visibility = TRUE;
            angle = 0;
            length = 20;
            decimals = 0;
            sortdirection = ASCENDING;
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 4;
            fontratio = 1.5;
            fontstyle = 0;
            fontslant = 0;
            pen = -1;
            oncombine = NONE;
        };

        valuefield _tmp_902
        {
            name = "PROJECT";
            location = (13.5, 56);
            formula = "GetValue(\"PROJECT.NAME\")";
            maxnumoflines = 1;
            datatype = STRING;
            class = "";
            cacheable = TRUE;
            formatzeroasempty = FALSE;
            justify = CENTERED;
            visibility = TRUE;
            angle = 0;
            length = 20;
            decimals = 0;
            sortdirection = ASCENDING;
            fontname = "Calibri";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 5;
            fontratio = 1.5;
            fontstyle = 0;
            fontslant = 0;
            pen = -1;
            oncombine = NONE;
        };

        text _tmp_842
        {
            name = "Text_14";
            x1 = 73.941650390625;
            y1 = 34;
            x2 = 73.941650390625;
            y2 = 34;
            string = "APPROVED:";
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 1.8;
            fontratio = 1;
            fontslant = 0;
            fontstyle = 0;
            angle = 0;
            justify = CENTERED;
            pen = -1;
        };

        valuefield _tmp_844
        {
            name = "APPROVED_BY";
            location = (83, 29.5);
            formula = "GetValue(\"USERDEFINED.DR_APPROVED_BY\")";
            datatype = STRING;
            class = "";
            cacheable = TRUE;
            justify = LEFT;
            visibility = TRUE;
            angle = 0;
            length = 10;
            decimals = 0;
            sortdirection = ASCENDING;
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 2.5;
            fontratio = 1.5;
            fontstyle = 0;
            fontslant = 0;
            pen = -1;
            oncombine = NONE;
        };

        valuefield _tmp_845
        {
            name = "REV";
            location = (92, 5);
            formula = "GetValue(\"REVISION.MARK\")";
            datatype = STRING;
            class = "";
            cacheable = TRUE;
            justify = RIGHT;
            visibility = TRUE;
            angle = 0;
            length = 4;
            decimals = 0;
            sortdirection = ASCENDING;
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 4;
            fontratio = 1.5;
            fontstyle = 0;
            fontslant = 0;
            pen = -1;
            oncombine = NONE;
        };

        rectangle _tmp_45
        {
            name = "Rectangle_1";
            x1 = 0;
            y1 = 37;
            x2 = 110;
            y2 = 69.5;
            filled = FALSE;
            filltype = -1;
            pen = -1;
            color = 161;
            linetype = 1;
            linewidth = 4;
        };

        lineorarc _tmp_47
        {
            name = "LineOrArc";
            x1 = 0;
            y1 = 53.5;
            x2 = 110;
            y2 = 53.5;
            pen = -1;
            color = 164;
            linetype = 1;
            linewidth = 1;
            bulge = 0;
        };

        rectangle _tmp_48
        {
            name = "Rectangle_2";
            x1 = 0;
            y1 = 15;
            x2 = 110;
            y2 = 0;
            filled = FALSE;
            filltype = -1;
            pen = -1;
            color = 161;
            linetype = 1;
            linewidth = 4;
        };

        lineorarc _tmp_50
        {
            name = "LineOrArc_1";
            x1 = 0;
            y1 = 26;
            x2 = 110;
            y2 = 26;
            pen = -1;
            color = 164;
            linetype = 1;
            linewidth = 1;
            bulge = 0;
        };

        lineorarc _tmp_53
        {
            name = "LineOrArc_2";
            x1 = 90;
            y1 = 15;
            x2 = 90;
            y2 = 0;
            pen = -1;
            color = 164;
            linetype = 1;
            linewidth = 1;
            bulge = 0;
        };

        lineorarc _tmp_57
        {
            name = "LineOrArc_3";
            x1 = 36.5;
            y1 = 37;
            x2 = 36.5;
            y2 = 15;
            pen = -1;
            color = 164;
            linetype = 1;
            linewidth = 1;
            bulge = 0;
        };

        lineorarc _tmp_58
        {
            name = "LineOrArc_4";
            x1 = 72.5;
            y1 = 37;
            x2 = 72.5;
            y2 = 15;
            pen = -1;
            color = 164;
            linetype = 1;
            linewidth = 1;
            bulge = 0;
        };

        lineorarc _tmp_60
        {
            name = "LineOrArc_5";
            x1 = 54.5;
            y1 = 26;
            x2 = 54.5;
            y2 = 15;
            pen = -1;
            color = 164;
            linetype = 1;
            linewidth = 1;
            bulge = 0;
        };

        text _tmp_62
        {
            name = "Text_9";
            x1 = 38;
            y1 = 23.5;
            x2 = 38;
            y2 = 23.5;
            string = "DESIGNED:";
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 1.8;
            fontratio = 1;
            fontslant = 0;
            fontstyle = 0;
            angle = 0;
            justify = CENTERED;
            pen = -1;
        };

        text _tmp_63
        {
            name = "Text_10";
            x1 = 1;
            y1 = 23;
            x2 = 1;
            y2 = 23;
            string = "PROJECT No:";
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 1.8;
            fontratio = 1;
            fontslant = 0;
            fontstyle = 0;
            angle = 0;
            justify = CENTERED;
            pen = -1;
        };

        valuefield _tmp_65
        {
            name = "PROJECT.NUMBER";
            location = (9.5, 19);
            formula = "GetValue(\"PROJECT.NUMBER\")";
            datatype = STRING;
            class = "";
            cacheable = TRUE;
            justify = RIGHT;
            visibility = TRUE;
            angle = 0;
            length = 10;
            decimals = 0;
            sortdirection = ASCENDING;
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 2.5;
            fontratio = 1.5;
            fontstyle = 0;
            fontslant = 0;
            pen = -1;
            oncombine = NONE;
        };

        valuefield _tmp_67
        {
            name = "ASSIGNED_BY";
            location = (42.5, 19);
            formula = "GetValue(\"USERDEFINED.DR_ASSIGNED_BY\")";
            maxnumoflines = 1;
            datatype = STRING;
            class = "";
            cacheable = TRUE;
            formatzeroasempty = FALSE;
            justify = CENTERED;
            visibility = TRUE;
            angle = 0;
            length = 4;
            decimals = 0;
            sortdirection = ASCENDING;
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 2.5;
            fontratio = 1.5;
            fontstyle = 0;
            fontslant = 0;
            pen = -1;
            oncombine = NONE;
            aligncontenttotop = FALSE;
        };

        rectangle _tmp_1
        {
            name = "Rectangle";
            x1 = 0;
            y1 = 15;
            x2 = 110;
            y2 = 37;
            filled = FALSE;
            filltype = -1;
            pen = -1;
            color = 161;
            linetype = 1;
            linewidth = 4;
        };

        text _tmp_4
        {
            name = "Text";
            x1 = 1.5;
            y1 = 100;
            x2 = 1.5;
            y2 = 100;
            string = "Powered by:";
            fontname = "Arial";
            fontcolor = 153;
            fonttype = 2;
            fontsize = 1.8;
            fontratio = 1;
            fontslant = 0;
            fontstyle = 0;
            angle = 0;
            justify = CENTERED;
            pen = -1;
        };

        picture _tmp_0
        {
            name = "Picture";
            file = "AUTRA.PNG";
            refpoint = (5.5, 74.5);
            height = 21;
            width = 96;
            keepaspect = TRUE;
        };

        rectangle _tmp_4
        {
            name = "Rectangle_3";
            x1 = 0;
            y1 = 69.5;
            x2 = 110;
            y2 = 103;
            filled = FALSE;
            filltype = -1;
            pen = -1;
            color = 161;
            linetype = 1;
            linewidth = 4;
        };
    };
};
